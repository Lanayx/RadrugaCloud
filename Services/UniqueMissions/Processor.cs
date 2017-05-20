namespace Services.UniqueMissions
{
    using System;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Models;
    using Core.Interfaces.Repositories;
    using Core.Tools;

    using Microsoft.Practices.Unity;

    using Services.DomainServices;

    internal static class UniqueMissionProcessor
    {
        internal static async Task<MissionCompletionResult> Process(
            Type uniqueMissionType,
            MissionRequest missionRequest,
            IMissionRequestRepository missionRequestRepository,
            IUserRepository userRepository,
            RatingService ratingService)
        {
            var oldUserPoints = missionRequest.User.Points;
            var uniqueMission = GetUniqueMission(uniqueMissionType);
            var result = await uniqueMission.ProcessRequest(missionRequest);
            if (result.Status != OperationResultStatus.Success)
            {
                return result;
            }

            var requestResult = await missionRequestRepository.AddMissionRequest(missionRequest);
            if (requestResult.Status != OperationResultStatus.Success)
            {
                return MissionCompletionResult.FromOperationResult(requestResult);
            }

            var userUpdateResult = await userRepository.UpdateUser(missionRequest.User);
            //after any mission completion user should be updated
            if (userUpdateResult.Status != OperationResultStatus.Error)
            {
                if (!missionRequest.User.Points.HasValue)
                    throw new Exception($"Points are null for userId {missionRequest.UserId}, mission {missionRequest.MissionId}, status {userUpdateResult.Status}");
                await ratingService.UpdateUserRating(missionRequest.User, oldUserPoints, missionRequest.User.Points.Value);
            }

            return userUpdateResult.Status == OperationResultStatus.Success
                       ? result
                       : MissionCompletionResult.FromOperationResult(userUpdateResult);
        }

        private static IUniqueMission GetUniqueMission(Type possibleUniqueMissionType)
        {
            return (IUniqueMission)IocConfig.GetConfiguredContainer().Resolve(possibleUniqueMissionType);
        }
    }
}