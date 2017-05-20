namespace RadrugaCloud.Models.Api
{
    using System;
    using System.Runtime.Serialization;

    using Core.Tools.CopyHelper;

    /// <summary>
    /// Info about mobile device errors
    /// </summary>
    public class AppErrorInfo
    {
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
        [DataMember(Name = "errorTime")]
        public DateTime ErrorTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the current view.
        /// </summary>
        /// <value>
        /// The name of the current view.
        /// </value>
        [DataMember(Name = "currentViewName")]
        public string CurrentViewName { get; set; }

        /// <summary>
        /// Gets or sets the error data.
        /// </summary>
        /// <value>
        /// The error data.
        /// </value>
        [DataMember(Name = "errorData")]
        public string ErrorData { get; set; }

        /// <summary>
        /// Gets or sets the device platform.
        /// </summary>
        /// <value>
        /// The device platform.
        /// </value>
        [DataMember(Name = "devicePlatform")]
        public string DevicePlatform { get; set; }

        /// <summary>
        /// Gets or sets the device model.
        /// </summary>
        /// <value>
        /// The device model.
        /// </value>
        [DataMember(Name = "deviceModel")]
        public string DeviceModel { get; set; }

        /// <summary>
        /// Gets or sets the device version.
        /// </summary>
        /// <value>
        /// The device version.
        /// </value>
        [DataMember(Name = "deviceVersion")]
        public string DeviceVersion { get; set; }

        /// <summary>
        /// Gets or sets the app version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        [DataMember(Name = "appVersion")]
        public string AppVersion { get; set; }

        /// <summary>
        /// Gets or sets the disk space available kb.
        /// </summary>
        /// <value>
        /// The disk space available kb.
        /// </value>
        [DataMember(Name = "diskSpaceAvailableKb")]
        public double? DiskSpaceAvailableKb { get; set; }

        /// <summary>
        /// Gets or sets the memory available kb.
        /// </summary>
        /// <value>
        /// The memory available kb.
        /// </value>
        [DataMember(Name = "memoryAvailableKb")]
        public double? MemoryAvailableKb { get; set; }

        public Core.DomainModels.AppErrorInfo ConvertToDomain()
        {
            var appErrorInfo = new Core.DomainModels.AppErrorInfo();
            this.CopyTo(appErrorInfo);
            return appErrorInfo;
        }

    }
}
