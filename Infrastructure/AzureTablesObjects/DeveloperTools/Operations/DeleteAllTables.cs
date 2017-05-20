namespace Infrastructure.AzureTablesObjects.DeveloperTools.Operations
{
    using System;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.Enums;
    using Core.Tools;

    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    ///     Class DeleteAllTables
    /// </summary>
    internal class DeleteAllTables : IDeveloperOperation
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Excecutes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>OperationResult.</returns>
        public async Task<OperationResult> Excecute(params object[] args)
        {
            var helper = new DeveloperHelper();

            try
            {
                await ProcessCloudTables(helper);
                await ProcessBlobs(helper);

                return new OperationResult(OperationResultStatus.Success);
            }
            catch (Exception ex)
            {
                var message = string.Format("Exception on deleting: \n{0}", ex.ToStringExtended());
                return new OperationResult(OperationResultStatus.Error, message);
            }
        }

        #endregion

        #region Methods

        private static async Task ProcessBlobs(DeveloperHelper helper)
        {
            var blobClient = helper.CloudBlobClient;
            var containers = blobClient.ListContainers();
            foreach (var cloudBlobContainer in containers)
            {
                var blobs = cloudBlobContainer.ListBlobs();
                foreach (var listBlobItem in blobs)
                {
                    var blob = listBlobItem as CloudBlockBlob;
                    if (blob != null)
                    {
                        await blob.DeleteAsync();
                    }
                }
            }
        }

        private static async Task ProcessCloudTables(DeveloperHelper helper)
        {
            var tableClient = helper.CloudTableClient;
            var tables = tableClient.ListTables();
            foreach (var cloudTable in tables)
            {
                var items = cloudTable.ExecuteQuery(new TableQuery());
                foreach (var dynamicTableEntity in items)
                {
                    var deleteOperation = TableOperation.Delete(dynamicTableEntity);
                    await cloudTable.ExecuteAsync(deleteOperation);
                }

                /*await cloudTable.DeleteIfExistsAsync();*/
            }
        }

        #endregion
    }
}