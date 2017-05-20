namespace Infrastructure.InfrastructureTools.Google
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Device.Location;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net.Http;

    using Core.Interfaces.Providers;
    using Core.NonDomainModels;
    using Core.NonDomainModels.Geocoding;
    using Core.Tools;

    using Newtonsoft.Json;

    public class LocationProvider : ILocationProvider
    {
        // AIzaSyCB6CcO9QS_Y0AkSkKJLQRIF97fPSC95dA
        private const string BaseUri =
            "https://maps.googleapis.com/maps/api/geocode/json?{0}={1},{2}&key=AIzaSyCURSRyMP4Fs8B7T6NCZC8AHQvNwB8SiYo";

        public async Task<UserCityInfo> GetUserCityInfo(GeoCoordinate coordinate)
        {
            var client = new HttpClient();
            var uri = string.Format(
                CultureInfo.InvariantCulture,
                BaseUri,
                "latlng",
                coordinate.Latitude,
                coordinate.Longitude);
            var result = await client.GetStringAsync(uri);
            var geoResponse = JsonConvert.DeserializeObject<GeocodingResponse>(result);
            var emptyResponse = new UserCityInfo { CityShortName = "---", CountryShortName = "--" };
            if (geoResponse == null)
            {
                Trace.TraceError("No response from Google Apis");
                return null;
            }

            if (!string.Equals(geoResponse.Status, "Ok", StringComparison.InvariantCultureIgnoreCase))
            {
                Trace.TraceError($"Google Api Bad status - {geoResponse.Status}");
                return emptyResponse;
            }

            if (!geoResponse.Results.AnyValues())
            {
                Trace.TraceError($"Google Api Results count = 0, {coordinate}");
                return emptyResponse;
            }

            var country = "";
            var town = "";
            foreach (var geoResult in geoResponse.Results)
            {
                if (!geoResult.Address_components.AnyValues())
                {
                    continue;
                }

                var countryResult =
                    geoResult.Address_components.FirstOrDefault(
                        c =>
                        c.Types.AnyValues() && c.Types.Any(t => t == "country") && c.Types.Any(t => t == "political"));

                var townResult =
                    geoResult.Address_components.FirstOrDefault(
                        c =>
                        c.Types.AnyValues() && c.Types.Any(t => t == "locality") && c.Types.Any(t => t == "political"))
                        ??
                    //if city is not defined, let's try postal code
                     geoResult.Address_components.FirstOrDefault(
                        c =>
                        c.Types.AnyValues() && c.Types.Any(t => t == "postal_code"));
                
                if (townResult != null && countryResult != null)
                {
                    country = countryResult.Short_name;
                    town = townResult.Short_name;
                    break;
                }
            }

            town = String.IsNullOrEmpty(town) ? emptyResponse.CityShortName : town;
            country = String.IsNullOrEmpty(country) ? emptyResponse.CountryShortName : country;

            return new UserCityInfo { CityShortName = town, CountryShortName = country };
        }
    }
}
