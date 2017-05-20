namespace RadrugaCloud.Controllers.Api
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Core.CommonModels.Results;

    using RadrugaCloud.Helpers;
    using RadrugaCloud.Models.Api;

    using Services.DomainServices;

    /// <summary>
    /// Controller for special actions
    /// </summary>
    public class SpecialsController : ApiController
    {
        private readonly SpecialsService _specialsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppErrorInfoController"/> class.
        /// </summary>
        /// <param name="appErrorInfoService">The AppErrorInfo service.</param>
        public SpecialsController(SpecialsService appErrorInfoService)
        {
            _specialsService = appErrorInfoService;
        }

        [HttpPost]
        [Authorize]
        public Task<OperationResult> PostMessageToDevelopers([FromBody]string message)
        {
            var userId = this.GetCurrentUserId();

            return _specialsService.PostMessageToDevelopers(userId, message);
        }

        [HttpGet]
        [Authorize]
        public Task<string> GetRatePageUrl(string platform)
        {
            var userId = this.GetCurrentUserId();

            return _specialsService.GetRatePageUrl(userId, platform);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <exception cref="System.Exception">This exception was thrown in an action method.</exception>
        public string Get()
        {
            throw new Exception("This exception was thrown in an action method.");
        }
    }
}