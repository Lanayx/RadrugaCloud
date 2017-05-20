namespace Infrastructure.AzureTablesObjects.TableEntities
{
    using System;
    using System.Linq;

    using Core.Constants;
    using Core.DomainModels;
    using Core.Enums;
    using Core.NonDomainModels;
    using Core.Tools;

    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    ///     The mission draft azure.
    /// </summary>
    public class MissionAzure : TableEntity
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionAzure" /> class.
        /// </summary>
        public MissionAzure()
        {
            RowKey = AzureTableConstants.MissionRowKey;
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
        ///     Gets or sets the photo url.
        /// </summary>
        public string PhotoUrl { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the difficulty.
        /// </summary>
        public int Difficulty { get; set; } 

        /// <summary>
        ///     Gets or sets the age from.
        /// </summary>
        public int? AgeFrom { get; set; } 

        /// <summary>
        ///     Gets or sets the age to.
        /// </summary>
        public int? AgeTo { get; set; } 

        /// <summary>
        ///     Gets or sets the type of the mission.
        /// </summary>
        /// <value>
        ///     The type of the mission.
        /// </value>
        public int ExecutionType { get; set; } 

        /// <summary>
        ///     Gets or sets the mission ids, current depends on. Optional
        /// </summary>
        /// <value>The depends on.</value>
        public string DependsOn { get; set; } 

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is final.
        /// </summary>
        /// <value><c>true</c> if this instance is final; otherwise, <c>false</c>.</value>
        public bool IsFinal { get; set; }

        /// <summary>
        ///     Gets or sets the message after completion.
        /// </summary>
        /// <value>The message after completion.</value>
        public string MessageAfterCompletion { get; set; }

        /// <summary>
        ///     Gets or sets the mission set id.
        /// </summary>
        /// <value>The mission set id.</value>
        public string MissionSetId { get; set; }

        // Right answer type

        /// <summary>
        ///     Gets or sets the answers count.
        /// </summary>
        /// <value>The answers count.</value>
        public int? AnswersCount { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [exact answer].
        /// </summary>
        /// <value><c>true</c> if [exact answer]; otherwise, <c>false</c>.</value>
        public bool? ExactAnswer { get; set; }

        /// <summary>
        ///     Gets or sets the correct answers.
        /// </summary>
        public string CorrectAnswersString { get; set; } 

        /// <summary>
        ///     Gets or sets the number of tries for 3 stars.
        /// </summary>
        /// <value>The tries for3 stars.</value>
        public int? TriesFor3Stars { get; set; } 

        /// <summary>
        ///     Gets or sets the number of tries for 2 stars.
        /// </summary>
        /// <value>The tries for2 stars.</value>
        public int? TriesFor2Stars { get; set; }

        /// <summary>
        ///     Gets or sets the number of tries for 1 star.
        /// </summary>
        /// <value>The tries for1 star.</value>
        public int? TriesFor1Star { get; set; }

        // Text Creation

        /// <summary>
        ///     Gets or sets the min chars count.
        /// </summary>
        /// <value>The min chars count.</value>
        public int? MinCharsCount { get; set; }

        // Photo Creation

        /// <summary>
        ///     Gets or sets the number of photos.
        /// </summary>
        /// <value>The number of photos.</value>
        public int? NumberOfPhotos { get; set; }

        // GeoCoordinates + GeoPath

        /// <summary>
        ///     Gets or sets the accuracy radius.
        /// </summary>
        /// <value>The accuracy radius.</value>
        public int? AccuracyRadius { get; set; }

        /// <summary>
        ///     Gets or sets the user coordinates calculation function.
        /// </summary>
        /// <value>The user coordinates calculation function.</value>
        public string UserCoordinatesCalculationFunction { get; set; }

        /// <summary>
        ///     Gets or sets the calculation function parameters.
        /// </summary>
        /// <value>The calculation function parameters.</value>
        public string CalculationFunctionParameters { get; set; }

        /// <summary>
        ///     Gets or sets the number of seconds for 3 stars.
        /// </summary>
        /// <value>The tries for3 stars.</value>
        public int? SecondsFor3Stars { get; set; } 

        /// <summary>
        ///     Gets or sets the number of seconds for 2 stars.
        /// </summary>
        /// <value>The tries for2 stars.</value>
        public int? SecondsFor2Stars { get; set; } 

        /// <summary>
        ///     Gets or sets the number of seconds for 1 star.
        /// </summary>
        /// <value>The tries for1 star.</value>
        public int? SecondsFor1Star { get; set; }

        /// <summary>
        ///     Gets or sets the common place alias.
        /// </summary>
        /// <value>The common place alias.</value>
       public string CommonPlaceAlias { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is mission entity.
        /// </summary>
        /// <value><c>true</c> if this instance is mission entity; otherwise, <c>false</c>.</value>
        public bool IsMissionEntity
        {
            get
            {
                return string.Equals(RowKey, AzureTableConstants.MissionRowKey);
            }
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
                return !string.Equals(RowKey, AzureTableConstants.MissionRowKey) && SplittedRowKey.Count() == 2
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
        ///     Creates the type of the link to person.
        /// </summary>
        /// <param name="missionId">The mission id.</param>
        /// <param name="personQualityId">The person quality id.</param>
        /// <param name="score">The score.</param>
        /// <returns>MissionDraftAzure.</returns>
        public static MissionAzure CreateLinkToPersonQuality(string missionId, string personQualityId, double score)
        {
            return new MissionAzure
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

        #region Hint Section

        /// <summary>
        ///     Gets a value indicating whether this instance is hint link.
        /// </summary>
        /// <value><c>true</c> if this instance is hint link; otherwise, <c>false</c>.</value>
        public bool IsHintLink => !string.Equals(RowKey, AzureTableConstants.MissionRowKey) && SplittedRowKey.Count() == 2
                                  && string.Equals(SplittedRowKey[0], AzureTableConstants.HintLinkRowKeyPrefix);

        /// <summary>
        ///     Gets the hint id.
        /// </summary>
        /// <value>The hint id.</value>
        public string HintId => IsHintLink ? SplittedRowKey[1] : string.Empty;

        /// <summary>
        ///     Gets or sets the hint score.
        /// </summary>
        /// <value>The hint score.</value>
        public int? HintScore { get; set; }

        /// <summary>
        ///     Gets or sets the hint text.
        /// </summary>
        /// <value>The hint text.</value>
        public string HintText { get; set; }        

        /// <summary>
        ///     Gets or sets the hint type.
        /// </summary>
        /// <value>The hint type coordinate.</value>
        public int? HintType { get; set; }

        /// <summary>
        ///     Creates the type of the link to hint.
        /// </summary>
        /// <param name="missionId">The mission id.</param>
        /// <param name="hint">The hint.</param>        
        /// <returns>MissionAzure</returns>
        public static MissionAzure CreateLinkToHint(string missionId, Hint hint)
        {
            var hintId = String.IsNullOrEmpty(hint.Id) 
                ? Guid.NewGuid().ToString("N")
                : hint.Id;
            return new MissionAzure
            {
                PartitionKey = missionId,
                RowKey = string.Concat(
                                   AzureTableConstants.HintLinkRowKeyPrefix,
                                   CommonConstants.StringsDelimiter,
                                   hintId),
                HintScore = hint.Score,
                HintText = hint.Type == Core.Enums.HintType.Text ? hint.Text : string.Empty,                
                HintType = (int)hint.Type
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