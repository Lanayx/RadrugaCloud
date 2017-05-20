namespace Infrastructure.Repositories.AzureTables
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.Constants;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using AzureTablesObjects;
    using AzureTablesObjects.TableEntities;

    using Core.Interfaces.Repositories.Common;
    using Core.Tools;

    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    ///     Class UserRepository
    /// </summary>
    public sealed class UserRepository : IUserRepository
    {
        /// <summary>
        ///     The _azure manager
        /// </summary>
        private readonly AzureTableStorageManager _azureManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserRepository" /> class.
        /// </summary>
        public UserRepository()
        {
            _azureManager = new AzureTableStorageManager(AzureTableName.User);
        }

        /// <summary>
        ///     Adds user after adding identity. Id generation doesn't happen.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<IdResult> AddUser(User user)
        {
            var azureModel = user.ToAzureModel();
            var typeLinks = GeneratePersonQualityLinks(user);

            var batch = new List<UserAzure> { azureModel };

            if (typeLinks.Any())
            {
                batch.AddRange(typeLinks);
            }

            return await _azureManager.AddEntityBatchAsync(batch);
        }

        /// <summary>
        ///     Decreases the kind action scales for all users.
        /// </summary>
        /// <returns></returns>
        public Task<OperationResult> DecreaseKindActionScales()
        {
            return
                _azureManager.UpdateEntitiesAsync(
                    new TableQuery<UserAzure>().Where(
                        TableQuery.GenerateFilterConditionForInt("KindScale", QueryComparisons.GreaterThan, 0))
                        .Select(new[] { "KindScale", "KindScaleHighCurrentDays", "KindScaleHighMaxDays" }),
                    async users =>
                        {
                            foreach (var user in users)
                            {
                                if (user.KindScale < GameConstants.KindScale.DailyRegression)
                                {
                                    user.KindScale = 0;
                                    user.KindScaleHighCurrentDays = 0;
                                }
                                else
                                {
                                    if (user.KindScale > GameConstants.KindScale.AchievementLimit)
                                    {
                                        user.KindScaleHighCurrentDays = (user.KindScaleHighCurrentDays ?? 0) + 1;
                                        if (!user.KindScaleHighMaxDays.HasValue
                                            || user.KindScaleHighCurrentDays > user.KindScaleHighMaxDays)
                                        {
                                            user.KindScaleHighMaxDays = user.KindScaleHighCurrentDays;
                                        }
                                    }
                                    user.KindScale -= GameConstants.KindScale.DailyRegression;
                                }
                                await _azureManager.UpdateEntityAsync(user, replace: false);
                            }
                        });
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //nothing to dispose
        }

        /// <summary>
        ///     The get user.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<User> GetUser(string id)
        {
            var userRelatedEntities = await _azureManager.GetEntitiesAsync(new TableQuery<UserAzure>().Where(id.GetFilterById()));
            return Converters.ConvertToUser(userRelatedEntities, true);
        }

        /// <summary>
        ///     Gets the users.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetUsers(QueryOptions<User> options)
        {
            string expandFilter;
            var needExpand = CheckExpandGetFilter(options.Expand, out expandFilter);
            var tableQuery = options.GenerateTableQuery<User, UserAzure>(expandFilter);

            var azureMissions = await _azureManager.GetEntitiesAsync(tableQuery);
            var users =
                azureMissions.GroupBy(m => m.PartitionKey)
                    .Select(group => Converters.ConvertToUser(group.ToIList(), needExpand))
                    .ToList();
            return users.FilterCollectionPostFactum(options);
        }

        /// <summary>
        ///     Removes the type of the links to deleted person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> RemoveLinksToDeletedPersonQuality(string personQualityId)
        {
            var users = await _azureManager.GetEntitiesAsync(new TableQuery<UserAzure>());
            var entitiesToDelete =
                users.Where(m => m.IsPersonQualityLink && string.Equals(m.PersonQualityId, personQualityId));
            var groups = entitiesToDelete.GroupBy(e => e.PartitionKey);
            var error = new StringBuilder();
            foreach (var group in groups)
            {
                var deleteResult = await _azureManager.DeleteEntitiesBatchAsync(group);
                if (deleteResult.Status != OperationResultStatus.Success)
                {
                    var message = $"{group.Key} delete error: {deleteResult.Description}. ";
                    error.AppendLine(message);
                }
            }

            return error.Capacity <= 0
                ? new OperationResult(OperationResultStatus.Success)
                : new OperationResult(OperationResultStatus.Error, error.ToString());
        }

        /// <summary>
        ///     Updates the last ratings places for all users.
        /// </summary>
        /// <returns></returns>
        public Task<OperationResult> UpdateLastRatingsPlaces()
        {
            return
                _azureManager.UpdateEntitiesAsync(
                    new TableQuery<UserAzure>().Where(
                        TableQuery.GenerateFilterConditionForInt("Points", QueryComparisons.GreaterThan, 0))
                        .Select(new[] { "LastRatingPlace", "Points", "UpInRatingCurrentDays", "UpInRatingMaxDays" }),
                    async users =>
                        {
                            var orderedUsers = users.OrderByDescending(u => u.Points).ThenBy(u => u.LastRatingPlace).ToArray();
                            for (var index = 0; index < orderedUsers.Length; index++)
                            {
                                var user = orderedUsers[index];
                                if (index < user.LastRatingPlace)
                                {
                                    user.UpInRatingCurrentDays = (user.UpInRatingCurrentDays ?? 0) + 1;
                                    if (user.UpInRatingMaxDays == null ||
                                        user.UpInRatingCurrentDays > user.UpInRatingMaxDays)
                                    {
                                        user.UpInRatingMaxDays = user.UpInRatingCurrentDays;
                                    }
                                }
                                user.LastRatingPlace = index;

                                await _azureManager.UpdateEntityAsync(user, replace: false);
                            }
                        });
        }

        /// <summary>
        ///     Updates the type of the links to person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <param name="personQualityName">Name of the person quality.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> UpdateLinksToPersonQuality(string personQualityId, string personQualityName)
        {
            var users = await _azureManager.GetEntitiesAsync(new TableQuery<UserAzure>());
            var entitiesToUpdate =
                users.Where(m => m.IsPersonQualityLink && string.Equals(m.PersonQualityId, personQualityId)).ToIList();
            foreach (var userAzure in entitiesToUpdate)
            {
                userAzure.PersonQualityName = personQualityName;
            }

            var groups = entitiesToUpdate.GroupBy(e => e.PartitionKey);
            var error = new StringBuilder();
            foreach (var group in groups)
            {
                var updateResult = await _azureManager.UpdateEntityBatchAsync(group.ToIList());
                if (updateResult.Status != OperationResultStatus.Success)
                {
                    var message = $"{group.Key} update error: {updateResult.Description}. ";
                    error.AppendLine(message);
                }
            }

            return error.Capacity <= 0
                ? new OperationResult(OperationResultStatus.Success)
                : new OperationResult(OperationResultStatus.Error, error.ToString());
        }

        /// <summary>
        ///     Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="replace">if set to <c>true</c> [replace].</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> UpdateUser(User user, bool replace)
        {
            //replace should be set to false only for simple changes (nickname and image)
            if (!replace)
            {
                var updatingUser = user.ToAzureModel();
                return await _azureManager.UpsertEntityAsync(updatingUser, false);
            }

            var userRelatedEntities =
                await _azureManager.GetEntitiesAsync(new TableQuery<UserAzure>().Where(user.Id.GetFilterById()));
            var userAzure = userRelatedEntities.FirstOrDefault(u => u.IsUserEntity);
            var newUser = user.ToAzureModel();
            newUser.CopyToTableEntity(userAzure);

            var entitiesToDelete = userRelatedEntities.Where(m => m.IsPersonQualityLink).ToList();
            var entitiesToAdd = GeneratePersonQualityLinks(user);
            var entitiesToUpdate = AzureTableExtensions.FilterUpdatableLinks(entitiesToAdd, entitiesToDelete);
            entitiesToUpdate.Add(userAzure);

            return
                await _azureManager.UpdateEntityBatchAsync(entitiesToUpdate, entitiesToAdd, entitiesToDelete);
        }

        private bool CheckExpandGetFilter(IEnumerable<string> expand, out string expandFilter)
        {
            // if expand is not empty, related entites should be selected so we don't need additional filter
            if (expand != null
                && expand.Any(e => string.Equals(e, "PersonQualities", StringComparison.InvariantCultureIgnoreCase)))
            {
                expandFilter = string.Empty;
                return true;
            }

            // if expand is empty, we need to add row filter to NOT select related entities
            expandFilter = TableQuery.GenerateFilterCondition(
                AzureTableConstants.RowKey,
                QueryComparisons.Equal,
                AzureTableConstants.UserRowKey);
            return false;
        }



        private List<UserAzure> GeneratePersonQualityLinks(User user)
        {
            if (user.PersonQualitiesWithScores == null)
            {
                return new List<UserAzure>();
            }

            return
                user.PersonQualitiesWithScores.Where(t => !string.IsNullOrEmpty(t.PersonQualityId))
                    .GroupBy(p => p.PersonQualityId)
                    .Select(
                        t => UserAzure.CreateLinkToPersonQuality(user.Id, t.First().PersonQualityId, t.First().Score))
                    .ToList();
        }
    }
}