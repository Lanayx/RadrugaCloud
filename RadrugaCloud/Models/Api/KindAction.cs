namespace RadrugaCloud.Models.Api
{
    using System;
    /// <summary>
    /// Kind action output model
    /// </summary>
    public class KindAction
    {

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date added.
        /// </summary>
        /// <value>
        /// The date added.
        /// </value>
        public DateTime DateAdded { get; set; }

        /// <summary>
        /// Gets or sets the list of userIds.
        /// </summary>
        /// <value>
        /// The likes.
        /// </value>
        public int LikesCount { get; set; }

        /// <summary>
        /// Gets or sets the list of userIds.
        /// </summary>
        /// <value>
        /// The dislikes.
        /// </value>
        public int DislikesCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether requesting user already liked this kindAction
        /// </summary>
        /// <value>
        ///   <c>true</c> if [already liked]; otherwise, <c>false</c>.
        /// </value>
        public bool AlreadyLiked { get; set; }
    }
}