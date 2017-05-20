namespace Infrastructure.Repositories.AzureTables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.AuthorizationModels;
    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using AzureTablesObjects;
    using AzureTablesObjects.TableEntities;

    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    ///     Class UserIdentityRepository
    /// </summary>
    public class UserIdentityRepository : IUserIdentityRepository
    {
        #region Fields

        private readonly AzureTableStorageManager _azureManager;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserIdentityRepository" /> class.
        /// </summary>
        public UserIdentityRepository()
        {
            _azureManager = new AzureTableStorageManager(AzureTableName.UserIdentities);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the user identity.
        /// </summary>
        /// <param name="userIdentity">The user identity.</param>
        /// <returns>Task{AddResult}.</returns>
        public async Task<IdResult> AddUserIdentity(UserIdentity userIdentity)
        {
            var checkExistanceResult = await CheckIdentityExist(userIdentity);
            if (!String.IsNullOrEmpty(checkExistanceResult.Id))
            {
                return new IdResult(checkExistanceResult.Id)
                           {
                               Description = "Specified identity is already in use", 
                               Status = OperationResultStatus.Warning
                           };
            }

            userIdentity.Id = Guid.NewGuid().ToString("N");
            var userIdentityAzure = userIdentity.ToAzureModel();
            return await _azureManager.AddEntityAsync(userIdentityAzure);
        }

        /// <summary>
        /// Updates the user identity.
        /// </summary>
        /// <param name="userIdentity">The user identity.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> UpdateUserIdentity(UserIdentity userIdentity)
        {
            var result =
                await
                _azureManager.GetEntitiesAsync(
                    new TableQuery<UserIdentityAzure>().Where(GetFilterByPartitionKey(userIdentity.Id)));
            var userIdentityAzure = result.FirstOrDefault(i => i.RowKey == AzureTableConstants.UserIdentityRowKey);
            if (userIdentityAzure == null)
            {
                return new IdResult(OperationResultStatus.Error, "Can't find user identity for update");
            }

            //stub to check vk existance, because we can't check with simple filter for vk in current realization
            if (userIdentityAzure.Vk_Id == 0 && userIdentity.VkIdentity != null && userIdentity.VkIdentity.Id > 0)
            {
                var checkExistanceResult = await CheckIdentityExist(new UserIdentity { VkIdentity = userIdentity.VkIdentity });
                if (!String.IsNullOrEmpty(checkExistanceResult.Id))
                {
                    return new OperationResult(OperationResultStatus.Error, "Vk account is already in use");
                }
            }

            var updatedIdentity = userIdentity.ToAzureModel();
            updatedIdentity.CopyToTableEntity(userIdentityAzure);
            return await _azureManager.UpdateEntityAsync(userIdentityAzure);
        }

        /// <summary>
        ///     Gets the user identity.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{UserIdentity}.</returns>
        public async Task<UserIdentity> GetUserIdentity(string id)
        {
            var result =
                await
                _azureManager.GetEntitiesAsync(new TableQuery<UserIdentityAzure>().Where(GetFilterByPartitionKey(id)));
            var userIdentityAzure = result.FirstOrDefault();
            return userIdentityAzure?.FromAzureModel();
        }

        /// <summary>
        ///     Gets the users identities.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{UserIdentity}}.</returns>
        public async Task<IEnumerable<UserIdentity>> GetUsersIdentities(QueryOptions<UserIdentity> options)
        {
            var tableQuery = options.GenerateTableQuery<UserIdentity, UserIdentityAzure>();
            var azureIdentities = await _azureManager.GetEntitiesAsync(tableQuery);
            var userIdentities = azureIdentities.Select(a => a.FromAzureModel()).ToList();
            return userIdentities.FilterCollectionPostFactum(options);
        }

        

        #endregion

        #region Methods

        private async Task<IdResult> CheckIdentityExist(UserIdentity userIdentity)
        {
            //Email is checked inside the controller, so vk only needs to be checked
            if (userIdentity.VkIdentity != null)
            {
                var tableQuery = TableQuery.GenerateFilterConditionForInt(
                    "Vk_Id",
                    QueryComparisons.Equal,
                    (int)userIdentity.VkIdentity.Id);

                var identities =
                    await _azureManager.GetEntitiesAsync(new TableQuery<UserIdentityAzure>().Where(tableQuery));
                var foundIdentity = identities.FirstOrDefault();
                return foundIdentity == null ? new IdResult(String.Empty) : new IdResult(foundIdentity.Id);
            }
            return new IdResult(String.Empty);
        }

        private string GetFilterByPartitionKey(string partitionKey)
        {
            return TableQuery.GenerateFilterCondition(
                AzureTableConstants.PartitionKey, 
                QueryComparisons.Equal, 
                partitionKey);
        }


        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            // nothing to dispose
        }

        #endregion
    }
}