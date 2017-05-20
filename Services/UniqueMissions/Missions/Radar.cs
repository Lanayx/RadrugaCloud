namespace Services.UniqueMissions.Missions
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Models;
    using Core.Interfaces.Repositories;
    using Core.Tools;

    using Services.BL;
    using Services.DomainServices;

    /// <summary>
    /// Class Radar
    /// </summary>
    public class Radar : IUniqueMission
    {
        private readonly IMissionRepository _missionRepository;
        private readonly IMissionRequestRepository _missionRequestRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="Radar" /> class.
        /// </summary>
        /// <param name="missionRepository">The mission repository.</param>
        /// <param name="missionRequestRepository">The mission request repository.</param>
        public Radar(IMissionRepository missionRepository, IMissionRequestRepository missionRequestRepository)
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
            request.Mission.TriesFor1Star = 10;
            request.Mission.TriesFor2Stars = 6;
            request.Mission.TriesFor3Stars = 2;

            if (request.Proof.Coordinates == null || !request.Proof.Coordinates.Any())
                return new MissionCompletionResult(OperationResultStatus.Error, "Not enough coordinates");
            var radarCoordinate = request.Proof.Coordinates.First();
            var borders = GetBorders(request.User.HomeCoordinate, radarCoordinate);
            bool isNear;
            if (!CheckValidRadar(radarCoordinate, borders, request.User, out isNear))
            {
                var incorrectResult = await MissionRequestService.ProcessIncorrectTry(request, _missionRepository, _missionRequestRepository); if (incorrectResult.MissionCompletionStatus == MissionCompletionStatus.IntermediateFail)
                incorrectResult.Description = isNear ? "Incorrect" : "IsNear";
                return incorrectResult;
            }

            await MissionRequestService.SetStarsAccordingToTries(request,_missionRequestRepository);

            request.User.RadarCoordinate = radarCoordinate;
            request.User.Selected2BaseCoordinates = borders;

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

        private List<string> GetBorders(GeoCoordinate homeCoordinate, GeoCoordinate radarCoordinate)
        {
            var coordinateList = new List<string> { "East", "West", "North", "South" };
            coordinateList.Remove(radarCoordinate.Latitude > homeCoordinate.Latitude ? "South" : "North");
            coordinateList.Remove(radarCoordinate.Longitude > homeCoordinate.Longitude ? "West" : "East");
            return coordinateList;
        }

        private bool CheckValidRadar(GeoCoordinate radarCoordinate, List<string> borders, User user, out bool isNear)
        {
            var twoCoordinates = new List<GeoCoordinate>();
            if (borders.Contains("East"))
                twoCoordinates.Add(user.BaseEastCoordinate);
            if (borders.Contains("West"))
                twoCoordinates.Add(user.BaseWestCoordinate);
            if (borders.Contains("North"))
                twoCoordinates.Add(user.BaseNorthCoordinate);
            if (borders.Contains("South"))
                twoCoordinates.Add(user.BaseSouthCoordinate);
            GeoCoordinate firstCoord = twoCoordinates[0], secondCoordinate = twoCoordinates[1], homeCoordinate = user.HomeCoordinate;
            isNear = true;
            return IsValidAngle(GetAngleFor3Points(firstCoord, radarCoordinate, homeCoordinate), ref isNear)
                   && IsValidAngle(GetAngleFor3Points(secondCoordinate, radarCoordinate, homeCoordinate), ref isNear)
                   && IsValidAngle(GetAngleFor3Points(firstCoord, radarCoordinate, secondCoordinate), ref isNear);
        }

        //All three points should be visible at 120 degree angle (2.094 rad) https://ru.wikipedia.org/wiki/Точка_Ферма
        private bool IsValidAngle(double angle, ref bool isNear)
        {
            double allowedError = 0.06, idealAngle = 2.094;
            var isValid = angle > (idealAngle - allowedError) && angle < (idealAngle + allowedError);
            isNear = isNear && angle > (idealAngle - 2*allowedError) && angle < (idealAngle + 2*allowedError);
            return isValid;
        }


        // according to http://en.wikipedia.org/wiki/Law_of_cosines
        private double GetAngleFor3Points(GeoCoordinate first, GeoCoordinate middle, GeoCoordinate second)
        {
            var a = first.GetDistanceTo(middle);
            var b = second.GetDistanceTo(middle);
            var c = first.GetDistanceTo(second);
            return Math.Acos((a * a + b * b - c * c) / (2 * a * b));
        }
    }
}
