namespace Core.DomainModels
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// A class for public drafts, which can be converted to real missions
    /// </summary>
    public class MissionDraft : Mission
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the add date.
        /// </summary>
        [DisplayName("Дата создания")]
        public DateTime AddDate { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        [DisplayName("Автор")]
        public string Author { get; set; }

        #endregion
    }
}