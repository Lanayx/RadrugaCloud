namespace Core.NonDomainModels
{
    /// <summary>
    /// Information about user home city
    /// </summary>
    public class UserCityInfo
    {
        /// <summary>
        /// Gets or sets the short name of the city.
        /// </summary>
        /// <value>
        /// The short name of the city.
        /// </value>
        public string CityShortName { get; set; }
        /// <summary>
        /// Gets or sets the short name of the country.
        /// </summary>
        /// <value>
        /// The short name of the country.
        /// </value>
        public string CountryShortName { get; set; }
    }
}
