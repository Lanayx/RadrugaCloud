namespace Core.NonDomainModels
{
    using System.Collections.Generic;

    using Core.DomainModels;

    /// <summary>
    /// Ratings with user count
    /// </summary>
    public class RatingsWithUserCount
    {
        /// <summary>
        /// Gets or sets the users count in the selected rating.
        /// </summary>
        /// <value>
        /// The users count.
        /// </value>
        public int UsersCount { get; set; }

        /// <summary>
        /// Gets or sets the selected type of ratings.
        /// </summary>
        /// <value>
        /// The ratings.
        /// </value>
        public List<RatingInfo> Ratings { get; set; } 
    }
}