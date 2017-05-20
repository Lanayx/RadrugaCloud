namespace Infrastructure.Repositories.AzureTables
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.Tools;

    using Infrastructure.AzureTablesObjects;
    using Infrastructure.AzureTablesObjects.TableEntities;
    using Infrastructure.InfrastructureTools.Azure;

    using LinqKit;

    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    ///     Class MissionRequestRepository
    /// </summary>
    public sealed class MissionRequestRepository : IMissionRequestRepository
    {
        #region Fields

        private readonly AzureTableStorageManager _azureManager;

        private readonly ImageProvider _imagesProvider;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionRequestRepository" /> class.
        /// </summary>
        public MissionRequestRepository()
        {
            _azureManager = new AzureTableStorageManager(AzureTableName.MissionRequests);
            _imagesProvider = new ImageProvider();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the missionRequest.
        /// </summary>
        /// <param name="missionRequest">
        ///     The missionRequest.
        /// </param>
        /// <returns>
        ///     Task{AddResult}.
        /// </returns>
        public async Task<IdResult> AddMissionRequest(MissionRequest missionRequest)
        {
            missionRequest.Id = Guid.NewGuid().ToString("N");
            if (missionRequest.Proof.ImageUrls != null)
            {
                var newUrlsTasks =
                    missionRequest.Proof.ImageUrls.Select(
                        async url =>
                        await _imagesProvider.SaveImageToProductionBlobOnAdd(url, BlobContainer.MissionRequestImages));
                var newUrls = await Task.WhenAll(newUrlsTasks);
                missionRequest.Proof.ImageUrls = newUrls.ToList();
            }

            var azureModel = missionRequest.ToAzureModel();
            return await _azureManager.AddEntityAsync(azureModel);
        }

        /// <summary>
        ///     Deletes the missionRequest.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <returns>
        ///     Task{OperationResult}.
        /// </returns>
        public async Task<OperationResult> DeleteMissionRequest(string id)
        {
            var entities = await _azureManager.GetEntitiesForCompleteDelete<MissionRequestAzure>(id);
            if (entities.AnyValues())
            {
                var deletingRequest = entities[0];
                var result = await _azureManager.DeleteEntityAsync(deletingRequest);
                await _imagesProvider.DeleteImages(deletingRequest.ProofImageUrls.SplitStringByDelimiter());
                return result;
            }

            return new OperationResult(OperationResultStatus.Warning, "No entities to delete");
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //nothing to dispose
        }

        /// <summary>
        ///     Gets the missionRequest.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <returns>
        ///     Task{MissionRequest}.
        /// </returns>
        public async Task<MissionRequest> GetMissionRequest(string id)
        {
            var result =
                await
                _azureManager.GetEntitiesAsync(new TableQuery<MissionRequestAzure>().Where(GetFilterByPartitionKey(id)));
            var azureEntity = result.SingleOrDefault();
            var model = azureEntity.FromAzureModel();
            await GetExpandProperties(new List<string> { "User", "Mission" }, new List<MissionRequest> { model });
            return model;
        }

        /// <summary>
        ///     Gets the missionRequests.
        /// </summary>
        /// <param name="options">
        ///     The options.
        /// </param>
        /// <returns>
        ///     Task{IEnumerable{MissionRequest}}.
        /// </returns>
        public async Task<List<MissionRequest>> GetMissionRequests(QueryOptions<MissionRequest> options)
        {
            var tableQuery = options.GenerateTableQuery<MissionRequest, MissionRequestAzure>();

            var azureMissionRequests = await _azureManager.GetEntitiesAsync(tableQuery);

            var missionRequests =
                azureMissionRequests.Select(missionRequest => missionRequest.FromAzureModel()).ToList();

            if (options.Expand != null && missionRequests.Any())
            {
                await GetExpandProperties(options.Expand, missionRequests);
            }

            var filteredRequests = missionRequests.FilterCollectionPostFactum(options);

            return filteredRequests;
        }

        /// <summary>
        ///     Updates the missionRequest.
        /// </summary>
        /// <param name="missionRequest">
        ///     The missionRequest.
        /// </param>
        /// <returns>
        ///     Task{OperationResult}.
        /// </returns>
        public async Task<OperationResult> UpdateMissionRequest(MissionRequest missionRequest)
        {
            var missionRequestsAzure =
                await
                _azureManager.GetEntitiesAsync(
                    new TableQuery<MissionRequestAzure>().Where(GetFilterByPartitionKey(missionRequest.Id)));
            var missionRequestAzure = missionRequestsAzure.FirstOrDefault();
            if (missionRequestAzure == null)
            {
                return new OperationResult(OperationResultStatus.Error, "Can't find missionRequest for update");
            }

            var updatedMissionRequest = missionRequest.ToAzureModel();

            var oldUrls = missionRequestAzure.ProofImageUrls.SplitStringByDelimiter();
            var newUrls = missionRequest.Proof.ImageUrls;
            await
                _imagesProvider.SaveImagesToProductionBlobOnUpdate(oldUrls, newUrls, BlobContainer.MissionRequestImages);

            updatedMissionRequest.ProofImageUrls = newUrls.JoinToString();
            updatedMissionRequest.CopyToTableEntity(missionRequestAzure);

            return await _azureManager.UpdateEntityAsync(missionRequestAzure);
        }

        #endregion

        #region Methods

        private static async Task FillMissions(List<MissionRequest> missionRequests)
        {
            var missionIds = missionRequests.GroupBy(mr => mr.MissionId).Select(group => group.Key).ToArray();
            var expressions = new List<Expression<Func<Mission, bool>>>();
            for (int index = 0; index < missionIds.Length; index++)
            {
                var missionId = missionIds[index];
                if (index > 0)
                {
                    var lastCriteria = expressions[index - 1];
                    expressions.Add(mission => lastCriteria.Invoke(mission) || mission.Id == missionId);
                }
                else
                {
                    expressions.Add(mission => mission.Id == missionId);
                }
            }

            var queryOptions = new QueryOptions<Mission> { Filter = expressions.Last().Expand() };
            var missionManager = new AzureTableStorageManager(AzureTableName.Missions);


            if (missionRequests.Count > 1) //for admin list
            {
                var initialFilter = TableQuery.GenerateFilterCondition(
                    AzureTableConstants.RowKey,
                    QueryComparisons.Equal,
                    AzureTableConstants.MissionRowKey);
                var missions =
                    await
                    missionManager.GetEntitiesAsync(
                        queryOptions.GenerateTableQuery<Mission, MissionAzure>(initialFilter));
                foreach (var missionRequest in missionRequests)
                {
                    missionRequest.Mission =
                        missions.FirstOrDefault(mission => mission.Id == missionRequest.MissionId).FromAzureModel();
                }
            }
            else //for single request get full mission
            {
                var initialFilter = TableQuery.GenerateFilterCondition(
                   AzureTableConstants.PartitionKey,
                   QueryComparisons.Equal,
                   missionRequests[0].MissionId);
                var missions =
                    await missionManager.GetEntitiesAsync(queryOptions.GenerateTableQuery<Mission, MissionAzure>(initialFilter));
                missionRequests[0].Mission = Converters.ConvertToMission(missions, true);
            }
        }

        private static async Task FillUsers(List<MissionRequest> missionRequests)
        {
            var userIds = missionRequests.GroupBy(mr => mr.UserId).Select(group => group.Key).ToArray();
            var expressions = new List<Expression<Func<User, bool>>>();
            for (int index = 0; index < userIds.Length; index++)
            {
                var userId = userIds[index];
                if (index > 0)
                {
                    var lastCriteria = expressions[index - 1];
                    expressions.Add(user => lastCriteria.Invoke(user) || user.Id == userId);
                }
                else
                {
                    expressions.Add(user => user.Id == userId);
                }
            }

            var queryOptions = new QueryOptions<User> { Filter = expressions.Last().Expand() };
            var userManager = new AzureTableStorageManager(AzureTableName.User);

            if (missionRequests.Count > 1)//for admin list
            {
                var initialFilter = TableQuery.GenerateFilterCondition(
                AzureTableConstants.RowKey,
                QueryComparisons.Equal,
                AzureTableConstants.UserRowKey);
                var usersResult =
                    await userManager.GetEntitiesAsync(queryOptions.GenerateTableQuery<User, UserAzure>(initialFilter));//TODO don't load all users
                foreach (var missionRequest in missionRequests)
                {
                    missionRequest.User = usersResult.SingleOrDefault(user => user.Id == missionRequest.UserId).FromAzureModel();
                }
            }
            else //for single request get user qualities
            {
                var initialFilter = TableQuery.GenerateFilterCondition(
                   AzureTableConstants.PartitionKey,
                   QueryComparisons.Equal,
                   missionRequests[0].UserId);
                var usersResult =
                    await userManager.GetEntitiesAsync(queryOptions.GenerateTableQuery<User, UserAzure>(initialFilter));
                missionRequests[0].User = Converters.ConvertToUser(usersResult, true);
            }

            
        }

        private async Task GetExpandProperties(List<string> expand, List<MissionRequest> missionRequests)
        {
            if (missionRequests.AnyValues() && expand.AnyValues())
            {
                if (expand.Contains("User"))
                {
                    await FillUsers(missionRequests);
                }

                if (expand.Contains("Mission"))
                {
                    await FillMissions(missionRequests);
                }
            }
           
        }

        private string GetFilterByPartitionKey(string partitionKey)
        {
            return TableQuery.GenerateFilterCondition(
                AzureTableConstants.PartitionKey, 
                QueryComparisons.Equal, 
                partitionKey);
        }

        #endregion
    }
}