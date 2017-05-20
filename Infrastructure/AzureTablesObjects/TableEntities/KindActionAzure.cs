namespace Infrastructure.AzureTablesObjects.TableEntities
{
    using System;

    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// The kindAction azure.
    /// </summary>
    public class KindActionAzure : TableEntity
    {

        #region Public Properties

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string UserId {
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
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>        
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date added.
        /// </summary>
        /// <value>
        /// The date added.
        /// </value>
        public DateTime DateAdded { get; set; }

        /// <summary>
        /// Gets or sets the likes.
        /// </summary>
        /// <value>
        /// The likes.
        /// </value>
        public string Likes { get; set; }

        /// <summary>
        /// Gets or sets the dislikes.
        /// </summary>
        /// <value>
        /// The dislikes.
        /// </value>
        public string Dislikes { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        public string Id
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

        #endregion
    }
}