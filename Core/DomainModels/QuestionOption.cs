namespace Core.DomainModels
{
    using System.Collections.Generic;

    using Core.NonDomainModels;

    /// <summary>
    /// Options for test
    /// </summary>
    public class QuestionOption
    {
        /// <summary>
        /// Gets or sets Number of the option
        /// </summary>
        public byte Number { get; set; }

        /// <summary>
        /// Gets or sets Option displayed name
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the Id of the next question. Is null if it is the last question in the set.
        /// </summary>
        public string NextQuestionId { get; set; }

        /// <summary>
        /// Gets or setsList of the person qualities with their scores for current mission
        /// </summary>
        public List<PersonQualityIdWithScore> PersonQualitiesWithScores { get; set; } 
    }
}