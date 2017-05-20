namespace Infrastructure.AzureTablesObjects.TableEntities
{
    using System;
    using System.Linq;

    using Core.Constants;
    using Core.Tools;

    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    ///     Class CommonPlaceAzure
    /// </summary>
    public class CommonPlaceAzure : TableEntity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonPlaceAzure"/> class.
        /// </summary>
        public CommonPlaceAzure()
        {
            RowKey = AzureTableConstants.CommonPlaceTempRowKeyPrefix;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommonPlaceAzure" /> class.
        /// </summary>
        /// <param name="commonPlaceName">Name of the common place.</param>
        /// <param name="approvedCoordinate">if set to <c>true</c> [approved coordinate].</param>
        public CommonPlaceAzure(string commonPlaceName, bool approvedCoordinate)
        {
            if (approvedCoordinate)
            {
                RowKey = string.Format(
                    "{0}{1}{2}",
                    AzureTableConstants.CommonPlaceApprovedRowKeyPrefix,
                    CommonConstants.StringsDelimiter,
                    commonPlaceName);
            }
            else
            {
                RowKey = string.Format(
                    "{0}{1}{2}{1}{3}",
                    AzureTableConstants.CommonPlaceTempRowKeyPrefix,
                    CommonConstants.StringsDelimiter,
                    commonPlaceName,
                    Guid.NewGuid().ToString("N"));
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the coorditane.
        /// </summary>
        /// <value>The coorditane.</value>
        public string Coordinate { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is approved coordinate.
        /// </summary>
        /// <value><c>true</c> if this instance is approved coordinate; otherwise, <c>false</c>.</value>
        public bool IsApprovedCoordinate
        {
            get
            {
                return SplittedRowKey.Count() == 2
                       && string.Equals(SplittedRowKey[0], AzureTableConstants.CommonPlaceApprovedRowKeyPrefix);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is temp coordinate.
        /// </summary>
        /// <value><c>true</c> if this instance is temp coordinate; otherwise, <c>false</c>.</value>
        public bool IsTempCoordinate
        {
            get
            {
                return SplittedRowKey.Count() == 3
                       && string.Equals(SplittedRowKey[0], AzureTableConstants.CommonPlaceTempRowKeyPrefix);
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                if (SplittedRowKey.Count() != 2 && SplittedRowKey.Count() != 3)
                {
                    return string.Empty;
                }

                return SplittedRowKey[1];
            }
        }

        /// <summary>
        ///     Gets or sets the settlement id.
        /// </summary>
        /// <value>The settlement id.</value>
        public string SettlementId
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

        #region Properties

        private string[] SplittedRowKey
        {
            get
            {
                return RowKey.SplitStringByDelimiter().ToArray();
            }
        }

        #endregion
    }
}