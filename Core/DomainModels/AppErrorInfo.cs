namespace Core.DomainModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Info about mobile device errors
    /// </summary>
    public class AppErrorInfo
    {
        /// <summary>
        /// Error Id
        /// </summary>
        [Required]
        public string Id { get; set; }

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

    }
}
