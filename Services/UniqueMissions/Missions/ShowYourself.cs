namespace Services.UniqueMissions.Missions
{
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Models;
    using Core.Interfaces.Repositories;

    using Services.BL;

    /// <summary>
    /// Class ShowYourself
    /// </summary>
    public class ShowYourself : IUniqueMission
    {
        private readonly IMissionRepository _missionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowYourself"/> class.
        /// </summary>
        /// <param name="missionRepository">The mission repository.</param>
        public ShowYourself(IMissionRepository missionRepository)
        {
            _missionRepository = missionRepository;
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task{MissionCompletionResult}.</returns>
        public async Task<MissionCompletionResult> ProcessRequest(MissionRequest request)
        {
            request.StarsCount = 3;
            var recievedPoints = await RewardsCalculator.UpdateUserAfterMissionCompletion(request, _missionRepository);
            return new MissionCompletionResult
                       {
                           Points = recievedPoints,
                           StarsCount = request.StarsCount,
                           MissionCompletionStatus = MissionCompletionStatus.Success
                       };
        }
    }
}