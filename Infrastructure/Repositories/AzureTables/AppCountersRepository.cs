namespace Infrastructure.Repositories.AzureTables
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Interfaces.Repositories;

    using Infrastructure.AzureTablesObjects;
    using Infrastructure.AzureTablesObjects.TableEntities;

    public sealed class AppCountersRepository : IAppCountersRepository
    {
        private readonly AzureTableStorageManager _azureManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AppCountersRepository" /> class.
        /// </summary>
        public AppCountersRepository()
        {
            _azureManager = new AzureTableStorageManager(AzureTableName.AppCounters);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //nothing to dispose
        }

        public async Task<AppCounters> GetAppCounters()
        {
            var azureModel =
                await
                _azureManager.GetEntityByIdAndRowKeyAsync<AppCountersAzure>(
                    AzureTableConstants.AppCountersPartitionKey,
                    AzureTableConstants.AppCountersRowKey);
            return azureModel.FromAzureModel() ?? new AppCounters();
        }

        public async Task<OperationResult> UpsertAppCounters(AppCounters appCounters)
        {
            var azureModel = appCounters.ToAzureModel();
            return await _azureManager.UpsertEntityAsync(azureModel, false);
        }
    }
}