namespace Infrastructure.AzureTablesObjects.TableEntities
{
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    ///     Class UserIdentityAzure
    /// </summary>
    public class UserIdentityAzure : TableEntity
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserIdentityAzure" /> class.
        /// </summary>
        public UserIdentityAzure()
        {
            RowKey = AzureTableConstants.UserIdentityRowKey;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the type of the hash.
        /// </summary>
        /// <value>The type of the hash.</value>
        public int HashType { get; set; }

        /// <summary>
        ///     Gets or sets the hashed password.
        /// </summary>
        /// <value>The hashed password.</value>
        public string HashedPassword { get; set; }

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
        public int EmailConfirmationAttempts { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id
        {
            get
            {
                return PartitionKey;
            }

            set
            {
                PartitionKey = value;
            }
        }

        /// <summary>
        ///     Gets or sets the login email.
        /// </summary>
        /// <value>The login email.</value>
        public string LoginEmail { get; set; }


        /// <summary>
        ///     Gets or sets the VK_ id.
        /// </summary>
        /// <value>The VK_ id.</value>
        public int Vk_Id { get; set; }

        /// <summary>
        ///     Gets or sets the VK_ city id.
        /// </summary>
        /// <value>The VK_ city id.</value>
        public int? Vk_CityId { get; set; }

        /// <summary>
        ///     Gets or sets the VK_ counters_ audios.
        /// </summary>
        /// <value>The VK_ counters_ audios.</value>
        public int? Vk_Counters_Audios { get; set; }

        /// <summary>
        ///     Gets or sets the VK_ counters_ followers.
        /// </summary>
        /// <value>The VK_ counters_ followers.</value>
        public int? Vk_Counters_Followers { get; set; }

        /// <summary>
        ///     Gets or sets the VK_ counters_ friends.
        /// </summary>
        /// <value>The VK_ counters_ friends.</value>
        public int? Vk_Counters_Friends { get; set; }

        /// <summary>
        ///     Gets or sets the VK_ counters_ photos.
        /// </summary>
        /// <value>The VK_ counters_ photos.</value>
        public int? Vk_Counters_Photos { get; set; }

        /// <summary>
        ///     Gets or sets the VK_ counters_ videos.
        /// </summary>
        /// <value>The VK_ counters_ videos.</value>
        public int? Vk_Counters_Videos { get; set; }

        /// <summary>
        ///     Gets or sets the VK_ country id.
        /// </summary>
        /// <value>The VK_ country id.</value>
        public int? Vk_CountryId { get; set; }

        /// <summary>
        ///     Gets or sets the VK_ university id.
        /// </summary>
        /// <value>The VK_ university id.</value>
        public int? Vk_UniversityId { get; set; }

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        /// <value>
        /// The device used.
        /// </value>
        public string Device { get; set; }

        #endregion
    }
}