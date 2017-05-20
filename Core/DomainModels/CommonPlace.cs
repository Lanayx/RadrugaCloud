namespace Core.DomainModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Device.Location;

    /// <summary>
    /// Class CommonPlaces
    /// </summary>
    public class CommonPlace
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Required]
        public string SettlementId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is approved.
        /// </summary>
        /// <value><c>true</c> if this instance is approved; otherwise, <c>false</c>.</value>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the coordinate.
        /// </summary>
        /// <value>The coordinate.</value>
        public GeoCoordinate Coordinate { get; set; }
    }
}