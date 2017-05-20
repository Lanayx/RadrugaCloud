namespace Services.UniqueMissions.Missions
{
    using System;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Models;
    using Core.Interfaces.Repositories;

    using Services.BL;
    using Services.DomainServices;

    /// <summary>
    /// Class Censored
    /// </summary>
    public class Censored : IUniqueMission
    {
        private readonly IMissionRepository _missionRepository;
        private readonly AppCountersService _appCountersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Censored"/> class.
        /// </summary>
        /// <param name="missionRepository">The mission repository.</param>
        /// <param name="appCountersService">The app counters service.</param>
        public Censored(IMissionRepository missionRepository, AppCountersService appCountersService)
        {
            _missionRepository = missionRepository;
            _appCountersService = appCountersService;
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<MissionCompletionResult> ProcessRequest(MissionRequest request)
        {
            if (String.IsNullOrEmpty(request.Proof.CreatedText))
            {
                request.StarsCount = 3;
                var recievedPoints =
                    await RewardsCalculator.UpdateUserAfterMissionCompletion(request, _missionRepository, _appCountersService);
                return new MissionCompletionResult
                           {
                               Points = recievedPoints,
                               StarsCount = request.StarsCount,
                               MissionCompletionStatus = MissionCompletionStatus.Success
                           };
            }

            request.DeclineReason = "Увы. В следующий раз постарайся думать своей головой и не совершать глупости";
            await RewardsCalculator.UpdateUserAfterMissionDecline(request, _missionRepository, _appCountersService);
            return new MissionCompletionResult
                       {
                           MissionCompletionStatus = MissionCompletionStatus.Fail,
                           Description = request.DeclineReason
                       };
        }
    }
}
