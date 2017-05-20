namespace RadrugaCloud.Controllers
{
    using System;
    using System.Device.Location;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Core.NonDomainModels.Geocoding;
    using Core.Tools;

    using Newtonsoft.Json;

    using Services.BL;

    /// <summary>
    ///     Class TestController
    /// </summary>
    [Authorize]
    public class TestController : Controller
    {
        private readonly PlaceIdService _placeIdService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestController"/> class.
        /// </summary>
        /// <param name="placeIdService">The place identifier service.</param>
        public TestController(PlaceIdService placeIdService)
        {
            _placeIdService = placeIdService;
        }

        #region Public Methods and Operators

        // GET: Test
        /// <summary>
        ///     Indexes this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Index()
        {
            var model = new GeoModel();
            return View(model);
        }

        /// <summary>
        ///     Processes this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        public async Task<ActionResult> Process(GeoModel model)
        {
            var result = await _placeIdService.GetUniquePlaceId(new GeoCoordinate(model.Lat, model.Lon));

            if (result == null)
            {
                return View("Index", new GeoModel { Message = "Cant find Place ID" });
            }

            var message = $"PlaceId = {result.CountryShortName}:{result.CityShortName}";

            return View(
                "Index",
                new GeoModel { Message = message });
        }

        #endregion
    }

    /// <summary>
    ///     Class GeoModel
    /// </summary>
    public class GeoModel
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the lat.
        /// </summary>
        /// <value>The lat.</value>
        public double Lat { get; set; }

        /// <summary>
        ///     Gets or sets the lon.
        /// </summary>
        /// <value>The lon.</value>
        public double Lon { get; set; }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        #endregion
    }
}