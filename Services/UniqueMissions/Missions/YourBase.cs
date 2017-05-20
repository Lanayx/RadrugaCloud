namespace Services.UniqueMissions.Missions
{
    using System.Device.Location;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Models;
    using Core.Interfaces.Repositories;

    using Services.BL;

    /// <summary>
    ///     Class YourBase
    /// </summary>
    public class YourBase : IUniqueMission
    {
        private const int MaxBorderDistanse = 260; //meters (250 ~ 10)

        private const int MinBorderDistance = 140; //meters (150 ~ 10)

        private readonly IMissionRepository _missionRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="YourBase" /> class.
        /// </summary>
        /// <param name="missionRepository">The mission repository.</param>
        public YourBase(IMissionRepository missionRepository)
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
            var baseCoordinates = request.Proof.Coordinates.ToArray();

            if (!ValidBaseCoordinates(baseCoordinates, request.User.HomeCoordinate))
            {
                request.DeclineReason = "Координаты базы ошибочны";
                await RewardsCalculator.UpdateUserAfterMissionDecline(request, _missionRepository);
                return new MissionCompletionResult
                           {
                               MissionCompletionStatus = MissionCompletionStatus.Fail,
                               Description = request.DeclineReason
                           };
            }

            request.User.BaseNorthCoordinate = baseCoordinates[0];
            request.User.BaseEastCoordinate = baseCoordinates[1];
            request.User.BaseSouthCoordinate = baseCoordinates[2];
            request.User.BaseWestCoordinate = baseCoordinates[3];            
            request.StarsCount = 3;
            var recievedPoints = await RewardsCalculator.UpdateUserAfterMissionCompletion(request, _missionRepository);
            return new MissionCompletionResult
                       {
                           Points = recievedPoints,
                           StarsCount = request.StarsCount,
                           MissionCompletionStatus = MissionCompletionStatus.Success
                       };
        }

        private bool ValidBaseCoordinates(GeoCoordinate[] baseCoordinates, GeoCoordinate homeCoordinate)
        {
            var fakeUser = new User
                               {
                                   BaseNorthCoordinate = baseCoordinates[0],
                                   BaseEastCoordinate = baseCoordinates[1],
                                   BaseSouthCoordinate = baseCoordinates[2],
                                   BaseWestCoordinate = baseCoordinates[3],
                                   HomeCoordinate = homeCoordinate
                               };

            var northValid = fakeUser.BaseNorthCoordinate.GetDistanceTo(fakeUser.HomeCoordinate) >= MinBorderDistance
                             && fakeUser.BaseNorthCoordinate.GetDistanceTo(fakeUser.HomeCoordinate) <= MaxBorderDistanse
                             && fakeUser.BaseNorthCoordinate.Latitude > fakeUser.HomeCoordinate.Latitude;
            var eastValid = fakeUser.BaseEastCoordinate.GetDistanceTo(fakeUser.HomeCoordinate) >= MinBorderDistance
                            && fakeUser.BaseEastCoordinate.GetDistanceTo(fakeUser.HomeCoordinate) <= MaxBorderDistanse
                            && fakeUser.BaseEastCoordinate.Longitude > fakeUser.HomeCoordinate.Longitude;
            var southValid = fakeUser.BaseSouthCoordinate.GetDistanceTo(fakeUser.HomeCoordinate) >= MinBorderDistance
                             && fakeUser.BaseSouthCoordinate.GetDistanceTo(fakeUser.HomeCoordinate) <= MaxBorderDistanse
                             && fakeUser.BaseSouthCoordinate.Latitude < fakeUser.HomeCoordinate.Latitude;
            var westValid = fakeUser.BaseWestCoordinate.GetDistanceTo(fakeUser.HomeCoordinate) >= MinBorderDistance
                            && fakeUser.BaseWestCoordinate.GetDistanceTo(fakeUser.HomeCoordinate) <= MaxBorderDistanse
                            && fakeUser.BaseWestCoordinate.Longitude < fakeUser.HomeCoordinate.Longitude;
           
            return northValid && eastValid && westValid && southValid;
        }
    }
}