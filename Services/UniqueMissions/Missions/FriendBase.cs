namespace Services.UniqueMissions.Missions
{
    using System.Device.Location;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Models;
    using Core.Interfaces.Repositories;

    using Services.BL;
    using Services.DomainServices;

    /// <summary>
    ///     Class FriendBase
    /// </summary>
    public class FriendBase : IUniqueMission
    {
        private readonly AppCountersService _appCountersService;

        private readonly IMissionRepository _missionRepository;

        private readonly IUserRepository _userRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FriendBase" /> class.
        /// </summary>
        /// <param name="missionRepository">The mission repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="appCountersService">The application counters service.</param>
        public FriendBase(
            IMissionRepository missionRepository,
            IUserRepository userRepository,
            AppCountersService appCountersService)
        {
            _missionRepository = missionRepository;
            _userRepository = userRepository;
            _appCountersService = appCountersService;
        }

        /// <summary>
        ///     Processes the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task{MissionCompletionResult}.</returns>
        public async Task<MissionCompletionResult> ProcessRequest(MissionRequest request)
        {
            var users = await _userRepository.GetUsers(new QueryOptions<User> { Filter = u => u.Id != request.UserId, Select = u => u.HomeCoordinate});
            var coordinates = users.Select(u => u.HomeCoordinate).Where(coord => coord != null);
            var passedCoordinate = request.Proof.Coordinates.First();
            GeoCoordinate friendsBaseCoordinate = null;
            foreach (var geoCoordinate in coordinates)
            {
                var distance = passedCoordinate.GetDistanceTo(geoCoordinate);

                if (distance <= 100)
                {
                    request.StarsCount = 1;
                    if (distance <= 50)
                    {
                        request.StarsCount = 2;
                    }
                    if (distance <= 10)
                    {
                        request.StarsCount = 3;
                    }
                    friendsBaseCoordinate = geoCoordinate;
                    break;
                }
            }

            if (friendsBaseCoordinate == null)
            {
                request.DeclineReason = "Мы не смогли найти ни одного пользователя с таким командным пунктом";
                await RewardsCalculator.UpdateUserAfterMissionDecline(request, _missionRepository, _appCountersService);
                return new MissionCompletionResult
                           {
                               MissionCompletionStatus = MissionCompletionStatus.Fail,
                               Description = request.DeclineReason
                           };
            }

            var recievedPoints =
                await
                RewardsCalculator.UpdateUserAfterMissionCompletion(request, _missionRepository, _appCountersService);
            return new MissionCompletionResult
                       {
                           Points = recievedPoints,
                           StarsCount = request.StarsCount,
                           MissionCompletionStatus = MissionCompletionStatus.Success
                       };
        }
    }
}