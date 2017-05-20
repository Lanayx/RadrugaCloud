namespace Core.DomainModels
{
    /// <summary>
    /// Class VkCounters
    /// </summary>
    public class VkCounters
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the audios.
        /// </summary>
        /// <value>The audios.</value>
        public uint? Audios { get; set; }

        /// <summary>
        /// Gets or sets the followers.
        /// </summary>
        /// <value>The followers.</value>
        public uint? Followers { get; set; }

        /// <summary>
        /// Gets or sets the friends.
        /// </summary>
        /// <value>The friends.</value>
        public uint? Friends { get; set; }

        /// <summary>
        /// Gets or sets the photos.
        /// </summary>
        /// <value>The photos.</value>
        public uint? Photos { get; set; }

        /// <summary>
        /// Gets or sets the videos.
        /// </summary>
        /// <value>The videos.</value>
        public uint? Videos { get; set; }

        #endregion
    }
}