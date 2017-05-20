namespace Core.NonDomainModels.Geocoding
{
    using System.Collections.Generic;

    /// <summary>
    /// Class GeocodingResponse
    /// </summary>
    public class GeocodingResponse
    {
        /// <summary>
        ///     Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        public IEnumerable<GeocodingResult> Results { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string Status { get; set; }
    }
}