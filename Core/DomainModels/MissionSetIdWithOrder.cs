namespace Core.DomainModels
{
    /// <summary>
    /// Mission set id with it's order in user setline
    /// </summary>
    public class MissionSetIdWithOrder
    {
        /// <summary>
        /// Gets or sets the mission set identifier.
        /// </summary>
        /// <value>
        /// The mission set identifier.
        /// </value>
        public string MissionSetId { get; set; }

        /// <summary>
        /// Gets or sets the set order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }
    }
}
