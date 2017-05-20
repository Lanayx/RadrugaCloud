namespace RadrugaCloud.Areas.Admin.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Core.Enums;

    using Infrastructure.AzureTablesObjects.DeveloperTools;

    using RadrugaCloud.Models;
    using RadrugaCloud.Models.Attributes;

    using Services.BL;
    using Services.DomainServices;

    /// <summary>
    ///     Class DeveloperController
    /// </summary>
    [Authorize]
    public class DeveloperController : Controller
    {
        private readonly ImageService _imageService;
        private readonly UserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeveloperController"/> class.
        /// </summary>
        /// <param name="imageService">The image service.</param>
        public DeveloperController(ImageService imageService, UserService userService)
        {
            _imageService = imageService;
            _userService = userService;
        }

        #region Public Methods and Operators

        /// <summary>
        ///     Deletes all tables.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [AllowMultipleInputButtons(Name = "action", Argument = "DeleteAllTables")]
        public async Task<ActionResult> DeleteAllTables()
        {
            return await ExecuteDeveloperOperation(DeveloperOperation.DeleteAllTables);
        }

        /// <summary>
        ///     Fakes this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [AllowMultipleInputButtons(Name = "action", Argument = "Fake")]
        public ActionResult Fake()
        {
            const bool ISError = false;
            const string ReturningResult = "Fake executed";

            Session["InSessionResult"] = ReturningResult;
            return RedirectToAction("Index", new { isError = ISError });
        }

        /// <summary>
        ///     Fills the tables with test data.
        /// </summary>
        /// <returns>Task{ActionResult}.</returns>
        [HttpPost]
        [AllowMultipleInputButtons(Name = "action", Argument = "FillTablesWithTestData")]
        public async Task<ActionResult> FillTablesWithTestData()
        {
            return await ExecuteDeveloperOperation(DeveloperOperation.FillTablesWithData);
        }

        /// <summary>
        ///     Deletes all tables.
        /// </summary>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [AllowMultipleInputButtons(Name = "action", Argument = "FixExternalImages")]
        public async Task<ActionResult> FixExternalImages()
        {
            return await ExecuteDeveloperOperation(DeveloperOperation.FixExternalImages);
        }

        /// <summary>
        /// Cleans the temp images storage.
        /// </summary>
        /// <returns>Task{ActionResult}.</returns>
        [HttpPost]
        [AllowMultipleInputButtons(Name = "action", Argument = "CleanTempImagesStorage")]
        public async Task<ActionResult> CleanTempImagesStorage()
        {
            return await ExecuteDeveloperOperation(DeveloperOperation.CleanTempImagesStorage);
        }

        /// <summary>
        /// Cleans the temp images storage.
        /// </summary>
        /// <returns>Task{ActionResult}.</returns>
        [HttpPost]
        [AllowMultipleInputButtons(Name = "action", Argument = "UpdateUserLocations")]
        public async Task<ActionResult> UpdateUserLocations()
        {
            return await ExecuteDeveloperOperation(DeveloperOperation.UpdateUserLocations);
        }

        /// <summary>
        /// Cleans the temp images storage.
        /// </summary>
        /// <returns>Task{ActionResult}.</returns>
        [HttpPost]
        [AllowMultipleInputButtons(Name = "action", Argument = "UpdateKindActionsCount")]
        public async Task<ActionResult> UpdateKindActionsCount()
        {
            return await ExecuteDeveloperOperation(DeveloperOperation.UpdateKindActionsCount);
        }

        /// <summary>
        /// Fixes light quality
        /// </summary>
        /// <returns>Task{ActionResult}.</returns>
        [HttpPost]
        [AllowMultipleInputButtons(Name = "action", Argument = "FixLightQuality")]
        public async Task<ActionResult> FixLightQuality()
        {
            return await ExecuteDeveloperOperation(DeveloperOperation.FixLightQuality);
        }

        /// <summary>
        /// Updates radruga oclor
        /// </summary>
        /// <returns>Task{ActionResult}.</returns>
        [HttpPost]
        [AllowMultipleInputButtons(Name = "action", Argument = "UpdateRadrugaColor")]
        public async Task UpdateRadrugaColor()
        {
            await _userService.UpdateRadrugaColor();
        }

        /// <summary>
        ///     The index.
        /// </summary>
        /// <param name="isError">if set to <c>true</c> [is error].</param>
        /// <returns>The <see cref="ActionResult" />.</returns>
        public async Task<ActionResult> Index(bool isError = false)
        {
            var result = Session["InSessionResult"]?.ToString() ?? "State OK.";
            result = result.Replace("\n", "<br/>");
            var imagesResult = await _imageService.GetTempImagesCount();
            var model = new DeveloperOperationsModel
                            {
                                Result = result,
                                IsError = isError,
                                TempImagesCount = imagesResult
                            };

            Session["InSessionResult"] = null;
            return View(model);
        }

        #endregion

        #region Methods

        private async Task<ActionResult> ExecuteDeveloperOperation(DeveloperOperation type)
        {
            var manager = new DeveloperStorageManager();
            var result = await manager.ExecuteOperation(type);
            bool isError = false;
            string returningResult = "Success";
            if (result.Status != OperationResultStatus.Success)
            {
                isError = true;
                returningResult = result.Description;
            }

            Session["InSessionResult"] = returningResult;
            return RedirectToAction("Index", new { isError });
        }

        #endregion
    }
}