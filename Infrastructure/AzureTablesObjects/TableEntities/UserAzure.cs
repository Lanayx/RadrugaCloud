namespace Infrastructure.AzureTablesObjects.TableEntities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Constants;
    using Core.DomainModels;
    using Core.NonDomainModels;
    using Core.Tools;

    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    ///     The user azure.
    /// </summary>
    public class UserAzure : TableEntity
    {
        private DateTime? _bornDate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserAzure" /> class.
        /// </summary>
        public UserAzure()
        {
            RowKey = AzureTableConstants.UserRowKey;
        }

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
        ///     Gets or sets the current mission id.
        /// </summary>
        /// <value>The current mission id.</value>
        public string CurrentMissionId { get; set; }

        /// <summary>
        ///     Gets or sets the city name
        /// </summary>
        /// <value>The unique city id.</value>
        public string CityShortName { get; set; }

        /// <summary>
        ///     Gets or sets the country code
        /// </summary>
        /// <value>The unique city id.</value>
        public string CountryShortName { get; set; }

        /// <summary>
        ///     Gets or sets the completed mission ids string.
        /// </summary>
        /// <value>The mission ids string.</value>
        public string CompletedMissionIdsString { get; set; }

        /// <summary>
        ///     Gets or sets the incompleted mission ids string.
        /// </summary>
        /// <value>
        ///     The incompleted mission ids string.
        /// </value>
        public string FailedMissionIdsString { get; set; }

        /// <summary>
        ///     Gets or sets the in progress mission ids string.
        /// </summary>
        /// <value>
        ///     The in progress mission ids string.
        /// </value>
        public string ActiveMissionIdsString { get; set; }

        /// <summary>
        ///     Gets or sets the mission set ids with order string.
        /// </summary>
        /// <value>
        ///     The mission set ids string.
        /// </value>
        public string MissionSetIdsString { get; set; }

        /// <summary>
        ///     Gets or sets the active mission set ids.
        /// </summary>
        /// <value>
        ///     The active mission set ids.
        /// </value>
        public string ActiveMissionSetIdsString { get; set; }

        /// <summary>
        ///     Gets or sets the bought hint ids.
        /// </summary>
        /// <value>
        ///     The bought hint ids.
        /// </value>
        public string BoughtHintIdsString { get; set; }
        
        /// <summary>
        ///     Gets or sets the completed mission set ids string.
        /// </summary>
        /// <value>
        ///     The completed mission set ids string.
        /// </value>
        public string CompletedMissionSetIdsString { get; set; }

        /// <summary>
        ///     Gets the person quality id.
        /// </summary>
        /// <value>The person quality id.</value>
        public string PersonQualityId => IsPersonQualityLink ? SplittedRowKey[1] : string.Empty;

        /// <summary>
        ///     Gets or sets the avatar URL.
        /// </summary>
        /// <value>The avatar URL.</value>
        public string AvatarUrl { get; set; }

        /// <summary>
        ///     Gets or sets the base east coordinate.
        /// </summary>
        /// <value>The base east coordinate.</value>
        public string BaseEastCoordinate { get; set; }

        /// <summary>
        ///     Gets or sets the base north coordinate.
        /// </summary>
        /// <value>The base north coordinate.</value>
        public string BaseNorthCoordinate { get; set; }

        /// <summary>
        ///     Gets or sets the base south coordinate.
        /// </summary>
        /// <value>The base south coordinate.</value>
        public string BaseSouthCoordinate { get; set; }

        /// <summary>
        ///     Gets or sets the base west coordinate.
        /// </summary>
        /// <value>The base west coordinate.</value>
        public string BaseWestCoordinate { get; set; }

        /// <summary>
        ///     Gets or sets the set passed count.
        /// </summary>
        /// <value>
        ///     The set passed count.
        /// </value>
        public int? CompletedSetsCount { get; set; }

        /// <summary>
        ///     Gets or sets the date of birth.
        /// </summary>
        /// <value>The date of birth.</value>
        public DateTime? DateOfBirth
        {
            get
            {
                if (_bornDate < AzureTableConstants.MinAzureUtcDate)
                {
                    _bornDate = null;
                }

                return _bornDate;
            }

            set
            {
                _bornDate = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [enable push notifications].
        /// </summary>
        /// <value><c>true</c> if [enable push notifications]; otherwise, <c>false</c>.</value>
        public bool? EnablePushNotifications { get; set; }

        /// <summary>
        ///     Gets or sets the five stars missions spree.
        /// </summary>
        /// <value>
        ///     The five stars missions spree.
        /// </value>
        public int? ThreeStarsMissionSpreeCurrentCount { get; set; }

        /// <summary>
        ///     Gets or sets the five stars missions spree maximum count.
        /// </summary>
        /// <value>
        ///     The five stars missions spree maximum count.
        /// </value>
        public int? ThreeStarsMissionSpreeMaxCount { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance has high education.
        /// </summary>
        /// <value><c>true</c> if this instance has high education; otherwise, <c>false</c>.</value>
        public bool? HasHighEducation { get; set; }

        /// <summary>
        ///     Gets or sets the home coordinate.
        /// </summary>
        /// <value>The home coordinate.</value>
        public string HomeCoordinate { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is person quality link.
        /// </summary>
        /// <value><c>true</c> if this instance is person quality link; otherwise, <c>false</c>.</value>
        public bool IsPersonQualityLink => !string.Equals(RowKey, AzureTableConstants.UserRowKey) && SplittedRowKey.Count == 2
                                           && string.Equals(SplittedRowKey[0], AzureTableConstants.PersonQualityLinkRowKeyPrefix);

        /// <summary>
        ///     Gets a value indicating whether this instance is user entity.
        /// </summary>
        /// <value><c>true</c> if this instance is user entity; otherwise, <c>false</c>.</value>
        public bool IsUserEntity => string.Equals(RowKey, AzureTableConstants.UserRowKey);

        /// <summary>
        ///     Gets or sets the kind scale.
        /// </summary>
        /// <value>The kind scale.</value>
        public int? KindScale { get; set; }

        /// <summary>
        ///     Gets or sets the kind scale high days.
        /// </summary>
        /// <value>
        ///     The kind scale high days.
        /// </value>
        public int? KindScaleHighCurrentDays { get; set; }

        /// <summary>
        ///     Gets or sets the kind scale high maximum days.
        /// </summary>
        /// <value>
        ///     The kind scale high maximum days.
        /// </value>
        public int? KindScaleHighMaxDays { get; set; }

        /// <summary>
        ///     Gets or sets the last rating place.
        /// </summary>
        /// <value>
        ///     The last rating place.
        /// </value>
        public int? LastRatingPlace { get; set; }

        /// <summary>
        ///     Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        public int? Level { get; set; }

        /// <summary>
        ///     Gets or sets the level percentage.
        /// </summary>
        public int? LevelPoints { get; set; }

        /// <summary>
        ///     Gets or sets the name of the nick.
        /// </summary>
        /// <value>The name of the nick.</value>
        public string NickName { get; set; }

        /// <summary>
        ///     Gets or sets outpost coordinate.
        /// </summary>
        /// <value>The outpost coordinate.</value>
        public string OutpostCoordinate { get; set; }

        /// <summary>
        ///     Gets or sets the name of the person quality.
        /// </summary>
        /// <value>The name of the person quality.</value>
        public string PersonQualityName { get; set; }

        /// <summary>
        ///     Gets or sets the person quality score.
        /// </summary>
        /// <value>The person quality score.</value>
        public double? PersonQualityScore { get; set; }

        /*public IEnumerable<PersonQualityWithScore> PersonQualitiesWithScores { get; set; }*/

        /// <summary>
        ///     Gets or sets the rating.
        /// </summary>
        /// <value>The rating.</value>
        public int? Points { get; set; }

        /// <summary>
        ///     Gets or sets radar coordinate.
        /// </summary>
        /// <value>The radar coordinate.</value>
        public string RadarCoordinate { get; set; }

        /// <summary>
        ///     Gets or sets RGB radruga color
        /// </summary>
        /// <value>
        ///     The color of the radruga.
        /// </value>
        public string RadrugaColor { get; set; }

        /// <summary>
        ///     Gets or sets the sex.
        /// </summary>
        /// <value>The sex.</value>
        public int? Sex { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [show age].
        /// </summary>
        /// <value><c>true</c> if [show age]; otherwise, <c>false</c>.</value>
        public bool? ShowAge { get; set; }

        /// <summary>
        ///     Gets or sets up in rating current days.
        /// </summary>
        /// <value>
        ///     Up in rating current days.
        /// </value>
        public int? UpInRatingCurrentDays { get; set; }

        /// <summary>
        ///     Gets or sets up in rating maximum days.
        /// </summary>
        /// <value>
        ///     Up in rating maximum days.
        /// </value>
        public int? UpInRatingMaxDays { get; set; }

        /// <summary>
        ///     Gets or sets the vk reposts count.
        /// </summary>
        /// <value>
        ///     The vk reposts count.
        /// </value>
        public int? VkRepostCount { get; set; }

        /// <summary>
        /// Gets or sets the kind actions count.
        /// </summary>
        /// <value>
        /// The kind actions count.
        /// </value>
        public int? KindActionsCount { get; set; }

        /// <summary>
        /// Gets or sets the kind action marks count.
        /// </summary>
        /// <value>
        /// The kin action marks count.
        /// </value>
        public int? KindActionMarksCount { get; set; }

        /// <summary>
        /// Gets or sets the coins count.
        /// </summary>
        /// <value>
        /// The coins count.
        /// </value>
        public int? CoinsCount { get; set; }

        private List<string> SplittedRowKey => RowKey.SplitStringByDelimiter().ToList();

        /// <summary>
        ///     Creates the type of the link to person.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="personQualityId">The person quality id.</param>
        /// <param name="score">The score.</param>
        /// <returns>UserAzure.</returns>
        public static UserAzure CreateLinkToPersonQuality(string userId, string personQualityId, double score)
        {
            return new UserAzure
                       {
                           PartitionKey = userId,
                           RowKey =
                               string.Concat(
                                   AzureTableConstants.PersonQualityLinkRowKeyPrefix,
                                   CommonConstants.StringsDelimiter,
                                   personQualityId),
                           PersonQualityScore = score
                       };
        }

        public List<MissionIdWithSetId> GetActiveMissions()
        {
            if (string.IsNullOrEmpty(ActiveMissionIdsString))
            {
                return new List<MissionIdWithSetId>();
            }
            var inputString = ActiveMissionIdsString.Trim(CommonConstants.StringsDelimiterChar);
            return inputString.Split(CommonConstants.StringsDelimiterChar).Select(
                item =>
                    {
                        var idAndOrder = item.Split(CommonConstants.CommaDelimiterChar);
                        return new MissionIdWithSetId { MissionSetId = idAndOrder[0], MissionId = idAndOrder[1] };
                    }).ToList();
        }

        /// <summary>
        ///     Splits the string by semicolon.
        /// </summary>
        /// <returns>List{System.String}.</returns>
        public List<MissionSetIdWithOrder> GetMissionSets()
        {
            if (string.IsNullOrEmpty(MissionSetIdsString))
            {
                return new List<MissionSetIdWithOrder>();
            }
            var inputString = MissionSetIdsString.Trim(CommonConstants.StringsDelimiterChar);
            return inputString.Split(CommonConstants.StringsDelimiterChar).Select(
                item =>
                    {
                        var idAndOrder = item.Split(CommonConstants.CommaDelimiterChar);
                        return new MissionSetIdWithOrder
                                   {
                                       MissionSetId = idAndOrder[0],
                                       Order = int.Parse(idAndOrder[1])
                                   };
                    }).ToList();
        }

        public void SetActiveMissions(List<MissionIdWithSetId> initialCollection)
        {
            if (initialCollection == null)
            {
                ActiveMissionIdsString = null;
                return;
            }
         
            if (!initialCollection.Any())
            {
                ActiveMissionIdsString = string.Empty;
                return;
            }

            var stringList =
                initialCollection.Where(e => e != null)
                    .Select(e => e.MissionSetId + CommonConstants.CommaDelimiter + e.MissionId)
                    // additional delimiter between setid and missionId
                    .Where(s => !string.IsNullOrEmpty(s));

            ActiveMissionIdsString = string.Join(CommonConstants.StringsDelimiter, stringList);
        }

        /// <summary>
        ///     Joins to string semicolon separated.
        /// </summary>
        /// <param name="initialCollection">The initial collection.</param>
        /// <returns>System.String.</returns>
        public void SetMissionSets(List<MissionSetIdWithOrder> initialCollection)
        {
            if (initialCollection == null)
            {
                MissionSetIdsString = null;
                return;
            }
            
            if (!initialCollection.Any())
            {
                MissionSetIdsString = string.Empty;
                return;
            }

            var stringList =
                initialCollection.Where(e => e != null)
                    .Select(e => e.MissionSetId + CommonConstants.CommaDelimiter + e.Order)
                    // additional delimiter between id and order
                    .Where(s => !string.IsNullOrEmpty(s));

            MissionSetIdsString = string.Join(CommonConstants.StringsDelimiter, stringList);
        }       
    }
}