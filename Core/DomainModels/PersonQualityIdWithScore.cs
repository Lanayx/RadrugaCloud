namespace Core.DomainModels
{
    /// <summary>
    /// Person type with score
    /// </summary>
    public class PersonQualityIdWithScore
    {
        /// <summary>
        /// Gets or sets the person quality identifier.
        /// </summary>
        /// <value>
        /// The person quality identifier.
        /// </value>
        public string PersonQualityId { get; set; }

        /// <summary>
        /// Gets or sets The more is the score the more Person Type is present
        /// </summary>
        public double Score { get; set; }
    }
}
