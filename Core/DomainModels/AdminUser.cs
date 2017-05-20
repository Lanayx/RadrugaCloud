namespace Core.DomainModels
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The admin user.
    /// </summary>
    public class AdminUser
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the surname.
        /// </summary>
        [MaxLength(50)]
        public string Surname { get; set; }

        #endregion
    }
}