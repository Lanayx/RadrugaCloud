namespace Infrastructure.AzureTablesObjects.TableEntities
{
    using System;

    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    ///     The missionRequest draft azure.
    /// </summary>
    public class MissionRequestAzure : TableEntity
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionRequestAzure" /> class.
        /// </summary>
        public MissionRequestAzure()
        {
            RowKey = AzureTableConstants.MissionRequestRowKey;
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
        /// Gets or sets the mission id.
        /// </summary>
        public string MissionId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the stars count.
        /// </summary>
        /// <value>
        /// The stars count.
        /// </value>
        public int? StarsCount { get; set; }
        
        /// <summary>
        /// Gets or sets the decline reason.
        /// </summary>
        /// <value>
        /// The decline reason.
        /// </value>
        public string DeclineReason { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the last update date.
        /// </summary>
        /// <value>
        /// The last update date.
        /// </value>
        public DateTime LastUpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the proof image urls.
        /// </summary>
        /// <value>
        /// The proof image urls.
        /// </value>
        public string ProofImageUrls { get; set; }

        /// <summary>
        /// Gets or sets the proof coordinates.
        /// </summary>
        /// <value>
        /// The proof coordinates.
        /// </value>
        public string ProofCoordinates { get; set; }

        /// <summary>
        /// Gets or sets the number of tries.
        /// </summary>
        /// <value>
        /// The number of tries.
        /// </value>
        public int? NumberOfTries { get; set; }

        /// <summary>
        /// Gets or sets the created text.
        /// </summary>
        /// <value>
        /// The created text.
        /// </value>
        public string CreatedText { get; set; }

        /// <summary>
        /// Gets or sets the time elapsed in secondds
        /// </summary>
        /// <value>
        /// The time elapsed.
        /// </value>
        public double? TimeElapsed { get; set; }

        #endregion
    }
}