namespace Core.DomainModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Device.Location;

    using Core.Attributes;
    using Core.Constants;
    using Core.Enums;
    using Core.NonDomainModels;

    /// <summary>
    ///     The user.
    /// </summary>
    public class User
    {
        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        ///     Gets or sets Completed missions ids
        /// </summary>
        public List<string> CompletedMissionIds { get; set; }

        /// <summary>
        ///     Gets or sets Completed missions ids
        /// </summary>
        public List<string> FailedMissionIds { get; set; }

        /// <summary>
        ///     Gets or sets all setline for the user
        /// </summary>
        /// <value>
        ///     The in progress mission ids.
        /// </value>
        public List<MissionSetIdWithOrder> MissionSetIds { get; set; }

        /// <summary>
        ///     Gets or sets the in progress mission ids.
        /// </summary>
        /// <value>
        ///     The in progress mission ids.
        /// </value>
        public List<MissionIdWithSetId> ActiveMissionIds { get; set; }

        /// <summary>
        ///     Gets or sets the in progress set ids.
        /// </summary>
        /// <value>
        ///     The in progress mission ids.
        /// </value>
        public List<string> ActiveMissionSetIds { get; set; }

        /// <summary>
        ///     Gets or sets the in progress mission ids.
        /// </summary>
        /// <value>
        ///     The in progress mission ids.
        /// </value>
        public List<string> CompletedMissionSetIds { get; set; }

        /// <summary>
        ///     Gets or sets the bougth hint ids.
        /// </summary>
        /// <value>
        ///     The bought hint ids.
        /// </value>
        public List<string> BoughtHintIds { get; set; }

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
        ///     Gets or sets the unique city id.
        /// </summary>
        /// <value>The unique city id.</value>
        public string UniqueCityId => String.IsNullOrEmpty(CountryShortName) || String.IsNullOrEmpty(CityShortName)
                                        ? null
                                        : CountryShortName + ":" + CityShortName;

        /// <summary>
        ///     Gets or sets the avatar url.
        /// </summary>
        [Url]
        [DisplayName("Аватар")]
        public string AvatarUrl { get; set; }

        /// <summary>
        ///     Gets or sets the base east coordinate.
        /// </summary>
        /// <value>The base east coordinate.</value>
        public GeoCoordinate BaseEastCoordinate { get; set; }

        /// <summary>
        ///     Gets or sets the base north coordinate.
        /// </summary>
        /// <value>The base north coordinate.</value>
        public GeoCoordinate BaseNorthCoordinate { get; set; }

        /// <summary>
        ///     Gets or sets the base south coordinate.
        /// </summary>
        /// <value>The base south coordinate.</value>
        public GeoCoordinate BaseSouthCoordinate { get; set; }

        /// <summary>
        ///     Gets or sets the base west coordinate.
        /// </summary>
        /// <value>The base west coordinate.</value>
        public GeoCoordinate BaseWestCoordinate { get; set; }
        
        /// <summary>
        ///     Gets or sets the date of birth.
        /// </summary>
        [Required]
        [UserAgeValidation]
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether enable push notifications.
        /// </summary>
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
        ///     Gets or sets the home coordinate.
        /// </summary>
        public GeoCoordinate HomeCoordinate { get; set; }

        /// <summary>
        ///     Gets or sets the kind scale.
        /// </summary>
        [Range(0, CommonConstants.PercentageMax)]
        public byte? KindScale { get; set; }

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
        public byte? Level { get; set; }

        /// <summary>
        ///     Gets or sets the level percentage.
        /// </summary>
        public ushort? LevelPoints { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [Required]
        [MaxLength(50)]
        [DisplayName("Никнейм")]
        public string NickName { get; set; }

        /// <summary>
        ///     Gets or sets the outpost coordinate.
        /// </summary>
        /// <value>
        ///     The outpost coordinate.
        /// </value>
        public GeoCoordinate OutpostCoordinate { get; set; }

        /// <summary>
        ///     Gets or sets List of the person qualities with their scores for current user
        /// </summary>
        public List<PersonQualityIdWithScore> PersonQualitiesWithScores { get; set; }

        /// <summary>
        ///     Gets or sets user total points.
        /// </summary>
        public int? Points { get; set; }

        /// <summary>
        ///     Gets or sets the radar coordinate.
        /// </summary>
        /// <value>
        ///     The radar coordinate.
        /// </value>
        public GeoCoordinate RadarCoordinate { get; set; }

        /// <summary>
        ///     Gets or sets RGB color value
        /// </summary>
        public string RadrugaColor { get; set; }

        /// <summary>
        ///     Gets or sets the selected2 base coordinates. (Like East, West)
        /// </summary>
        /// <value>
        ///     The selected2 base coordinates.
        /// </value>
        public List<string> Selected2BaseCoordinates { get; set; }

        /// <summary>
        ///     Gets or sets the sex.
        /// </summary>
        public Sex? Sex { get; set; }

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
        /// The kind action marks count.
        /// </value>
        public int? KindActionMarksCount { get; set; }

        /// <summary>
        /// Gets or sets the coins count.
        /// </summary>
        /// <value>
        /// The coins count.
        /// </value>
        public int? CoinsCount { get; set; }


        /// <summary>
        /// Checks the important fields.
        /// </summary>
        /// <returns></returns>
        public List<string> CheckImportantFields()
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(NickName))
            {
                list.Add("nickName");
            }

            if (DateOfBirth == null)
            {
                list.Add("bdate");
            }

            if (!Sex.HasValue || Sex == Enums.Sex.NotSet)
            {
                list.Add("sex");
            }

            return list;
        }
    }
}