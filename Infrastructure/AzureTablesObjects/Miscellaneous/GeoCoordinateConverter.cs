namespace Infrastructure.AzureTablesObjects
{
    using System.Collections.Generic;
    using System.Device.Location;

    using Core.Tools;

    /// <summary>
    /// Class GeoCoordinateConverter
    /// </summary>
    public static class GeoCoordinateConverter
    {
        /// <summary>
        /// Converts to azure coordinate.
        /// </summary>
        /// <param name="coordinate">The coordinate.</param>
        /// <returns>System.String.</returns>
        public static string ConvertToAzureCoordinate(this GeoCoordinate coordinate)
        {
            if (coordinate == null)
            {
                return null;
            }

            var collection = new List<double>
                                 {
                                     coordinate.Latitude, // 0
                                     coordinate.Longitude, // 1
                                     coordinate.Altitude, // 2
                                     coordinate.HorizontalAccuracy, // 3
                                     coordinate.VerticalAccuracy, // 4
                                     coordinate.Course, // 5
                                     coordinate.Speed // 6
                                 };

            var stringCoordinate = collection.JoinToString();
            return stringCoordinate;
        }

        /// <summary>
        /// Converts to geo coordinate.
        /// </summary>
        /// <param name="azureCoordinate">The azure coordinate.</param>
        /// <returns>GeoCoordinate.</returns>
        public static GeoCoordinate ConvertToGeoCoordinate(this string azureCoordinate)
        {
            var collection = azureCoordinate.SplitStringByDelimiter();
            if (!collection.AnyValues() || collection.Count != 7)
            {
                return null;
            }

            var latitude = double.Parse(collection[0]);
            var longitude = double.Parse(collection[1]);
            var altitude = double.Parse(collection[2]);
            var horizontalAccuracy = double.Parse(collection[3]);
            var verticalAccuracy = double.Parse(collection[4]);
            var course = double.Parse(collection[5]);
            var speed = double.Parse(collection[6]);
            var coordinate = new GeoCoordinate(
                latitude,
                longitude,
                altitude,
                horizontalAccuracy,
                verticalAccuracy,
                speed,
                course);
            return coordinate;
        }
    }
}