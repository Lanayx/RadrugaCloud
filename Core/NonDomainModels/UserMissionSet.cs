namespace Core.NonDomainModels
{
    using System.Collections.Generic;

    /// <summary>
    /// Class UserMissionSet
    /// </summary>
    public class UserMissionSet
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the missions.
        /// </summary>
        /// <value>The missions.</value>
        public IEnumerable<UserMission> Missions { get; set; } 
    }
}