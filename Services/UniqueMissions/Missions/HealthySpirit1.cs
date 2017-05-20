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

    /// <summary>
    ///     Class HealthySpirit1
    /// </summary>
    public class HealthySpirit1 : IUniqueMission
    {
        private readonly IMissionRepository _missionRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HealthySpirit1" /> class.
        /// </summary>
        /// <param name="missionRepository">The mission repository.</param>
        public HealthySpirit1(IMissionRepository missionRepository)
        {
            _missionRepository = missionRepository;
        }

        /// <summary>
        ///     Processes the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task{MissionCompletionResult}.</returns>
        public async Task<MissionCompletionResult> ProcessRequest(MissionRequest request)
        {
            ushort amount;
            if (!ushort.TryParse(request.Proof.CreatedText, out amount))
            {
                return new MissionCompletionResult(OperationResultStatus.Error, "Amount is not a number");
            }
            if (amount > 180 || amount < 10)
            {
                request.DeclineReason = "Не верим!";
                await RewardsCalculator.UpdateUserAfterMissionDecline(request, _missionRepository);
                return new MissionCompletionResult
                           {
                               MissionCompletionStatus = MissionCompletionStatus.Fail,
                               Description = request.DeclineReason
                           };
            }

            var dateOfBirth = request.User.DateOfBirth ?? DateTime.UtcNow;
            request.StarsCount = GetStarsBasingOnSexAndAge(
                (DateTime.UtcNow - dateOfBirth).TotalDays / 365,
                request.User.Sex ?? Sex.NotSet,
                amount);

            var recievedPoints = await RewardsCalculator.UpdateUserAfterMissionCompletion(request, _missionRepository);
            return new MissionCompletionResult
                       {
                           Points = recievedPoints,
                           StarsCount = request.StarsCount,
                           MissionCompletionStatus = MissionCompletionStatus.Success
                       };
        }

        private byte GetStarsBasingOnSexAndAge(double age, Sex sex, ushort amount)
        {
            if (amount > 109 || (amount > 99 && age < 17) || (amount > 89 && sex == Sex.Female)
                || (amount > 69 && sex == Sex.Female && age < 17))
            {
                return 3;
            }
            if (amount > 89 || (amount > 79 && age < 17) || (amount > 69 && sex == Sex.Female)
                || (amount > 49 && sex == Sex.Female && age < 17))
            {
                return 2;
            }
            return 1;
        }
    }
}