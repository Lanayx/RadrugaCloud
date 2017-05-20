namespace Infrastructure.AzureTablesObjects.TableEntities
{
    using System.Linq;

    using Core.Constants;
    using Core.Tools;

    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// The MissionSet draft azure.
    /// </summary>
    public class MissionSetAzure : TableEntity
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionSetAzure" /> class.
        /// </summary>
        public MissionSetAzure()
        {
            RowKey = AzureTableConstants.MissionSetRowKey;
        }

        #endregion

        #region Public Properties

        // Common

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
        ///     Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the age from.
        /// </summary>
        public int? AgeFrom { get; set; } 

        /// <summary>
        ///     Gets or sets the age to.
        /// </summary>
        public int? AgeTo { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is MissionSet entity.
        /// </summary>
        /// <value><c>true</c> if this instance is MissionSet entity; otherwise, <c>false</c>.</value>
        public bool IsMissionSetEntity
        {
            get
            {
                return string.Equals(RowKey, AzureTableConstants.MissionSetRowKey);
            }
        }

        #endregion

        #region Mission Section

        /// <summary>
        /// Gets a value indicating whether this instance is mission link.
        /// </summary>
        /// <value><c>true</c> if this instance is mission link; otherwise, <c>false</c>.</value>
        public bool IsMissionLink
        {
            get
            {
                return !string.Equals(RowKey, AzureTableConstants.MissionSetRowKey) && SplittedRowKey.Count() == 2
                       && string.Equals(SplittedRowKey[0], AzureTableConstants.MissionLinkRowKeyPrefix);
            }
        }
        
        /// <summary>
        /// Gets or sets the order in set.
        /// </summary>
        /// <value>The order in set.</value>
        public int OrderInSet { get; set; }

        /// <summary>
        ///     Gets the person quality id.
        /// </summary>
        /// <value>The person quality id.</value>
        public string MissionId
        {
            get
            {
                return !IsMissionSetEntity ? SplittedRowKey[1] : string.Empty;
            }
        }

        /// <summary>
        /// Creates the type of the link to person.
        /// </summary>
        /// <param name="missionSetId">The MissionSet id.</param>
        /// <param name="missionId">The mission id.</param>
        /// <param name="order">The order.</param>
        /// <returns>MissionSetDraftAzure.</returns>
        public static MissionSetAzure CreateMissionLink(
            string missionSetId, 
            string missionId, 
            short order)
        {
            return new MissionSetAzure
                       {
                           PartitionKey = missionSetId,
                           RowKey =
                               string.Concat(
                                   AzureTableConstants.MissionLinkRowKeyPrefix,
                                   CommonConstants.StringsDelimiter,
                                   missionId),
                           OrderInSet = order
                       };
        }

        #endregion

        #region Person Quality Section

        /// <summary>
        ///     Gets a value indicating whether this instance is person quality link.
        /// </summary>
        /// <value><c>true</c> if this instance is person quality link; otherwise, <c>false</c>.</value>
        public bool IsPersonQualityLink
        {
            get
            {
                return !string.Equals(RowKey, AzureTableConstants.MissionSetRowKey) && SplittedRowKey.Count() == 2
                       && string.Equals(SplittedRowKey[0], AzureTableConstants.PersonQualityLinkRowKeyPrefix);
            }
        }

        /// <summary>
        ///     Gets the name of the person quality.
        /// </summary>
        /// <value>The name of the person quality.</value>
        public string PersonQualityName
        {
            get
            {
                return Name;
            }
        }

        /// <summary>
        ///     Gets or sets the person quality score.
        /// </summary>
        /// <value>The person quality score.</value>
        public double? PersonQualityScore { get; set; }

        /// <summary>
        ///     Gets the person quality id.
        /// </summary>
        /// <value>The person quality id.</value>
        public string PersonQualityId
        {
            get
            {
                return IsPersonQualityLink ? SplittedRowKey[1] : string.Empty;
            }
        }

        /// <summary>
        /// Creates the type of the link to person.
        /// </summary>
        /// <param name="missionSetId">The mission id.</param>
        /// <param name="personQualityId">The person quality id.</param>
        /// <param name="score">The score.</param>
        /// <returns>MissionDraftAzure.</returns>
        public static MissionSetAzure CreateLinkToPersonQuality(
            string missionSetId,
            string personQualityId,
            double score)
        {
            return new MissionSetAzure
            {
                PartitionKey = missionSetId,
                RowKey =
                    string.Concat(
                        AzureTableConstants.PersonQualityLinkRowKeyPrefix,
                        CommonConstants.StringsDelimiter,
                        personQualityId),
                PersonQualityScore = score
            };
        }

        #endregion

        private string[] SplittedRowKey
        {
            get
            {
                return RowKey.SplitStringByDelimiter().ToArray();
            }
        }
    }
}