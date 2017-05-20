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
    /// Class AppErrorInfoController
    /// </summary>
    public class AppErrorInfoController : ApiController
    {
        private readonly AppErrorInfoService _appErrorInfoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppErrorInfoController"/> class.
        /// </summary>
        /// <param name="appErrorInfoService">The AppErrorInfo service.</param>
        public AppErrorInfoController(AppErrorInfoService appErrorInfoService)
        {
            _appErrorInfoService = appErrorInfoService;
        }

        /// <summary>
        /// Posts the specified application error information.
        /// </summary>
        /// <param name="appErrorInfo">The application error information.</param>
        /// <returns>Task{AddResult}.</returns>
        public Task<IdResult> Post([FromBody]AppErrorInfo appErrorInfo)
        {
            appErrorInfo.UserId = User.Identity.IsAuthenticated ? this.GetCurrentUserId() : null;

            return _appErrorInfoService.AddNewAppErrorInfo(appErrorInfo.ConvertToDomain());
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