namespace RadrugaCloud.Models.Api
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    using Core.Enums;

    /// <summary>
    /// Class RegisterModel
    /// </summary>
    public class RegisterModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [DataMember(Name = "password")]
        [Required]
        [StringLength(50,MinimumLength = 6)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        [DataMember(Name = "loginemail")]
        [EmailAddress]
        [Required]
        [MaxLength(50)]
        public string LoginEmail { get; set; }


        [DataMember(Name = "device")]
        [MaxLength(80)]
        public string Device { get; set; }

        #endregion
    }
}