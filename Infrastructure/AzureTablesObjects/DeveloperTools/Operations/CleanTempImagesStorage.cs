namespace Infrastructure.AzureTablesObjects.DeveloperTools.Operations
{
    using System;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.Enums;
    using Core.Tools;

    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Class CleanTempImagesStorage
    /// </summary>
    public class CleanTempImagesStorage : IDeveloperOperation
    {
        #region Public Methods and Operators

        /// <summary>
        /// Excecutes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>OperationResult.</returns>
        public async Task<OperationResult> Excecute(params object[] args)
        {
            try
            {
                var helper = new DeveloperHelper();
                var container = helper.GetBlobContainer(BlobContainer.TempStorage);
                var blobs = container.ListBlobs();
                foreach (var listBlobItem in blobs)
                {
                    var blob = listBlobItem as CloudBlockBlob;
                    if (blob != null)
                    {
                        await blob.DeleteAsync();
                    }
                }

                return new OperationResult(OperationResultStatus.Success);
            }
            catch (Exception ex)
            {
                return new OperationResult(OperationResultStatus.Error, ex.ToStringExtended());
            }
        }

        #endregion
    }
}