namespace Services.UniqueMissions.Missions
{
    using System.Linq;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Models;
    using Core.Interfaces.Repositories;

    using Services.BL;
    using Services.DomainServices;

    /// <summary>
    ///     Class CommandPoint
    /// </summary>
    public class CommandPoint : IUniqueMission
    {
        private readonly AppCountersService _appCountersService;
        private readonly PlaceIdService _placeIdService;

        private readonly IMissionRepository _missionRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandPoint" /> class.
        /// </summary>
        /// <param name="missionRepository">The mission repository.</param>
        /// <param name="appCountersService">The application counters service.</param>
        public CommandPoint(IMissionRepository missionRepository, AppCountersService appCountersService, PlaceIdService placeIdService)
        {
            _missionRepository = missionRepository;
            _appCountersService = appCountersService;
            _placeIdService = placeIdService;
        }

        /// <summary>
        ///     Processes the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task{MissionCompletionResult}.</returns>
        public async Task<MissionCompletionResult> ProcessRequest(MissionRequest request)
        {
            var homeCoordinate = request.Proof.Coordinates.First();
            request.User.HomeCoordinate = homeCoordinate;
            var homePlaceIdResult = await _placeIdService.GetUniquePlaceId(request.User.HomeCoordinate);
            if (homePlaceIdResult== null)
            {
                request.DeclineReason = "Невозможно опеределить идентификатор города. Возможно, ты находишься в пути. Попробуй еще раз позже.";
                return new MissionCompletionResult
                {
                    MissionCompletionStatus = MissionCompletionStatus.IntermediateFail,
                    Description = request.DeclineReason
                };
            }

            request.User.CityShortName = homePlaceIdResult.CityShortName;
            request.User.CountryShortName = homePlaceIdResult.CountryShortName;
            request.StarsCount = 3;
            var recievedPoints = await RewardsCalculator.UpdateUserAfterMissionCompletion(request, _missionRepository);
            await _appCountersService.UserPassedFirstMission();
            return new MissionCompletionResult
                       {
                           Points = recievedPoints,
                           StarsCount = request.StarsCount,
                           MissionCompletionStatus = MissionCompletionStatus.Success
                       };
        }
    }
}