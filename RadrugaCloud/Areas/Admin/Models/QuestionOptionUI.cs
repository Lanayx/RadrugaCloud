namespace RadrugaCloud.Areas.Admin.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Core.DomainModels;
    using Core.NonDomainModels;

    public class QuestionOptionUI
    {
        [Display(Name = "Текст варианта")]
        public string Text { get; set; }

        [Display(Name = "Номер варианта")]
        public byte Number { get; set; }

        [Display(Name = "Id следующего вопроса")]
        public string NextQuestionId { get; set; }

        [Display(Name = "Качества с их весом")]
        public List<PersonQualityIdWithScore> PersonQualitiesWithScores { get; set; }
    }
}