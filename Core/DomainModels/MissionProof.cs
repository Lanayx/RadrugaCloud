namespace Core.DomainModels
{
    using System.Collections.Generic;
    using System.Device.Location;

    /// <summary>
    ///     The mission proof.
    /// </summary>
    public class MissionProof
    {
        /// <summary>
        ///     Gets or sets the coordinates.
        /// </summary>
        /// <value>
        ///     The coordinates.
        /// </value>
        public List<GeoCoordinate> Coordinates { get; set; }

        /// <summary>
        ///     Gets or sets the created text.
        /// </summary>
        /// <value>
        ///     The created text.
        /// </value>
        public string CreatedText { get; set; }

        /// <summary>
        ///     Gets or sets the image urls.
        /// </summary>
        public List<string> ImageUrls { get; set; }

        /// <summary>
        ///     Gets or sets the number of tries.
        /// </summary>
        /// <value>
        ///     The number of tries (sent by user only)
        /// </value>
        public ushort? NumberOfTries { get; set; }

        /// <summary>
        ///     Gets or sets the time elapsed in secondds
        /// </summary>
        /// <value>
        ///     The time elapsed.
        /// </value>
        public double? TimeElapsed { get; set; }
    }
}