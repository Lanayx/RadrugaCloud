namespace RadrugaCloud.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.NonDomainModels;

    using RadrugaCloud.Helpers;
    using RadrugaCloud.Models.Api;
    using RadrugaCloud.Models.Attributes;

    using Services.BL;
    using Services.DomainServices;

    /// <summary>
    /// Class MissionController
    /// </summary>
    [Authorize]
    public class MissionController : ApiController
    {
        private readonly MissionService _missionService;
        private readonly MissionRequestService _missionRequestService;
        private readonly ImageService _imageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MissionController" /> class.
        /// </summary>
        /// <param name="missionService">The mission service.</param>
        /// <param name="missionRequestService">The mission request service.</param>
        /// <param name="imageService">The image service.</param>
        public MissionController(MissionService missionService, MissionRequestService missionRequestService, ImageService imageService)
        {
            _missionService = missionService;
            _missionRequestService = missionRequestService;
            _imageService = imageService;
        }

        /// <summary>
        /// Declines the mission.
        /// </summary>
        /// <returns>Task{OperationResult}.</returns>
        [HttpPost]
        public async Task<MissionCompletionResult> DeclineMission(string missionId)
        {
            if (String.IsNullOrEmpty(missionId))
                return (MissionCompletionResult)OperationResult.NotFound;

            var userId = this.GetCurrentUserId();
            return await _missionService.DeclineMission(userId, missionId);
        }

        /// <summary>
        /// Gets the user mission sets.
        /// </summary>
        /// <returns>Task{IEnumerable{UserMissionSet}}.</returns>
        public async Task<IEnumerable<UserMissionSet>> GetMissionSets()
        {
            return await _missionService.GetActiveMissionSets(this.GetCurrentUserId());
        }

        /// <summary>
        /// Completes the mission.
        /// </summary>
        /// <param name="missionId">The mission identifier.</param>
        /// <param name="missionProof">The mission proof.</param>
        /// <returns>Task{MissionCompletionResult}</returns>
        [HttpPost]
        public async Task<MissionCompletionResult> CompleteMission(string missionId, MissionProof missionProof)
        {
            //TODO validate image urls

            if (string.IsNullOrEmpty(missionId))
            {
                return (MissionCompletionResult)OperationResult.NotFound;
            }

            var userId = this.GetCurrentUserId();
            return await _missionRequestService.CompleteMission(userId, missionId, missionProof);
        }


        /// <summary>
        /// Completes mission with only one photo (to reduce requests amount).
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateMimeMultipartContentFilter]
        public async Task<MissionCompletionResult> CompleteOnePhotoMission()
        {
            var parsedContent = await FileUploadHelper.GetMultipartContent(Request, "MissionId");
            if (parsedContent.ErrorType == MultipartDataError.FileDataEmpty)
                return new MissionCompletionResult(OperationResultStatus.Error, "Attached file is empty");
            if (parsedContent.ErrorType == MultipartDataError.TextDataEmpty)
                return new MissionCompletionResult(OperationResultStatus.Error, "Mission id is empty");

            var imagePath = await _imageService.SaveProofImage(parsedContent.FileData);
            var missionId = parsedContent.TextData;
            var userId = this.GetCurrentUserId();


            return await _missionRequestService.CompleteMission(userId, missionId, new MissionProof { ImageUrls = new List<string> { imagePath } });
        }

        /// <summary>
        /// Use when add multiple images as a proof
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateMimeMultipartContentFilter]
        public async Task<string> AddProofImage()
        {
            var oneFileData = await FileUploadHelper.GetOneFileContent(Request);
            if (oneFileData.ErrorType == MultipartDataError.FileDataEmpty)
                 return "";// meaning url couldn't be retrieved. I don't want to throw exception and touch elmah.

            return await _imageService.SaveProofImage(oneFileData.FileData);
        }


        /// <summary>
        /// Gets the hint.
        /// </summary>
        /// <param name="missionId">The mission identifier.</param>
        /// <param name="hintId">The hint identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HintRequestResult> GetHint(string missionId, string hintId)
        {
            return await _missionService.GetHint(missionId, hintId, this.GetCurrentUserId());                                  
        }
    }
}
