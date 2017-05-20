namespace RadrugaCloud.Models.Api
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    using Core.Enums;

    /// <summary>
    /// Class RegisterModel
    /// </summary>
    public class RequiredFields
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>The date of birth.</value>
        [DataMember(Name = "dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the name of the nick.
        /// </summary>
        /// <value>The name of the nick.</value>
        [DataMember(Name = "nickname")]
        [MaxLength(50)]
        public string NickName { get; set; }

        /// <summary>
        /// Gets or sets users sex.
        /// </summary>
        /// <value>Enum value</value>
        [DataMember(Name = "sex")]
        public Sex? Sex { get; set; }
        #endregion
    }
}