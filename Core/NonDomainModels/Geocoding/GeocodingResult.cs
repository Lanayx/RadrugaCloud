// ReSharper disable InconsistentNaming
namespace Core.NonDomainModels.Geocoding
{
    using System.Collections.Generic;

    /// <summary>
    /// Class GeocodingResult
    /// </summary>
    public class GeocodingResult
    {
        /// <summary>
        ///     Gets or sets the address_components.
        /// </summary>
        /// <value>The address_components.</value>
        public IEnumerable<GeocodingAddressComponent> Address_components { get; set; }

        /// <summary>
        ///     Gets or sets the place_id.
        /// </summary>
        /// <value>The place_id.</value>
        public string Place_id { get; set; } 
    }
}