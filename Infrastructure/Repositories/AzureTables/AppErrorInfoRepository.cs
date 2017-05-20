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
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using AzureTablesObjects;
    using AzureTablesObjects.TableEntities;

    using Core.Tools;

    /// <summary>
    ///     Class AppErrorInfoRepository
    /// </summary>
    public sealed class AppErrorInfoRepository : IAppErrorInfoRepository
    {
        #region Fields

        private readonly AzureTableStorageManager _azureManager;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AppErrorInfoRepository" /> class.
        /// </summary>
        public AppErrorInfoRepository()
        {
            _azureManager = new AzureTableStorageManager(AzureTableName.AppErrorInfo);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the appErrorInfo.
        /// </summary>
        /// <param name="appErrorInfo">
        ///     The appErrorInfo.
        /// </param>
        /// <returns>
        ///     Task{AddResult}.
        /// </returns>
        public async Task<IdResult> AddAppErrorInfo(AppErrorInfo appErrorInfo)
        {
            appErrorInfo.Id = Guid.NewGuid().ToString("N");
            var azureModel = appErrorInfo.ToAzureModel();
            return await _azureManager.AddEntityAsync(azureModel);
        }

        /// <summary>
        ///     Deletes the appErrorInfo.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <returns>
        ///     Task{OperationResult}.
        /// </returns>
        public async Task<OperationResult> DeleteAppErrorInfo(string id)
        {
            var appErrorInfoAzure = await _azureManager.GetEntityByIdAndRowKeyAsync<AppErrorInfoAzure>(id, AzureTableConstants.AppErrorInfoRowKey);
            if (appErrorInfoAzure != null)
            {
                var result = await _azureManager.DeleteEntityAsync(appErrorInfoAzure);
                return result;
            }

            return new OperationResult(OperationResultStatus.Warning, "No entity to delete");
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //nothing to dispose
        }

        /// <summary>
        ///     Gets the appErrorInfo.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <returns>
        ///     Task{AppErrorInfo}.
        /// </returns>
        public async Task<AppErrorInfo> GetAppErrorInfo(string id)
        {
            var azureModel = await _azureManager.GetEntityByIdAndRowKeyAsync<AppErrorInfoAzure>(id, AzureTableConstants.AppErrorInfoRowKey);
            return azureModel.FromAzureModel();
        }

        /// <summary>
        ///     Gets the appErrorInfos.
        /// </summary>
        /// <param name="options">
        ///     The options.
        /// </param>
        /// <returns>
        ///     Task{IEnumerable{AppErrorInfo}}.
        /// </returns>
        public async Task<IEnumerable<AppErrorInfo>> GetAppErrorInfos(QueryOptions<AppErrorInfo> options)
        {
            var tableQuery = options.GenerateTableQuery<AppErrorInfo, AppErrorInfoAzure>();
            var result = await _azureManager.GetEntitiesAsync(tableQuery);
            var azureAppErrorInfos = result.Select(azureModel => azureModel.FromAzureModel()).ToList();
            return azureAppErrorInfos.FilterCollectionPostFactum(options);
        }

        #endregion
    }
}