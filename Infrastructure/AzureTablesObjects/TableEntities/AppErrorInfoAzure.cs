namespace Infrastructure.AzureTablesObjects.TableEntities
{
    using System;

    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// The appErrorInfo draft azure.
    /// </summary>
    public class AppErrorInfoAzure : TableEntity
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AppErrorInfoAzure" /> class.
        /// </summary>
        public AppErrorInfoAzure()
        {
            RowKey = AzureTableConstants.AppErrorInfoRowKey;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the error time.
        /// </summary>
        /// <value>
        /// The error time.
        /// </value>
        public DateTime ErrorTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the current view.
        /// </summary>
        /// <value>
        /// The name of the current view.
        /// </value>
        public string CurrentViewName { get; set; }

        /// <summary>
        /// Gets or sets the error data.
        /// </summary>
        /// <value>
        /// The error data.
        /// </value>
        public string ErrorData { get; set; }

        /// <summary>
        /// Gets or sets the device platform.
        /// </summary>
        /// <value>
        /// The device platform.
        /// </value>
        public string DevicePlatform { get; set; }

        /// <summary>
        /// Gets or sets the device model.
        /// </summary>
        /// <value>
        /// The device model.
        /// </value>
        public string DeviceModel { get; set; }

        /// <summary>
        /// Gets or sets the device version.
        /// </summary>
        /// <value>
        /// The device version.
        /// </value>
        public string DeviceVersion { get; set; }

        /// <summary>
        /// Gets or sets the app version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public string AppVersion { get; set; }

        /// <summary>
        /// Gets or sets the disk space available kb.
        /// </summary>
        /// <value>
        /// The disk space available kb.
        /// </value>
        public double? DiskSpaceAvailableKb { get; set; }

        /// <summary>
        /// Gets or sets the memory available kb.
        /// </summary>
        /// <value>
        /// The memory available kb.
        /// </value>
        public double? MemoryAvailableKb { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        public string Id
        {
            get
            {
                return PartitionKey;
            }

            set
            {
                PartitionKey = value;
            }
        }

        #endregion
    }
}