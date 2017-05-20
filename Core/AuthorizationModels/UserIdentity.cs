namespace Core.AuthorizationModels
{
    using Core.Enums;

    /// <summary>
    /// Represents user identity model
    /// </summary>
    public class UserIdentity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the login/email.
        /// </summary>
        /// <value>
        /// The login email.
        /// </value>
        public string LoginEmail { get; set; }

        /// <summary>
        /// Gets or sets the hashed password.
        /// </summary>
        /// <value>
        /// The hashed password.
        /// </value>
        public string HashedPassword { get; set; }

        /// <summary>
        /// Gets or sets the type of the hash.
        /// </summary>
        /// <value>
        /// The type of the hash.
        /// </value>
        public HashType HashType { get; set; }


        /// <summary>
        /// Gets or sets the email confirmation code.
        /// </summary>
        /// <value>
        /// The email confirmation code.
        /// </value>
        public string EmailConfirmationCode { get; set; }
        

        /// <summary>
        /// Gets or sets the reset password code.
        /// </summary>
        /// <value>
        /// The reset password code.
        /// </value>
        public byte EmailConfirmationAttempts { get; set; }

        /// <summary>
        /// Gets or sets the vk identity.
        /// </summary>
        /// <value>
        /// The vk identity.
        /// </value>
        public VkIdentity VkIdentity { get; set; }

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        /// <value>
        /// The device used.
        /// </value>
        public string Device { get; set; }
    }
}
