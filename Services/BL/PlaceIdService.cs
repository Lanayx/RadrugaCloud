namespace Services.BL
{
    using System;
    using System.Device.Location;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.Enums;
    using Core.Interfaces.Providers;
    using Core.NonDomainModels;
    using Core.NonDomainModels.Geocoding;
    using Core.Tools;

    using Newtonsoft.Json;

    /// <summary>
    /// Class PlaceIdService
    /// </summary>
    public class PlaceIdService
    {
        private readonly ILocationProvider _locationProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaceIdService"/> class.
        /// </summary>
        /// <param name="locationProvider">The location provider.</param>
        public PlaceIdService(ILocationProvider locationProvider)
        {
            _locationProvider = locationProvider;
        }

        /// <summary>
        ///     Processes this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public async Task<UserCityInfo> GetUniquePlaceId(GeoCoordinate homeCoordinate)
        {
            return await _locationProvider.GetUserCityInfo(homeCoordinate);
        }

    }
}