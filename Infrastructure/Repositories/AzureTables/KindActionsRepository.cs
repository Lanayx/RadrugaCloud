namespace Infrastructure.Repositories.AzureTables
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Interfaces.Repositories;
    using AzureTablesObjects;
    using AzureTablesObjects.TableEntities;

    using Microsoft.WindowsAzure.Storage.Table;
    using System.Collections.Concurrent;

    /// <summary>
    ///     Class KindActionRepository
    /// </summary>
    public sealed class KindActionRepository : IKindActionRepository
    {
        #region Fields

        private readonly AzureTableStorageManager _azureManager;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="KindActionRepository" /> class.
        /// </summary>
        public KindActionRepository()
        {
            _azureManager = new AzureTableStorageManager(AzureTableName.KindAction);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the kindAction.
        /// </summary>
        /// <param name="kindAction">
        ///     The kindAction.
        /// </param>
        /// <returns>
        ///     Task{AddResult}.
        /// </returns>
        public async Task<IdResult> AddKindAction(KindAction kindAction)
        {
            kindAction.Id = Guid.NewGuid().ToString("N");
            var azureModel = kindAction.ToAzureModel();
            return await _azureManager.AddEntityAsync(azureModel);
        }

        /// <summary>
        /// Removes the kind actions (developer purpose, not for regular use).
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task RemoveKindActions(string userId)
        {
            var filter = TableQuery.GenerateFilterCondition(
                    "UserId",
                    QueryComparisons.Equal,
                    userId);
            var kindActions = await _azureManager.GetEntitiesAsync(new TableQuery<KindActionAzure>().Where(filter));
            foreach (var kindActionAzure in kindActions)
            {
                kindActionAzure.PartitionKey = kindActionAzure.Id;
                kindActionAzure.RowKey = "KindAction";
                try
                {
                    await _azureManager.DeleteEntityAsync(kindActionAzure);
                }
                catch
                {
                    //do nothing
                }

            }           
        }

        /// <summary>
        /// Removes the kind actions (developer purpose, not for regular use).
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task RemoveDuplicateKindActions(string userId)
        {
            var filter = TableQuery.GenerateFilterCondition(
                    "UserId",
                    QueryComparisons.Equal,
                    userId);
            var kindActions = await _azureManager.GetEntitiesAsync(new TableQuery<KindActionAzure>().Where(filter));
            var uniqueDictionary = new ConcurrentDictionary<DateTime, KindActionAzure>();
            foreach (var kindActionAzure in kindActions)
            {
                if (!uniqueDictionary.TryAdd(kindActionAzure.DateAdded, kindActionAzure))
                {                    
                    try
                    {
                        await _azureManager.DeleteEntityAsync(kindActionAzure);
                    }
                    catch
                    {
                        //do nothing
                    }
                }
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //nothing to dispose
        }

        /// <summary>
        /// Gets the mission.
        /// </summary>
        /// <param name="kindActionId">The kind action identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// Task{Mission}.
        /// </returns>
        public async Task<KindAction> GetKindAction(string kindActionId, string userId)
        {
            var kindAction =
                await _azureManager.GetEntityByIdAndRowKeyAsync<KindActionAzure>(userId, kindActionId);
            return kindAction.FromAzureModel();
        }

        /// <summary>
        ///     Gets the kindActions.
        /// </summary>
        /// <param name="options">
        ///     The options.
        /// </param>
        /// <returns>
        ///     Task{IEnumerable{KindAction}}.
        /// </returns>
        public async Task<List<KindAction>> GetKindActions(QueryOptions<KindAction> options)
        {
            var tableQuery = options.GenerateTableQuery<KindAction, KindActionAzure>();
            var result = await _azureManager.GetEntitiesAsync(tableQuery);
            var azureKindActions = result.Select(azureModel => azureModel.FromAzureModel()).ToList();
            return azureKindActions.FilterCollectionPostFactum(options);
        }

        /// <summary>
        /// Updates the specified kind action.
        /// </summary>
        /// <param name="kindAction">The kind action.</param>
        /// <returns></returns>
        public async Task<OperationResult> UpdateLikes(KindAction kindAction)
        {
            var updatedKindAction = kindAction.ToAzureModel();
            return await _azureManager.UpsertEntityAsync(updatedKindAction, false);
        }

        #endregion
    }
}