namespace Core.NonDomainModels
{
    /// <summary>
    /// Mission id with Set id
    /// </summary>
    public class MissionIdWithSetId
    {
        /// <summary>
        /// Gets or sets the mission set identifier.
        /// </summary>
        /// <value>
        /// The mission set identifier.
        /// </value>
        public string MissionSetId { get; set; }


        /// <summary>
        /// Gets or sets the mission identifier.
        /// </summary>
        /// <value>
        /// The mission identifier.
        /// </value>
        public string MissionId { get; set; }
    }
}
