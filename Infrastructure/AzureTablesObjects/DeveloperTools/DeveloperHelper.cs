namespace Infrastructure.AzureTablesObjects.DeveloperTools
{
    using System.Configuration;

    using Core.Enums;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    ///     Class DeveloperHelper
    /// </summary>
    internal class DeveloperHelper
    {
        #region Fields

        private CloudBlobClient _cloudBlobClient;

        private CloudTableClient _cloudTableClient;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the cloud table client.
        /// </summary>
        /// <value>The cloud table client.</value>
        public CloudTableClient CloudTableClient
        {
            get
            {
                if (_cloudTableClient != null)
                {
                    return _cloudTableClient;
                }

                var connectionString =
                    ConfigurationManager.ConnectionStrings["TableStorageAzureConnectionString"].ConnectionString;
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                _cloudTableClient = tableClient;
                return _cloudTableClient;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the cloud BLOB client.
        /// </summary>
        /// <value>The cloud BLOB client.</value>
        public CloudBlobClient CloudBlobClient
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
                _cloudBlobClient = blobClient;
                return _cloudBlobClient;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the BLOB container.
        /// </summary>
        /// <param name="blobContainer">The BLOB container.</param>
        /// <returns>CloudBlobContainer.</returns>
        public CloudBlobContainer GetBlobContainer(BlobContainer blobContainer)
        {
            var cloudBlobContainer = CloudBlobClient.GetContainerReference(blobContainer.ToString().ToLower());

            if (cloudBlobContainer.CreateIfNotExists())
            {
                // configure container for public access
                var permissions = cloudBlobContainer.GetPermissions();
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                cloudBlobContainer.SetPermissions(permissions);
            }

            return cloudBlobContainer;
        }

        /// <summary>
        ///     Gets the table reference.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>CloudTable.</returns>
        public CloudTable GetTableReference(AzureTableName tableName)
        {
            var cloudTableReference = CloudTableClient.GetTableReference(tableName.ToString());
            cloudTableReference.CreateIfNotExists();
            return cloudTableReference;
        }

        #endregion
    }
}