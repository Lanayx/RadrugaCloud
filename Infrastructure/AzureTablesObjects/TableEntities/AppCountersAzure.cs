namespace Infrastructure.AzureTablesObjects.TableEntities
{
    using System;

    using Microsoft.WindowsAzure.Storage.Table;

    public class AppCountersAzure : TableEntity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppCountersAzure"/> class.
        /// </summary>
        public AppCountersAzure()
        {
            PartitionKey = AzureTableConstants.AppCountersPartitionKey;
            RowKey = AzureTableConstants.AppCountersRowKey;
        }

        #endregion

        #region Public Properties

        public int? RegisteredUsers { get; set; }
        public int? OneMissionPassedUsers { get; set; }
        public int? KindActionsSubmited { get; set; }
        public int? TestPassed { get; set; }
        public int? VkReposts { get; set; }
        public int? FinishedUsers { get; set; }

        #endregion
    }
}