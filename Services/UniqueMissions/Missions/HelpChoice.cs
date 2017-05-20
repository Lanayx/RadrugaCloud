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
    /// Class HelpChoice
    /// </summary>
    public class HelpChoice : IUniqueMission
    {
        private readonly IMissionRepository _missionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpChoice"/> class.
        /// </summary>
        /// <param name="missionRepository">The mission repository.</param>
        public HelpChoice(IMissionRepository missionRepository)
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
            var userAnswer = request.Proof.CreatedText.ToLower();
            if ((userAnswer.Contains("мам") && !userAnswer.Contains("друг"))
                || (userAnswer.Contains("друг") && !userAnswer.Contains("мам")))
            {
                request.StarsCount = 1;
            }
            else if (userAnswer.Contains("мам") && userAnswer.Contains("друг") || userAnswer.Contains("обоим")
                     || userAnswer.Contains("двоим"))
            {
                request.StarsCount = 3;
            }
            else
            {
                request.DeclineReason = "Это неправильный выбор.";
                await RewardsCalculator.UpdateUserAfterMissionDecline(request, _missionRepository);
                return new MissionCompletionResult
                           {
                               MissionCompletionStatus = MissionCompletionStatus.Fail,
                               Description = request.DeclineReason
                           };
            }

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