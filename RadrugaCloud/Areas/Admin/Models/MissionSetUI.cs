namespace RadrugaCloud.Areas.Admin.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Core.DomainModels;

    /// <summary>
    /// Class MissionSetUI
    /// </summary>
    public class MissionSetUI
    {
        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Required(ErrorMessage = "Название должно быть указано")]
        [DisplayName("Название*")]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the missions.
        /// </summary>
        /// <value>The missions.</value>
        [DisplayName("Список миссий по порядку")]
        public List<string> Missions { get; set; }

        internal MissionSet ConvertToDomain()
        {
            var missionSet = new MissionSet
            {
                // Common
                Id = Id,
                Name = Name,
            };

            if (Missions != null)
            {
                var order = (byte)0;
                var missionsWithScores =
                    Missions.Distinct().Select(m => new MissionWithOrder { Mission = new Mission(m), Order = order++ }).ToList();

                missionSet.Missions = missionsWithScores;
            }

            return missionSet;
        }
    }
}