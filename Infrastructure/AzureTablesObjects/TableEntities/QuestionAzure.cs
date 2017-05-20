namespace Infrastructure.AzureTablesObjects.TableEntities
{
    using System.Linq;

    using Core.Constants;
    using Core.DomainModels;
    using Core.Tools;

    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    ///     Class QuestionAzure
    /// </summary>
    public class QuestionAzure : TableEntity
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="QuestionAzure" /> class.
        /// </summary>
        public QuestionAzure()
        {
            RowKey = AzureTableConstants.QuestionRowKey;
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
        ///     Gets a value indicating whether this instance is option link.
        /// </summary>
        /// <value><c>true</c> if this instance is option link; otherwise, <c>false</c>.</value>
        public bool IsOptionLink
        {
            get
            {
                return !string.Equals(RowKey, AzureTableConstants.QuestionRowKey) && SplittedRowKey.Count() == 2
                       && string.Equals(SplittedRowKey[0], AzureTableConstants.QuestionOptionRowKeyPrefix);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is person quality link.
        /// </summary>
        /// <value><c>true</c> if this instance is person quality link; otherwise, <c>false</c>.</value>
        public bool IsPersonQualityLink
        {
            get
            {
                return !string.Equals(RowKey, AzureTableConstants.QuestionRowKey) && SplittedRowKey.Count() == 3
                       && string.Equals(SplittedRowKey[0], AzureTableConstants.PersonQualityLinkRowKeyPrefix);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is question.
        /// </summary>
        /// <value><c>true</c> if this instance is question; otherwise, <c>false</c>.</value>
        public bool IsQuestion
        {
            get
            {
                return string.Equals(RowKey, AzureTableConstants.QuestionRowKey);
            }
        }

        /// <summary>
        ///     Gets the person quality id.
        /// </summary>
        /// <value>The person quality id.</value>
        public string PersonQualityId
        {
            get
            {
                return IsPersonQualityLink ? SplittedRowKey[2] : string.Empty;
            }
        }

        /// <summary>
        /// Gets the person quality option number.
        /// </summary>
        /// <value>The person quality option number.</value>
        public byte PersonQualityOptionNumber
        {
            get
            {
                var numberString = IsPersonQualityLink ? SplittedRowKey[1] : string.Empty;
                byte number;
                byte.TryParse(numberString, out number);
                return number;
            }
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the option number.
        /// </summary>
        /// <value>The option number.</value>
        public int OptionNumber { get; set; }

        /// <summary>
        ///     Gets or sets the option text.
        /// </summary>
        /// <value>The option text.</value>
        public string OptionText { get; set; }


        /// <summary>
        /// Gets or sets the next question identifier.
        /// </summary>
        /// <value>
        /// The next question identifier.
        /// </value>
        public string NextQuestionId { get; set; }

        /// <summary>
        ///     Gets or sets the name of the person quality.
        /// </summary>
        /// <value>The name of the person quality.</value>
        public string PersonQualityName { get; set; }

        /// <summary>
        ///     Gets or sets the person quality score.
        /// </summary>
        /// <value>The person quality score.</value>
        public double PersonQualityScore { get; set; }

        /// <summary>
        ///     Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        #endregion

        #region Properties

        private string[] SplittedRowKey
        {
            get
            {
                return RowKey.SplitStringByDelimiter().ToArray();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the type of the link to person.
        /// </summary>
        /// <param name="questionId">The question id.</param>
        /// <param name="questionOptionId">The user id.</param>
        /// <param name="personQualityId">The person quality id.</param>
        /// <param name="score">The score.</param>
        /// <returns>UserAzure.</returns>
        public static QuestionAzure CreateLinkToPersonQuality(
            string questionId, 
            byte questionOptionId, 
            string personQualityId, 
            double score)
        {
            return new QuestionAzure
                       {
                           PartitionKey = questionId, 
                           RowKey =
                               string.Concat(
                                   AzureTableConstants.PersonQualityLinkRowKeyPrefix, 
                                   CommonConstants.StringsDelimiter, 
                                   questionOptionId, 
                                   CommonConstants.StringsDelimiter, 
                                   personQualityId),
                           PersonQualityScore = score
                       };
        }

        /// <summary>
        ///     Creates the option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="questionId">The quistion id.</param>
        /// <returns>QuestionAzure.</returns>
        public static QuestionAzure CreateOption(QuestionOption option, string questionId)
        {
            return new QuestionAzure
                       {
                           PartitionKey = questionId, 
                           RowKey =
                               string.Concat(
                                   AzureTableConstants.QuestionOptionRowKeyPrefix, 
                                   CommonConstants.StringsDelimiter, 
                                   option.Number), 
                           OptionText = option.Text, 
                           OptionNumber = option.Number,
                           NextQuestionId = option.NextQuestionId
                       };
        }

        #endregion
    }
}