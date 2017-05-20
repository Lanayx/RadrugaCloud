namespace Infrastructure.AzureTablesObjects.TableEntities
{
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    ///     The person quality azure.
    /// </summary>
    public class PersonQualityAzure : TableEntity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonQualityAzure"/> class.
        /// </summary>
        public PersonQualityAzure()
        {
            RowKey = AzureTableConstants.PersonQualityRowKey;
        }

        #endregion

        #region Public Properties

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

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        #endregion
    }
}