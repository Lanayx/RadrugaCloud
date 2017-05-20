namespace Core.DomainModels
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Core.NonDomainModels;

    /// <summary>
    /// Class MissionSet
    /// </summary>
    public class MissionSet
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DisplayName("Название")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the age from.
        /// </summary>
        [DisplayName("Возраст От")]
        public byte? AgeFrom { get; set; }

        /// <summary>
        /// Gets or sets the age to.
        /// </summary>
        [DisplayName("Возраст До")]
        public byte? AgeTo { get; set; }


        /// <summary>
        /// Gets or sets the missions.
        /// </summary>
        /// <value>The missions.</value>
        [DisplayName("Качества человека")]
        public List<MissionWithOrder> Missions { get; set; }

        /// <summary>
        /// Gets or sets the person qualities.
        /// </summary>
        /// <value>
        /// The person qualities.
        /// </value>
        [DisplayName("Качества человека")]
        public List<PersonQualityIdWithScore> PersonQualities { get; set; }
        #endregion
    }
}