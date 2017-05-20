namespace Services.UniqueMissions.Missions
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
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
    /// Class Outpost
    /// </summary>
    public class Outpost : IUniqueMission
    {
        /// <summary>
        /// The _mission repository
        /// </summary>
        private readonly IMissionRepository _missionRepository;
        /// <summary>
        /// The _mission repository
        /// </summary>
        private readonly IMissionRequestRepository _missionRequestRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="Outpost" /> class.
        /// </summary>
        /// <param name="missionRepository">The mission repository.</param>
        /// <param name="missionRequestRepository">The mission request repository.</param>
        public Outpost(IMissionRepository missionRepository, IMissionRequestRepository missionRequestRepository)
        {
            _missionRepository = missionRepository;
            _missionRequestRepository = missionRequestRepository;
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task{MissionCompletionResult}.</returns>
        public async Task<MissionCompletionResult> ProcessRequest(MissionRequest request)
        {
            request.Mission.TriesFor1Star = 7;
            request.Mission.TriesFor2Stars = 4;
            request.Mission.TriesFor3Stars = 2;

            if (request.Proof.Coordinates == null || !request.Proof.Coordinates.Any())
                return new MissionCompletionResult(OperationResultStatus.Error, "Not enough data");

            var oupostCoordinate = request.Proof.Coordinates.First();
            double distanceError;
            if (!CheckOutpost(oupostCoordinate, request.User, out distanceError))
            {
                return await MissionRequestService.ProcessIncorrectTry(request, _missionRepository, _missionRequestRepository);
            }
            await MissionRequestService.SetStarsAccordingToTries(request, _missionRequestRepository);
       
            request.User.OutpostCoordinate = oupostCoordinate;

            var recievedPoints = await RewardsCalculator.UpdateUserAfterMissionCompletion(
                                           request,
                                           _missionRepository);

            return new MissionCompletionResult
                       {
                           Points = recievedPoints,
                           StarsCount = request.StarsCount,
                           MissionCompletionStatus = MissionCompletionStatus.Success
                       };
        }

      

        private bool CheckOutpost(GeoCoordinate oupostCoordinate, User user, out double distance)
        {
            var twoCoordinates = new List<GeoCoordinate>();
            if (user.Selected2BaseCoordinates.Contains("East"))
                twoCoordinates.Add(user.BaseEastCoordinate);
            if (user.Selected2BaseCoordinates.Contains("West"))
                twoCoordinates.Add(user.BaseWestCoordinate);
            if (user.Selected2BaseCoordinates.Contains("North"))
                twoCoordinates.Add(user.BaseNorthCoordinate);
            if (user.Selected2BaseCoordinates.Contains("South"))
                twoCoordinates.Add(user.BaseSouthCoordinate);

            var firstDistanseDifference = Math.Abs(user.RadarCoordinate.GetDistanceTo(twoCoordinates[0])
                                     - oupostCoordinate.GetDistanceTo(twoCoordinates[0]));

            var secondDistanseDifference = Math.Abs(user.RadarCoordinate.GetDistanceTo(twoCoordinates[1])
                                     - oupostCoordinate.GetDistanceTo(twoCoordinates[1]));

            distance = firstDistanseDifference > secondDistanseDifference
                           ? firstDistanseDifference
                           : secondDistanseDifference;

            return firstDistanseDifference < 11 && secondDistanseDifference < 11
                   && oupostCoordinate.GetDistanceTo(user.RadarCoordinate) > 11;

        }
    }
}
