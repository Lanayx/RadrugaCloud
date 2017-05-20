namespace Core.DomainModels
{
    /// <summary>
    /// Rating item to display on rating page
    /// </summary>
    public class RatingInfo
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the place.
        /// </summary>
        /// <value>
        /// The place.
        /// </value>
        public int Place { get; set; }

        /// <summary>
        /// Gets or sets the name of the nick.
        /// </summary>
        /// <value>
        /// The name of the nick.
        /// </value>
        public string NickName { get; set; }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets the avatar URL.
        /// </summary>
        /// <value>
        /// The avatar URL.
        /// </value>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// Gets or sets the last place set with scheduler
        /// </summary>
        /// <value>
        /// The last place.
        /// </value>
        public int? LastPlace { get; set; }
    }
}
