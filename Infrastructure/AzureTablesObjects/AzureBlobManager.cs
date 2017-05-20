namespace Infrastructure.AzureTablesObjects
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.Enums;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;

    /// <summary>
    ///     Class AzureBlobManager
    /// </summary>
    public class AzureBlobManager
    {
        #region Fields

        private readonly string _containerName;

        private CloudBlobClient _cloudBlobClient;

        private CloudBlobContainer _cloudBlobContainer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AzureBlobManager" /> class.
        /// </summary>
        /// <param name="blobContainer">The BLOB container.</param>
        public AzureBlobManager(BlobContainer blobContainer)
        {
            _containerName = blobContainer.ToString().ToLower();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the cloud table client.
        /// </summary>
        private CloudBlobClient CloudBlobClient
        {
            get
            {
                if (_cloudBlobClient != null)
                {
                    return _cloudBlobClient;
                }

                var connectionString =
                    ConfigurationManager.ConnectionStrings["BlobStorageAzureConnectionString"].ConnectionString;
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                blobClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan.FromMilliseconds(200), 3);
                _cloudBlobClient = blobClient;
                return _cloudBlobClient;
            }
        }

        private CloudBlobContainer CloudBlobContainer
        {
            get
            {
                if (_cloudBlobContainer != null)
                {
                    return _cloudBlobContainer;
                }

                _cloudBlobContainer = CloudBlobClient.GetContainerReference(_containerName);

                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["Warming"]) && _cloudBlobContainer.CreateIfNotExists())
                {
                    // configure container for public access
                    var permissions = _cloudBlobContainer.GetPermissions();
                    permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                    _cloudBlobContainer.SetPermissions(permissions);
                }

                return _cloudBlobContainer;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the block BLOB.
        /// </summary>
        /// <param name="uniqueName">Name of the unique.</param>
        /// <param name="content">The content.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>Task{System.String}.</returns>
        public async Task<string> AddBlockBlob(string uniqueName, Stream content, string contentType)
        {
            CloudBlockBlob blob = CloudBlobContainer.GetBlockBlobReference(uniqueName);
            blob.Properties.ContentType = contentType;
            await blob.UploadFromStreamAsync(content);
            return blob.Uri.ToString();
        }

        /// <summary>
        ///     Deletes the block BLOB.
        /// </summary>
        /// <param name="blobName">The URL.</param>
        /// <returns>Task{System.Boolean}.</returns>
        public async Task<bool> DeleteBlob(string blobName)
        {
            var reference = await GetBlobReference(blobName);
            if (reference == null)
            {
                return false;
            }

            await reference.DeleteAsync();

            return true;
        }

        /// <summary>
        ///     Gets the block blobs.
        /// </summary>
        /// <returns>Task{IEnumerable{System.String}}.</returns>
        public async Task<List<string>> GetBlockBlobs()
        {
            var result = await Task.Factory.StartNew(() => CloudBlobContainer.ListBlobs());
            return result.Select(b => b.Uri.ToString()).ToList();
        }

        #endregion

        #region Methods

        private async Task<ICloudBlob> GetBlobReference(string blobName)
        {
            return await CloudBlobContainer.GetBlobReferenceFromServerAsync(blobName);
        }

        #endregion
    }
}