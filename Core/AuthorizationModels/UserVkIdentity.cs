namespace Core.AuthorizationModels
{
    using Core.DomainModels;

    /// <summary>
    /// Fields uniquie to Vk
    /// </summary>
    public class VkIdentity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public uint Id { get; set; }


        /// <summary>
        /// Gets or sets the VK counters.
        /// </summary>
        /// <value>
        /// The counters.
        /// </value>
        public VkCounters Counters { get; set; }


        /// <summary>
        /// Gets or sets the university vk identifier.
        /// </summary>
        /// <value>
        /// The university identifier.
        /// </value>
        public uint? UniversityId { get; set; }


        /// <summary>
        /// Gets or sets the country identifier.
        /// </summary>
        /// <value>
        /// The country identifier.
        /// </value>
        public uint? CountryId { get; set; }


        /// <summary>
        /// Gets or sets the city identifier.
        /// </summary>
        /// <value>
        /// The city identifier.
        /// </value>
        public uint? CityId { get; set; }

    }
}
