namespace Infrastructure.AzureTablesObjects
{
    using System;
    using System.Text.RegularExpressions;

    using Core.Enums;

    /// <summary>
    ///     Class BlobInfo
    /// </summary>
    public class BlobInfo
    {
        #region Constants

        private const string AzureBaseUrlPattern = @"^(http(s)?://defor\.blob\.core\.windows\.net)$";

        private const string BaseUrlGroup = "BaseUrl";

        private const string ContainerNameGroup = "ContainerName";

        private const string EmulatorBaseUrlPattern = @"^(http(s)?://127\.0\.0\.1.*?/\w+)$";

        private const string ExtensionGroup = "Extension";

        private const string NameGroup = "Name";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="BlobInfo" /> class from being created.
        /// </summary>
        private BlobInfo()
        {
            Name = Extension = string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public BlobContainer Container { get; private set; }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        /// <value>The extension.</value>
        public string Extension { get; private set; }

        /// <summary>
        ///     Gets the full name.
        /// </summary>
        /// <value>The full name.</value>
        public string FullName
        {
            get
            {
                return string.Concat(Name, Extension);
            }
        }

        /// <summary>
        ///     Gets the kind.
        /// </summary>
        /// <value>The kind.</value>
        public BlobKind Kind { get; private set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the info by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>BlobInfo.</returns>
        public static BlobInfo GetInfoByUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new BlobInfo { Kind = BlobKind.Empty };
            }

            var info = new BlobInfo { Container = BlobContainer.Undefined, Kind = BlobKind.External };

            var imageUrlPattern = string.Format(
                @"^(?<{0}>.+)\/(?<{1}>\w+?)\/(?<{2}>.+)(?<{3}>\.\w+)$", 
                BaseUrlGroup, 
                ContainerNameGroup, 
                NameGroup, 
                ExtensionGroup);

            var imageUrlRegex = new Regex(imageUrlPattern);
            if (!imageUrlRegex.IsMatch(url))
            {
                return info;
            }

            var collection = imageUrlRegex.Match(url).Groups;
            var baseUrl = collection[BaseUrlGroup].Value;
            var azureRegex = new Regex(AzureBaseUrlPattern);
            var emulatorRegex = new Regex(EmulatorBaseUrlPattern);
            var isAzureBlob = azureRegex.IsMatch(baseUrl);
            var isEmulatorBlob = emulatorRegex.IsMatch(baseUrl);
            info.Extension = collection[ExtensionGroup].Value;
            info.Name = collection[NameGroup].Value;
            if (!isAzureBlob && !isEmulatorBlob)
            {
                return info;
            }

            info.Container = GetBlobContainer(collection[ContainerNameGroup].Value);
            info.Kind = isAzureBlob ? BlobKind.Azure : BlobKind.Emulator;

            return info;
        }

        #endregion

        #region Methods

        private static BlobContainer GetBlobContainer(string blobContainerString)
        {
            BlobContainer container;
            return Enum.TryParse(blobContainerString, true, out container) ? container : BlobContainer.Undefined;
        }

        #endregion
    }
}