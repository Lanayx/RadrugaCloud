// ReSharper disable InconsistentNaming
namespace Core.NonDomainModels.Geocoding
{
    using System.Collections.Generic;

    /// <summary>
    /// Class GeoAddressComponent
    /// </summary>
    public class GeocodingAddressComponent
    {
        /// <summary>
        ///     Gets or sets the long_name.
        /// </summary>
        /// <value>The long_name.</value>
        public string Long_name { get; set; }

        /// <summary>
        ///     Gets or sets the short_name.
        /// </summary>
        /// <value>The short_name.</value>
        public string Short_name { get; set; }

        /// <summary>
        ///     Gets or sets the types.
        /// </summary>
        /// <value>The types.</value>
        public IEnumerable<string> Types { get; set; }
    }
}