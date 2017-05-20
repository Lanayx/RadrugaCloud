namespace Infrastructure.AzureTablesObjects.TableEntities
{
    using System;

    using Core.Constants;

    /// <summary>
    ///     The mission draft azure.
    /// </summary>
    public class MissionDraftAzure : MissionAzure
    {
        #region Fields

        private DateTime _addDate;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionDraftAzure" /> class.
        /// </summary>
        public MissionDraftAzure()
        {
            RowKey = AzureTableConstants.DraftRowKey;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the add date.
        /// </summary>
        public DateTime AddDate
        {
            get
            {
                if (_addDate < AzureTableConstants.MinAzureUtcDate)
                {
                    _addDate = DateTime.UtcNow;
                }

                return _addDate;
            }

            set
            {
                _addDate = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is mission draft entity.
        /// </summary>
        /// <value><c>true</c> if this instance is mission draft entity; otherwise, <c>false</c>.</value>
        public bool IsMissionDraftEntity
        {
            get
            {
                return string.Equals(RowKey, AzureTableConstants.DraftRowKey);
            }
        }

        /// <summary>
        ///     Gets or sets the author.
        /// </summary>
        public string Author { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Creates the type of the link to person.
        /// </summary>
        /// <param name="missionId">The mission id.</param>
        /// <param name="personQualityId">The person quality id.</param>
        /// <param name="score">The score.</param>
        /// <returns>MissionDraftAzure.</returns>
        public static new MissionDraftAzure CreateLinkToPersonQuality(
            string missionId, 
            string personQualityId,
            double score)
        {
            return new MissionDraftAzure
                       {
                           PartitionKey = missionId,
                           RowKey =
                               string.Concat(
                                   AzureTableConstants.PersonQualityLinkRowKeyPrefix,
                                   CommonConstants.StringsDelimiter,
                                   personQualityId),
                           PersonQualityScore = score
                       };
        }

        #endregion
    }
}