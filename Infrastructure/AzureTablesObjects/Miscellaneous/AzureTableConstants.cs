namespace Infrastructure.AzureTablesObjects
{
    using System;

    /// <summary>
    ///     Class AzureTableConstants
    /// </summary>
    public static class AzureTableConstants
    {
        #region Constants

        /// <summary>
        /// The application counters row key
        /// </summary>
        public const string AppCountersRowKey = "AppCounters";

        /// <summary>
        /// The application counters partition key
        /// </summary>
        public const string AppCountersPartitionKey = "AppCountersKey";

        /// <summary>
        /// The app error info row key
        /// </summary>
        public const string AppErrorInfoRowKey = "AppErrorInfo";

        /// <summary>
        /// The common place approved row key
        /// </summary>
        public const string CommonPlaceApprovedRowKeyPrefix = "CommonPlaceApproved";

        /// <summary>
        /// The common place temp row key
        /// </summary>
        public const string CommonPlaceTempRowKeyPrefix = "CommonPlaceTemp";

        /// <summary>
        /// The common places aliases partition key
        /// </summary>
        public const string CommonPlacesAliasesPartitionKey = "CommonPlacesAliases";

        /// <summary>
        ///     The draft row key
        /// </summary>
        public const string DraftRowKey = "Draft";

        /// <summary>
        ///     The mission request row key
        /// </summary>
        public const string MissionProofRowKey = "MissionProof";

        /// <summary>
        ///     The mission request row key
        /// </summary>
        public const string MissionRequestRowKey = "MissionRequest";

        /// <summary>
        ///     The missin row key
        /// </summary>
        public const string MissionRowKey = "Mission";

        /// <summary>
        /// The mission set row key
        /// </summary>
        public const string MissionSetRowKey = "MissionSet";

        /// <summary>
        /// The mission link row key prefix
        /// </summary>
        public const string MissionLinkRowKeyPrefix = "MissionLink";

        /// <summary>
        ///     The partition key
        /// </summary>
        public const string PartitionKey = "PartitionKey";

        /// <summary>
        ///     The person quality link row key prefix
        /// </summary>
        public const string PersonQualityLinkRowKeyPrefix = "PersonQualityLink";

        /// <summary>
        ///     The person quality row key
        /// </summary>
        public const string PersonQualityRowKey = "PersonQuality";

        /// <summary>
        ///     The hint link row key prefix
        /// </summary>
        public const string HintLinkRowKeyPrefix = "HintLink";

        /// <summary>
        ///     The question option row key prefix
        /// </summary>
        public const string QuestionOptionRowKeyPrefix = "QuestionOptionLink";

        /// <summary>
        ///     The question row key
        /// </summary>
        public const string QuestionRowKey = "QuestionRow";

        /// <summary>
        ///     The row key
        /// </summary>
        public const string RowKey = "RowKey";

        /// <summary>
        ///     The user identity row key
        /// </summary>
        public const string UserIdentityRowKey = "UserIdentityRow";

        /// <summary>
        ///     The user row key
        /// </summary>
        public const string UserRowKey = "UserRow";


        /// <summary>
        /// The user data message
        /// </summary>
        public const string UserDataMessage = "UserMessage";

        /// <summary>
        /// The hint request row key
        /// </summary>
        public const string HintRequestRowKey = "HintRequest";
        #endregion

        #region Static Fields

        /// <summary>
        ///     The min azure UTC date
        /// </summary>
        public static readonly DateTime MinAzureUtcDate = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion
    }
}