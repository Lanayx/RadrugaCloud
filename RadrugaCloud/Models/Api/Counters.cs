namespace RadrugaCloud.Models.Api
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Class Counters
    /// </summary>
    public class Counters
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the audios.
        /// </summary>
        /// <value>The audios.</value>
        [DataMember(Name = "audios")]
        public uint Audios { get; set; }

        /// <summary>
        /// Gets or sets the followers.
        /// </summary>
        /// <value>The followers.</value>
        [DataMember(Name = "followers")]
        public uint Followers { get; set; }

        /// <summary>
        /// Gets or sets the friends.
        /// </summary>
        /// <value>The friends.</value>
        [DataMember(Name = "friends")]
        public uint Friends { get; set; }

        /// <summary>
        /// Gets or sets the photos.
        /// </summary>
        /// <value>The photos.</value>
        [DataMember(Name = "photos")]
        public uint Photos { get; set; }

        /// <summary>
        /// Gets or sets the videos.
        /// </summary>
        /// <value>The videos.</value>
        [DataMember(Name = "videos")]
        public uint Videos { get; set; }

        #endregion
    }
}