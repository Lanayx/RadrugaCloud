namespace RadrugaCloud.Controllers.Api
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.OData.Query;

    using Core.CommonModels.Results;
    using Core.Enums;

    using Infrastructure.InfrastructureTools;

    using Helpers;
    using Models.Api;
    using Models.Attributes;

    using Services.BL;
    using Services.DomainServices;

    /// <summary>
    /// Class KindActionController
    /// </summary>
    [Authorize]
    public class KindActionController : ApiController
    {
        private readonly KindActionService _kindActionService;
        private readonly ImageService _imageService;
        private readonly AppCountersService _appCountersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="KindActionController" /> class.
        /// </summary>
        /// <param name="kindActionService">The kindAction service.</param>
        /// <param name="imageService">The image service</param>
        /// <param name="appCountersService">The application counters service.</param>
        public KindActionController(KindActionService kindActionService, ImageService imageService, AppCountersService appCountersService)
        {
            _imageService = imageService;
            _kindActionService = kindActionService;
            _appCountersService = appCountersService;
        }

        /// <summary>
        /// Posts the specified kind action.
        /// </summary>
        /// <returns>Task{AddResult}.</returns>
        [ValidateMimeMultipartContentFilter]
        public async Task<KindActionResult> Post()
        {
            var parsedContent = await FileUploadHelper.GetMultipartContent(Request, "KindActionDescription");
            if (parsedContent.ErrorType == MultipartDataError.FileDataEmpty)
                return new KindActionResult(OperationResultStatus.Error, "Attached file is empty");
            if (parsedContent.ErrorType == MultipartDataError.TextDataEmpty)
                return new KindActionResult(OperationResultStatus.Error, "Kind action description is empty");

            var kindAction = new Core.DomainModels.KindAction
                                 {
                                     UserId = this.GetCurrentUserId(),
                                     Description = HttpUtility.UrlDecode(parsedContent.TextData)
                                 };
            if (!kindAction.ValidateObject())
            {
                return new KindActionResult(OperationResultStatus.Error, "Validation error");
            }


            var imagePath = await _imageService.SaveKindActionImage(parsedContent.FileData);
            kindAction.ImageUrl = imagePath;

            var result = await _kindActionService.AddNewKindAction(kindAction);
            if (result.Status == OperationResultStatus.Success)
                await _appCountersService.KindActionSubmited();
            return result;
        }

        /// <summary>
        /// Posts the specified kind action without image
        /// </summary>
        /// <param name="kindActionDescription">The kind action description.</param>
        /// <returns>Task{AddResult}.</returns>
        [HttpPost]
        public async Task<KindActionResult> PostWithoutImage([FromBody]string kindActionDescription)
        {
            var kindAction = new Core.DomainModels.KindAction
            {
                UserId = this.GetCurrentUserId(),
                Description = kindActionDescription
            };
            if (!kindAction.ValidateObject())
            {
                return new KindActionResult(OperationResultStatus.Error, "Validation error");
            }

            var result = await _kindActionService.AddNewKindAction(kindAction);
            if (result.Status == OperationResultStatus.Success)
                await _appCountersService.KindActionSubmited();
            return result;
        }

        /// <summary>
        /// Gets kind actions with options.
        /// </summary>
        /// <param name="odataOptions">The odata options.</param>
        /// <returns></returns>
        public async Task<List<KindAction>> Get(ODataQueryOptions<Core.DomainModels.KindAction> odataOptions)
        {
            var currentUserId = this.GetCurrentUserId();
            var kinActions = await _kindActionService.GetKindActions(currentUserId, odataOptions.ToQueryOptions());
            return kinActions.Select(kindAction => kindAction.ConvertToApi(currentUserId)).ToList();
        }


        /// <summary>
        /// Likes the specified kind action identifier.
        /// </summary>
        /// <param name="kindActionIds">The kind action ids.</param>
        /// <returns></returns>
        [HttpPost]
        public Task<OperationResult> Like(KindActionIds kindActionIds)
        {
            var currentUserId = this.GetCurrentUserId();
            return _kindActionService.JudgeKindAction(currentUserId, kindActionIds.UserId, kindActionIds.KindActionId, true);
        }

        /// <summary>
        /// Dislikes the specified kind action identifier.
        /// </summary>
        /// <param name="kindActionIds">The kind action ids.</param>
        /// <returns></returns>
        [HttpPost]
        public Task<OperationResult> Dislike(KindActionIds kindActionIds)
        {
            var currentUserId = this.GetCurrentUserId();
            return _kindActionService.JudgeKindAction(currentUserId, kindActionIds.UserId, kindActionIds.KindActionId,  false);
        }

    }
}