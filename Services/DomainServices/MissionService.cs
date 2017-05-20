namespace Services.DomainServices
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;
    using Core.Tools;
    using Core.Tools.CopyHelper;

    using Services.BL;
    using Services.UniqueMissions;
    using Services.UniqueMissions.Missions;

    /// <summary>
    ///     Class MissionService
    /// </summary>
    public sealed class MissionService
    {
        private readonly IMissionRepository _missionRepository;

        private readonly IMissionRequestRepository _missionRequestRepository;

        private readonly IHintRequestRepository _hintRequestRepository;

        private readonly IMissionSetRepository _missionSetRepository;

        private readonly RatingService _ratingService;

        private readonly IUserRepository _userRepository;

        private readonly ICommonPlacesRepository _commonPlacesRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionService" /> class.
        /// </summary>
        /// <param name="missionRepository">The mission repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="missionSetRepository">The mission set repository.</param>
        /// <param name="missionRequestRepository">The mission request repository.</param>
        /// <param name="hintRequestRepository">The hint request repository.</param>
        /// <param name="ratingService">The rating service.</param>
        /// <param name="commonPlacesRepository">The common places repository.</param>
        public MissionService(
            IMissionRepository missionRepository,
            IUserRepository userRepository,
            IMissionSetRepository missionSetRepository,
            IMissionRequestRepository missionRequestRepository,
            IHintRequestRepository hintRequestRepository,
            RatingService ratingService,
            ICommonPlacesRepository commonPlacesRepository)
        {
            _missionRepository = missionRepository;
            _userRepository = userRepository;
            _ratingService = ratingService;
            _missionSetRepository = missionSetRepository;
            _missionRequestRepository = missionRequestRepository;
            _hintRequestRepository = hintRequestRepository;
            _commonPlacesRepository = commonPlacesRepository;
        }

        /// <summary>
        ///     Adds the new mission.
        /// </summary>
        /// <param name="mision">The mision.</param>
        /// <returns>Task{AddResult}.</returns>
        public Task<IdResult> AddNewMission(Mission mision)
        {
            return _missionRepository.AddMission(mision);
        }

        /// <summary>
        ///     Declines the mission.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="missionId">The mission identifier.</param>
        /// <returns>
        ///     Task{OperationResult}.
        /// </returns>
        public async Task<MissionCompletionResult> DeclineMission(string userId, string missionId)
        {
            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return new MissionCompletionResult(OperationResultStatus.Error, "User was not found");
            }
            var currentMission = user.ActiveMissionIds.FirstOrDefault(m => m.MissionId == missionId);
            if (currentMission == null)
            {
                return new MissionCompletionResult(OperationResultStatus.Error, "Mission was not found");
            }

            var mission = await _missionRepository.GetMission(missionId);

            //Check if mission is unique and sensored
            var possibleUniqueMissionType = UniqueMissionsDictionary.GetMissionType(missionId);
            if (possibleUniqueMissionType == typeof(Censored))
            {
                return
                    await
                    UniqueMissionProcessor.Process(
                        possibleUniqueMissionType,
                        new MissionRequest
                        {
                            Mission = mission,
                            MissionId = mission.Id,
                            User = user,
                            UserId = user.Id,
                            Status = MissionRequestStatus.AutoApproval,
                            LastUpdateDate = DateTime.UtcNow,
                            Proof = new MissionProof()
                        },
                        _missionRequestRepository,
                        _userRepository,
                        _ratingService);
            }

            var oldPoints = user.Points;

            await RewardsCalculator.UpdateUserAfterMissionDecline(
                    new MissionRequest { User = user, Mission = mission },
                    _missionRepository);

            var userUpdateResult = await _userRepository.UpdateUser(user);
            if (userUpdateResult.Status != OperationResultStatus.Error)
            {
                // new points should always have value
                // ReSharper disable once PossibleInvalidOperationException
                await _ratingService.UpdateUserRating(user, oldPoints, user.Points.Value);
            }
            return userUpdateResult.Status == OperationResultStatus.Success
                       ? new MissionCompletionResult
                       {
                           MissionCompletionStatus = MissionCompletionStatus.Fail
                       }
                       : MissionCompletionResult.FromOperationResult(userUpdateResult);
        }

        /// <summary>
        ///     Deletes the mission.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{OperationResult}.</returns>
        public Task<OperationResult> DeleteMission(string id)
        {
            //TODO remove mission id from users' mission lists
            return _missionRepository.DeleteMission(id);
        }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            _missionRepository.Dispose();
        }

        /// <summary>
        ///     Gets the attached mission sets.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>Task{IEnumerable{UserMissionSet}}.</returns>
        public async Task<IEnumerable<UserMissionSet>> GetActiveMissionSets(string userId)
        {
            var user = await _userRepository.GetUser(userId);
            if (!user.ActiveMissionSetIds.AnyValues())
            {
                return null;
            }

            var activeSets =
                (await
                _missionSetRepository.GetMissionSets(
                    new QueryOptions<MissionSet>
                    {
                        Filter =
                                ExpressionHelper.Expr((MissionSet x) => x.Id)
                                .In(user.ActiveMissionSetIds),
                        Expand = new List<string> { "Missions" }
                    }));

            var setsMissions = activeSets.SelectMany(attachedSet => attachedSet.Missions).Select(m => m.Mission).ToIList();
            var missionRequests =
                (await
                 _missionRequestRepository.GetMissionRequests(
                     new QueryOptions<MissionRequest> { Filter = mr => mr.UserId == userId })).Where(
                         missionRequest => setsMissions.Select(m => m.Id).Contains(missionRequest.MissionId)).ToIList();

            var activeUserSets = new List<UserMissionSet>();
            foreach (var missionSet in activeSets)
            {
                var userSet = new UserMissionSet { Id = missionSet.Id, Name = missionSet.Name };
                var missions = new List<UserMission>();

                foreach (var mission in missionSet.Missions.OrderBy(m => m.Order).Select(m => m.Mission))
                {
                    var convertedMission = ConvertToUserMission(mission, user);
                    missions.Add(convertedMission);
                }
                userSet.Missions = missions;
                activeUserSets.Add(userSet);
            }

            SetDisplayStatuses(activeUserSets, user, missionRequests, setsMissions);

            return activeUserSets.OrderBy(set => user.MissionSetIds.First(setId => setId.MissionSetId == set.Id).Order);
        }

        /// <summary>
        ///     Gets the mission.
        /// </summary>
        /// <param name="missionId">The mission id.</param>
        /// <returns>Task{Mission}.</returns>
        public async Task<Mission> GetMission(string missionId)
        {
            return await _missionRepository.GetMission(missionId);
        }

        /// <summary>
        ///     Gets the missions.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{Mission}}.</returns>
        public Task<List<Mission>> GetMissions(QueryOptions<Mission> options = null)
        {
            return _missionRepository.GetMissions(options);
        }

        /// <summary>
        ///     Updates the mission.
        /// </summary>
        /// <param name="mision">The mision.</param>
        /// <returns>Task{OperationResult}.</returns>
        public Task<OperationResult> UpdateMission(Mission mision)
        {
            return _missionRepository.UpdateMission(mision);
        }



        /// <summary>
        /// Adds the hint to database.
        /// </summary>
        /// <param name="hintRequest">The hint request.</param>
        /// <returns></returns>
        public async Task<IdResult> AddHintRequest(HintRequest hintRequest)
        {
            return await _hintRequestRepository.AddHintRequest(hintRequest);
        }

        /// <summary>
        /// Checks user payment for hint.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<bool> CheckPayment(string userId)
        {
            var user = await _userRepository.GetUser(userId);
            // if (userId == "User1Id")
            // {
            return await Task.FromResult(true);
            // }
            // return await Task.FromResult(false);
        }

        /// <summary>
        /// Gets the hint to user.
        /// </summary>
        /// <param name="missionId">The mission identifier.</param>
        /// <param name="hintId">The hint identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<HintRequestResult> GetHint(string missionId, string hintId, string userId)
        {
            var hintRequest = new HintRequest
            {
                HintId = hintId,
                MissionId = missionId,
                UserId = userId,
                Status = HintRequestStatus.Success
            };

            var user = await _userRepository.GetUser(userId);
            if (!user.ActiveMissionIds.AnyValues(t => t.MissionId == missionId))
            {
                hintRequest.Status = HintRequestStatus.UserDontHaveThatMissionInActiveStatus;
                return await LogHintRequestAndReturnResult(hintRequest, OperationResultStatus.Error, "Mission is inactive");
            }
            var mission = await GetMission(missionId);
            var hint = mission.Hints?.SingleOrDefault(h => h.Id == hintId);
            if (hint == null)
            {
                hintRequest.Status = HintRequestStatus.HintNotFound;
                return await LogHintRequestAndReturnResult(hintRequest, OperationResultStatus.Error, "Invalid hint Id");
            }

            string hintText;
            switch (hint.Type)
            {
                case HintType.Coordinate:
                case HintType.Direction:
                    {
                        var commonPlaceAlias = mission.CommonPlaceAlias;
                        var realCommonPlace = await _commonPlacesRepository.GetCommonPlaceByAlias(userId, commonPlaceAlias);
                        if (realCommonPlace == null)
                        {
                            hintRequest.Status = HintRequestStatus.CommonPlaceNotExist;
                            return await LogHintRequestAndReturnResult(hintRequest, OperationResultStatus.Warning, "Common place is not fixed");
                        }
                        hintText = realCommonPlace.Coordinate.ToString();
                        break;
                    }
                case HintType.Text:
                    {
                        hintText = hint.Text;
                        break;
                    }
                default:
                    throw new Exception("Invalid hint type");
            }

            if (!user.BoughtHintIds.AnyValues(t => t == hint.Id))
            {
                if (!user.CoinsCount.HasValue || user.CoinsCount < hint.Score)
                {
                    hintRequest.Status = HintRequestStatus.UserDontHaveCoins;
                    return await LogHintRequestAndReturnResult(hintRequest, OperationResultStatus.Error, "Not enough coins");
                }
                user.CoinsCount = user.CoinsCount - hint.Score;
                user.BoughtHintIds = user.BoughtHintIds ?? new List<string>();
                user.BoughtHintIds.Add(hint.Id);
                await _userRepository.UpdateUser(user);
            }
            return await LogHintRequestAndReturnResult(hintRequest, OperationResultStatus.Success, hintText);
        }

        private async Task<HintRequestResult> LogHintRequestAndReturnResult(HintRequest hintRequest, OperationResultStatus operationResultStatus, string hintText)
        {
            await AddHintRequest(hintRequest);
            return new HintRequestResult(operationResultStatus, hintRequest.Status, hintText);
        }

        private static void SetFailSuccessWaitingStatuses(
            List<UserMissionSet> userSets,
            User user,
            IList<MissionRequest> missionRequests)
        {
            foreach (var userMissionSet in userSets)
            {
                foreach (var userMission in userMissionSet.Missions)
                {
                    if (user.FailedMissionIds.AnyValues() && user.FailedMissionIds.Contains(userMission.Id))
                    {
                        userMission.DisplayStatus = MissionDisplayStatus.Fail;
                    }
                    else if (user.CompletedMissionIds.AnyValues() && user.CompletedMissionIds.Contains(userMission.Id))
                    {
                        userMission.DisplayStatus = MissionDisplayStatus.Success;
                    }
                    else if (missionRequests.AnyValues())
                    {
                        var mission = userMission;
                        var missionRequestsForMission = missionRequests.Where(mr => mr.MissionId == mission.Id).ToIList();
                        if (missionRequestsForMission.Any(mr => mr.Status == MissionRequestStatus.NotChecked))
                        {
                            userMission.DisplayStatus = MissionDisplayStatus.Waiting;
                        }
                        else
                        {
                            userMission.DisplayStatus = MissionDisplayStatus.Available;
                            userMission.TryCount = missionRequestsForMission.Count;
                        }
                    }
                    else
                    {
                        userMission.DisplayStatus = MissionDisplayStatus.Available;
                    }
                }
            }
        }

        private UserMission ConvertToUserMission(Mission missionDomain, User user)
        {
            if (missionDomain == null)
            {
                return null;
            }

            var mission = new UserMission();
            missionDomain.CopyTo(mission);

            var missionHints = new List<UserMissionHint>();
            if (missionDomain.Hints != null)
            {
                foreach (var hint in missionDomain.Hints)
                {
                    var isPayed = user.BoughtHintIds.AnyValues(t => t == hint.Id);
                    missionHints.Add(new UserMissionHint
                    {
                        Id = hint.Id,
                        Score = hint.Score,
                        Type = hint.Type,
                        Text = isPayed ? hint.Text : string.Empty,
                        IsPayed = isPayed
                    });
                }
            }
            mission.Hints = missionHints;

            if (missionDomain.ExecutionType != ExecutionType.Path)
            {
                return mission;
            }

            var paramsCount = missionDomain.CalculationFunctionParameters.Count;
            var sb = new StringBuilder("function(");
            for (var i = 1; i <= paramsCount; i++)
            {
                if (i != 1)
                {
                    sb.Append(",");
                }

                sb.Append($"param{i}");
            }
            sb.Append("){");
            sb.AppendLine(missionDomain.UserCoordinatesCalculationFunction);
            sb.AppendLine("}");
            mission.CoordinatesCalculationFunction = sb.ToString();
            mission.CalculationFunctionParameters =
                missionDomain.CalculationFunctionParameters.Select(c => c.Substring(2).ToCamelCase());
            return mission;
        }

        private void SetDisplayStatuses(
            List<UserMissionSet> activeUserSets,
            User user,
            IList<MissionRequest> missionRequests,
            IList<Mission> missions)
        {

            SetFailSuccessWaitingStatuses(activeUserSets, user, missionRequests);
            var allActiveMissions = activeUserSets.SelectMany(us => us.Missions).ToList();

            var updatedAvailableMissions =
                allActiveMissions.Where(m => m.DisplayStatus == MissionDisplayStatus.Available).ToIList();

            foreach (var availableMission in updatedAvailableMissions)
            {
                var dependOn = missions.First(m => m.Id == availableMission.Id).DependsOn;
                if (!dependOn.AnyValues())
                {
                    continue;
                }
                foreach (var missionId in dependOn)
                {
                    if (user.FailedMissionIds != null && user.FailedMissionIds.Contains(missionId))
                    {
                        availableMission.DisplayStatus = MissionDisplayStatus.Fail;
                        break;
                    }
                    if (user.CompletedMissionIds == null || !user.CompletedMissionIds.Contains(missionId))
                    {
                        availableMission.DisplayStatus = MissionDisplayStatus.NotAvailable;
                        break;
                    }
                }

                var missionsDependsOn = 
                    allActiveMissions.Where(m => m.DisplayStatus != MissionDisplayStatus.Success && m.DisplayStatus != MissionDisplayStatus.Fail)
                        .Where(m => dependOn.Contains(m.Id));
                foreach (var missionDependsOn in missionsDependsOn)
                {
                    missionDependsOn.DependentMissionIds = missionDependsOn.DependentMissionIds ?? new List<string>();
                    missionDependsOn.DependentMissionIds.Add(availableMission.Id);
                }
            }
        }
    }
}