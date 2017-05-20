namespace RadrugaCloud.Areas.Admin.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Core.DomainModels;
    using Core.NonDomainModels;

    /// <summary>
    /// Class QuestionUi
    /// </summary>
    public class QuestionUi
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [Required]
        [Display(Name = "Текст вопроса")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the person qualities.
        /// </summary>
        /// <value>The person qualities.</value>
        [DisplayName("Варианты ответов")]
        [Required]
        public List<QuestionOptionUI> QuestionOptions { get; set; }

        #region Methods

        /// <summary>
        /// Converts to domain.
        /// </summary>
        /// <returns>Question.</returns>
        internal Question ConvertToDomain()
        {
            var question = new Question { Id = Id, Name = Name, Text = Text, };

            if (QuestionOptions != null)
            {
                var options = QuestionOptions.Select(
                    t =>
                        {
                            var option = new QuestionOption
                                             {
                                                 Number = t.Number,
                                                 Text = t.Text,
                                                 NextQuestionId = t.NextQuestionId,
                                             };
                            if (t.PersonQualitiesWithScores != null)
                            {
                                option.PersonQualitiesWithScores =
                                    t.PersonQualitiesWithScores.Where(ptws => ptws.PersonQualityId != null)
                                        .GroupBy(ptws => ptws.PersonQualityId)
                                        .Select(grp => grp.First())
                                        .Select(
                                            ptws =>
                                                {
                                                    var personQualityWithScore = new PersonQualityIdWithScore
                                                                                     {
                                                                                         PersonQualityId = ptws.PersonQualityId,
                                                                                         Score = ptws.Score
                                                                                     };
                                                    return personQualityWithScore;
                                                }).ToList();
                            }
                            return option;
                        }).ToList();

                question.Options = options;
            }

            return question;
        }

        #endregion
    }
}