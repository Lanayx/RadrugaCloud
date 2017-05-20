namespace Core.NonDomainModels
{
    using System.Collections.Generic;
    using System.Device.Location;

    using Core.DomainModels;
    using Core.Enums;

    /// <summary>
    /// Class UserMission
    /// </summary>
    public class UserMission
    {
        // Common

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the photo url.
        /// </summary>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the difficulty.
        /// </summary>
        public byte Difficulty { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public ExecutionType ExecutionType { get; set; }

        /// <summary>
        /// Gets or sets the display status.
        /// </summary>
        /// <value>
        /// The display status.
        /// </value>
        public MissionDisplayStatus DisplayStatus { get; set; }
        
        /// <summary>
        /// Gets or sets the ids of the missions which depend on the current
        /// </summary>
        /// <value>
        /// The dependent mission.
        /// </value>
        public List<string> DependentMissionIds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is final.
        /// </summary>
        /// <value><c>true</c> if this instance is final; otherwise, <c>false</c>.</value>
        public bool IsFinal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [message after completion].
        /// </summary>
        /// <value><c>true</c> if [message after completion]; otherwise, <c>false</c>.</value>
        public string MessageAfterCompletion { get; set; }

        /// <summary>
        /// Gets or sets the mission set id.
        /// </summary>
        /// <value>The mission set id.</value>
        public string MissionSetId { get; set; }

        // Right answer type
        
        /// <summary>
        /// Gets or sets the answers count.
        /// </summary>
        /// <value>The answers count.</value>
        public byte? AnswersCount { get; set; }

        /// <summary>
        /// Gets or sets the answers count.
        /// </summary>
        /// <value>The answers count.</value>
        public int? TryCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [exact answer].
        /// </summary>
        /// <value><c>true</c> if [exact answer]; otherwise, <c>false</c>.</value>
        public bool? ExactAnswer { get; set; }

        /// <summary>
        /// Gets or sets the correct answers.
        /// </summary>
        public string CorrectAnswers { get; set; }

        /// <summary>
        /// Gets or sets the number of tries for 3 stars.
        /// </summary>
        /// <value>The tries for3 stars.</value>
        public byte? TriesFor3Stars { get; set; }

        /// <summary>
        /// Gets or sets the number of tries for 2 stars.
        /// </summary>
        /// <value>The tries for2 stars.</value>
        public byte? TriesFor2Stars { get; set; }

        /// <summary>
        /// Gets or sets the number of tries for 1 star.
        /// </summary>
        /// <value>The tries for1 star.</value>
        public byte? TriesFor1Star { get; set; }

        // Text Creation

        /// <summary>
        /// Gets or sets the min chars count.
        /// </summary>
        /// <value>The min chars count.</value>
        public int? MinCharsCount { get; set; }

        // Photo Creation

        /// <summary>
        /// Gets or sets the number of photos.
        /// </summary>
        /// <value>The number of photos.</value>
        public byte? NumberOfPhotos { get; set; }

        // GeoCoordinates + GeoPath

        /// <summary>
        /// Gets or sets the accuracy radius.
        /// </summary>
        /// <value>The accuracy radius.</value>
        public int? AccuracyRadius { get; set; }

        /// <summary>
        /// Gets or sets the user coordinates calculation function.
        /// </summary>
        /// <value>The user coordinates calculation function.</value>
        public string CoordinatesCalculationFunction { get; set; }

        /// <summary>
        /// Gets or sets the user coordinates calculation function.
        /// </summary>
        /// <value>The user coordinates calculation function.</value>
        public IEnumerable<GeoCoordinate> CoordinatesCalculationFunctionParameters { get; set; }

        /// <summary>
        /// Gets or sets the calculation function parameters.
        /// </summary>
        /// <value>The calculation function parameters.</value>
        public IEnumerable<string> CalculationFunctionParameters { get; set; }

        /// <summary>
        /// Gets or sets the list of hints for this mission.
        /// </summary>
        /// <value>The list of hints for this mission.</value>
        public IEnumerable<UserMissionHint> Hints { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds for 3 stars.
        /// </summary>
        /// <value>The tries for3 stars.</value>
        public int? SecondsFor3Stars { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds for 2 stars.
        /// </summary>
        /// <value>The tries for2 stars.</value>
        public int? SecondsFor2Stars { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds for 1 star.
        /// </summary>
        /// <value>The tries for1 star.</value>
        public int? SecondsFor1Star { get; set; }

        /// <summary>
        /// Gets or sets the common point alias.
        /// </summary>
        /// <value>The common point alias.</value>
        public string CommonPlaceAlias { get; set; }
    }
}