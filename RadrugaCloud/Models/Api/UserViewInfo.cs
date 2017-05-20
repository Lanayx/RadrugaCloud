using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadrugaCloud.Models.Api
{
    /// <summary>
    /// Read only info about another user
    /// </summary>
    public class UserViewInfo
    {
        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Gets or sets Completed missions ids
        /// </summary>
        public int CompletedMissionIdsCount { get; set; }

        /// <summary>
        ///     Gets or sets Completed missions ids count
        /// </summary>
        public int FailedMissionIdsCount { get; set; }

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
        ///     Gets or sets the kind scale.
        /// </summary>
        public byte? KindScale { get; set; }

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
        public string NickName { get; set; }

        /// <summary>
        ///     Gets or sets user total points.
        /// </summary>
        public int? Points { get; set; }

        /// <summary>
        ///     Gets or sets RGB color value
        /// </summary>
        public string RadrugaColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user has [three five stars badge].
        /// </summary>
        /// <value>
        /// <c>true</c> if [three five stars badge]; otherwise, <c>false</c>.
        /// </value>
        public bool ThreeFiveStarsBadge { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether user has [five sets badge].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [five sets badge]; otherwise, <c>false</c>.
        /// </value>
        public bool FiveSetsBadge { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether user has [kind scale badge].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [kind scale badge]; otherwise, <c>false</c>.
        /// </value>
        public bool KindScaleBadge { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether user has [five reposts badge].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [five reposts badge]; otherwise, <c>false</c>.
        /// </value>
        public bool FiveRepostsBadge { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether user has [rating growth badge].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [rating growth badge]; otherwise, <c>false</c>.
        /// </value>
        public bool RatingGrowthBadge { get; set; }

        /// <summary>
        /// Gets or sets the global rank.
        /// </summary>
        /// <value>
        /// The global rank.
        /// </value>
        public long GlobalRank { get; set; }
        /// <summary>
        /// Gets or sets the country rank.
        /// </summary>
        /// <value>
        /// The country rank.
        /// </value>
        public long CountryRank { get; set; }
        /// <summary>
        /// Gets or sets the city rank.
        /// </summary>
        /// <value>
        /// The city rank.
        /// </value>
        public long CityRank { get; set; }

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
    }
}