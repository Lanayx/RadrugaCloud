namespace RadrugaCloud.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Core.DomainModels;
    using Core.NonDomainModels;

    /// <summary>
    /// Class MissionDraftUi
    /// </summary>
    public class MissionDraftUi : MissionUi
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the add date.
        /// </summary>
        /// <value>The add date.</value>
        [DisplayName("Дата создания")]
        public DateTime AddDate { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        [DisplayName("Автор")]
        [MaxLength(50)]
        public string Author { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Converts to domain.
        /// </summary>
        /// <returns>
        /// MissionDraft.
        /// </returns>
        internal new MissionDraft ConvertToDomain()
        {
            var draft = new MissionDraft
                       {
                           AddDate = AddDate, 
                           AgeFrom = AgeFrom, 
                           AgeTo = AgeTo, 
                           Author = Author, 
                           CorrectAnswers = CorrectAnswers, 
                           Description = Description, 
                           Difficulty = Difficulty, 
                           Id = Id, 
                           Name = Name, 
                           PhotoUrl = PhotoUrl
                       };

            if (PersonQualitiesWithScores != null)
            {
                draft.PersonQualities =
                    PersonQualitiesWithScores.Where(ptws => ptws.PersonQualityId != null)
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

            return draft;
        }

        #endregion
    }
}