namespace RadrugaCloud.Models.Api
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Class ResetPasswordModel
    /// </summary>
    public class ResetPasswordModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [DataMember(Name = "email")]
        public string Email { get; set; }
   

        /// <summary>
        /// Gets or sets the approval code.
        /// </summary>
        /// <value>The approval code.</value>
        [DataMember(Name = "approvalcode")]
        public string ApprovalCode { get; set; }

        #endregion
    }
}