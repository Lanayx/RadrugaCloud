namespace Core.DomainModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The person quality.
    /// </summary>
    public class PersonQuality
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Required]
        [DisplayName("Название")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DisplayName("Описание")]
        public string Description { get; set; }

        #endregion
    }
}