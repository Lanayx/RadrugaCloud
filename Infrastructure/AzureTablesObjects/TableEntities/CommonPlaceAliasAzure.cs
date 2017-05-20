namespace Infrastructure.AzureTablesObjects.TableEntities
{
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Class CommonPlaceAliaseAzure
    /// </summary>
    public class CommonPlaceAliasAzure : TableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonPlaceAliasAzure"/> class.
        /// </summary>
        public CommonPlaceAliasAzure()
        {
            PartitionKey = AzureTableConstants.CommonPlacesAliasesPartitionKey;
        }


        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        /// <value>The short name.</value>
        public string ShortName
        {
            get
            {
                return RowKey;
            }

            set
            {
                RowKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>The full name.</value>
        public string FullName { get; set; }
    }
}