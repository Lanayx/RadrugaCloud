namespace Services.DomainServices
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Core.CommonModels;
    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.Tools;

    using Services.BL;
    using Services.UniqueMissions;

    /// <summary>
    ///     The mission request service.
    /// </summary>
    public sealed class MissionRequestService
    {
        private readonly AppCountersService _appCountersService;

        private readonly ICommonPlacesRepository _commonPlacesRepository;

        private readonly IMissionRepository _missionRepository;

        private readonly IMissionRequestRepository _missionRequestRepository;

        private readonly NotificationService _notificationService;

        private readonly RatingService _ratingService;

        private readonly IUserRepository _userRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionRequestService" /> class.
        /// </summary>
        /// <param name="missionRequestRepository">The mission request repository.</param>
        /// <param name="missionRepository">The mission repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="commonPlacesRepository">The _common places repository.</param>
        /// <param name="ratingService">The rating service.</param>
        /// <param name="notificationService">The notification service.</param>
        /// <param name="appCountersService">The application counters service.</param>
        public MissionRequestService(
            IMissionRequestRepository missionRequestRepository,
            IMissionRepository missionRepository,
            IUserRepository userRepository,
            ICommonPlacesRepository commonPlacesRepository,
            RatingService ratingService,
            NotificationService notificationService,
            AppCountersService appCountersService)
        {
            _missionRequestRepository = missionRequestRepository;
            _missionRepository = missionRepository;
            _userRepository = userRepository;
            _ratingService = ratingService;
            _notificationService = notificationService;
            _appCountersService = appCountersService;
            _commonPlacesRepository = commonPlacesRepository;
        }

        public static async Task<MissionCompletionResult> ProcessIncorrectTry(
            MissionRequest request,
            IMissionRepository missionRepository,
            IMissionRequestRepository missionRequestRepository)
        {
            Expression<Func<MissionRequest, bool>> filter =
                a => a.MissionId == request.MissionId && a.UserId == request.UserId;
            var previousRequiests =
                await missionRequestRepository.GetMissionRequests(new QueryOptions<MissionRequest> { Filter = filter });
            var tryCount = previousRequiests.Count + 1;
            if (tryCount >= request.Mission.TriesFor1Star)
            {
                request.DeclineReason = "Увы, достигнуто максимальное количество попыток.";
                await RewardsCalculator.UpdateUserAfterMissionDecline(request, missionRepository);
                await missionRequestRepository.AddMissionRequest(request);
                return new MissionCompletionResult
                           {
                               MissionCompletionStatus = MissionCompletionStatus.Fail,
                               Description = request.DeclineReason
                           };
            }

            var rejectResult = new MissionCompletionResult
                                   {
                                       MissionCompletionStatus =
                                           MissionCompletionStatus.IntermediateFail,
                                       TryCount = tryCount
                                   };

            var failedRequesResult = await missionRequestRepository.AddMissionRequest(request);
            return failedRequesResult.Status != OperationResultStatus.Success
                       ? MissionCompletionResult.FromOperationResult(failedRequesResult)
                       : rejectResult;
        }

        public static async Task SetStarsAccordingToTries(
            MissionRequest request,
            IMissionRequestRepository missionRequestRepository)
        {
            Expression<Func<MissionRequest, bool>> filter =
                a => a.MissionId == request.MissionId && a.UserId == request.UserId;
            var previousRequiests =
                await missionRequestRepository.GetMissionRequests(new QueryOptions<MissionRequest> { Filter = filter });
            var tries = previousRequiests.Count + 1;
            if (tries <= request.Mission.TriesFor3Stars)
            {
                request.StarsCount = 3;
            }
            else if (tries <= request.Mission.TriesFor2Stars)
            {
                request.StarsCount = 2;
            }
            else if (tries <= request.Mission.TriesFor1Star)
            {
                request.StarsCount = 1;
            }
            else
            {
                throw new Exception("Can't pass mission with more tries than allowed");
            }
        }

        /// <summary>
        ///     The approve request.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="starsCount">The stars count.</param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<OperationResult> ApproveRequest(string id, int starsCount)
        {
            var missionRequest = await _missionRequestRepository.GetMissionRequest(id);
            if (missionRequest == null || missionRequest.Status != MissionRequestStatus.NotChecked)
            {
                return OperationResult.NotFound;
            }

            missionRequest.Status = MissionRequestStatus.Approved;
            missionRequest.StarsCount = (byte)starsCount;

            var result = await _missionRequestRepository.UpdateMissionRequest(missionRequest);
            if (result.Status == OperationResultStatus.Error)
            {
                return result;
            }

            var oldUserPoints = missionRequest.User.Points;
            await
                RewardsCalculator.UpdateUserAfterMissionCompletion(
                    missionRequest,
                    _missionRepository,
                    _appCountersService);

            if (result.Status == OperationResultStatus.Error)
            {
                return result;
            }

            var finalResult = await _userRepository.UpdateUser(missionRequest.User);

            if (finalResult.Status != OperationResultStatus.Error)
            {
                // new points should always have value
                // ReSharper disable once PossibleInvalidOperationException
                await _ratingService.UpdateUserRating(missionRequest.User, oldUserPoints, missionRequest.User.Points.Value);
            }

            await _notificationService.ApproveMissionNotify(missionRequest.UserId, missionRequest.Mission.Name);

            return finalResult;
        }

        /// <summary>
        ///     Completes the mission.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="missionId">The mission id.</param>
        /// <param name="missionProof">The mission proof.</param>
        /// <returns>Task{MissionCompletionResult}.</returns>
        public async Task<MissionCompletionResult> CompleteMission(
            string userId,
            string missionId,
            MissionProof missionProof)
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
            var missionRequest = new MissionRequest
                                     {
                                         Mission = mission,
                                         User = user,
                                         MissionId = missionId,
                                         UserId = userId,
                                         Proof = missionProof,
                                         LastUpdateDate = DateTime.UtcNow,
                                         Status = MissionRequestStatus.AutoApproval
                                     };
            var possibleUniqueMissionType = UniqueMissionsDictionary.GetMissionType(missionRequest.MissionId);

            if (possibleUniqueMissionType != null)
            {
                return
                    await
                    UniqueMissionProcessor.Process(
                        possibleUniqueMissionType,
                        missionRequest,
                        _missionRequestRepository,
                        _userRepository,
                        _ratingService);
            }

            if (mission.ExecutionType == ExecutionType.PhotoCreation
                || mission.ExecutionType == ExecutionType.TextCreation || mission.ExecutionType == ExecutionType.Video)
            {
                missionRequest.Status = MissionRequestStatus.NotChecked;
                var addResult = await _missionRequestRepository.AddMissionRequest(missionRequest);
                if (addResult.Status != OperationResultStatus.Success)
                {
                    return MissionCompletionResult.FromOperationResult(addResult);
                }

                return new MissionCompletionResult { MissionCompletionStatus = MissionCompletionStatus.Waiting };
            }

            return await ProcessAutoApprovedMissions(missionRequest);
        }

        /// <summary>
        ///     The decline request.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<OperationResult> DeclineRequest(string id, string message)
        {
            var missionRequest = await _missionRequestRepository.GetMissionRequest(id);
            if (missionRequest == null || missionRequest.Status != MissionRequestStatus.NotChecked)
            {
                return OperationResult.NotFound;
            }

            missionRequest.Status = MissionRequestStatus.Declined;
            missionRequest.DeclineReason = message;

            var result = await _missionRequestRepository.UpdateMissionRequest(missionRequest);
            if (result.Status == OperationResultStatus.Error)
            {
                return result;
            }

            var oldUserPoints = missionRequest.User.Points;
            await
                RewardsCalculator.UpdateUserAfterMissionDecline(missionRequest, _missionRepository, _appCountersService);

            if (result.Status == OperationResultStatus.Error)
            {
                return result;
            }

            var finalResult = await _userRepository.UpdateUser(missionRequest.User);
            if (finalResult.Status != OperationResultStatus.Error)
            {
                // new points should always have value
                // ReSharper disable once PossibleInvalidOperationException
                await _ratingService.UpdateUserRating(missionRequest.User, oldUserPoints, missionRequest.User.Points.Value);
            }
            await _notificationService.DeclineMissionNotify(missionRequest.UserId, missionRequest.Mission.Name);

            return finalResult;
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            _missionRequestRepository.Dispose();
        }

        /// <summary>
        ///     The get request.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task<MissionRequest> GetRequest(string id)
        {
            return _missionRequestRepository.GetMissionRequest(id);
        }

        /// <summary>
        ///     The get requests.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<List<MissionRequest>> GetRequests(QueryOptions<MissionRequest> options = null)
        {
            return _missionRequestRepository.GetMissionRequests(options);
        }

        private static bool CheckAllWordsAnswer(
            string userAnswer,
            IEnumerable<AnswerModel> allWordAnswers,
            bool exactAnswer,
            out AnswerModel validAnswer)
        {
            validAnswer = null;
            var splittedUserAnswer = userAnswer.SplitStringByDelimiter(" ");
            if (splittedUserAnswer.Count == 1)
            {
                return false;
            }

            foreach (var answer in allWordAnswers)
            {
                var rightWords = answer.AllWordsInside.Select(a => a).ToIList();
                if (!rightWords.Any())
                {
                    continue;
                }

                foreach (var userAnswerWord in splittedUserAnswer.TakeWhile(userAnswerWord => rightWords.Any()))
                {
                    string rightWord;
                    if (exactAnswer)
                    {
                        rightWord =
                            rightWords.FirstOrDefault(
                                w => string.Equals(w, userAnswerWord, StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        rightWord =
                            rightWords.FirstOrDefault(
                                w => userAnswerWord.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
                    }

                    if (!string.IsNullOrEmpty(rightWord))
                    {
                        rightWords.Remove(rightWord);
                    }
                }

                if (!rightWords.Any())
                {
                    validAnswer = answer;
                    break;
                }
            }

            return validAnswer != null;
        }

        private static bool CheckAlternativeAnswer(
            string userAnswer,
            IEnumerable<AnswerModel> alternativeAnswers,
            bool exactAnswer,
            out AnswerModel validAnswer)
        {
            foreach (var alternativeAnswer in alternativeAnswers)
            {
                var singleAnswers =
                    alternativeAnswer.Alternatives.Where(a => a.Type == AnswerType.SingleAnswer);
                var answerValid = CheckSingleAnswer(userAnswer, singleAnswers, exactAnswer, out validAnswer);
                if (answerValid)
                {
                    validAnswer = alternativeAnswer;
                    return true;
                }

                var allWordAnswers =
                    alternativeAnswer.Alternatives.Where(a => a.Type == AnswerType.AllWordsInside);
                answerValid = CheckAllWordsAnswer(userAnswer, allWordAnswers, exactAnswer, out validAnswer);
                if (answerValid)
                {
                    validAnswer = alternativeAnswer;
                    return true;
                }
            }

            validAnswer = null;
            return false;
        }

        private static bool CheckSingleAnswer(
            string userAnswer,
            IEnumerable<AnswerModel> singleAnswers,
            bool exactAnswer,
            out AnswerModel validAnswer)
        {
            if (exactAnswer)
            {
                validAnswer =
                    singleAnswers.FirstOrDefault(
                        a => string.Equals(a.SingleAnswer, userAnswer, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                validAnswer =
                    singleAnswers.FirstOrDefault(
                        a => userAnswer.IndexOf(a.SingleAnswer, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            return validAnswer != null;
        }

        private static int ParseUserAnswers(
            List<AnswerModel> rightAnswers,
            List<string> userAnswers,
            bool exactAnswer,
            out List<string> userAnswerStatuses)
        {
            var usedRightAnswers = new List<AnswerModel>();
            userAnswerStatuses = new List<string>();
            var incorrectAnswers = 0;
            const string InvalidStatus = "invalid";
            const string ValidStatus = "valid";

            foreach (var userAnswer in userAnswers)
            {
                AnswerModel validAnswer;
                var singleAnswers = rightAnswers.Where(a => a.Type == AnswerType.SingleAnswer);
                var answerValid = CheckSingleAnswer(userAnswer, singleAnswers, exactAnswer, out validAnswer);
                if (answerValid)
                {
                    if (usedRightAnswers.Contains(validAnswer))
                    {
                        userAnswerStatuses.Add(InvalidStatus);
                        incorrectAnswers++;
                    }
                    else
                    {
                        usedRightAnswers.Add(validAnswer);
                        userAnswerStatuses.Add(ValidStatus);
                    }
                    continue;
                }

                var allWordAnswers = rightAnswers.Where(a => a.Type == AnswerType.AllWordsInside);
                answerValid = CheckAllWordsAnswer(userAnswer, allWordAnswers, exactAnswer, out validAnswer);
                if (answerValid)
                {
                    if (usedRightAnswers.Contains(validAnswer))
                    {
                        userAnswerStatuses.Add(InvalidStatus);
                        incorrectAnswers++;
                    }
                    else
                    {
                        usedRightAnswers.Add(validAnswer);
                        userAnswerStatuses.Add(ValidStatus);
                    }

                    continue;
                }

                var alternativeAnswers = rightAnswers.Where(a => a.Type == AnswerType.Alternatives);
                answerValid = CheckAlternativeAnswer(userAnswer, alternativeAnswers, exactAnswer, out validAnswer);
                if (answerValid)
                {
                    if (usedRightAnswers.Contains(validAnswer))
                    {
                        userAnswerStatuses.Add(InvalidStatus);
                        incorrectAnswers++;
                    }
                    else
                    {
                        usedRightAnswers.Add(validAnswer);
                        userAnswerStatuses.Add(ValidStatus);
                    }
                    continue;
                }

                incorrectAnswers++;
                userAnswerStatuses.Add(InvalidStatus);

            }

            return incorrectAnswers;
        }

        private static void SetStarsAccordingToTime(MissionRequest request)
        {
            if (!request.Proof.TimeElapsed.HasValue)
            {
                request.StarsCount = 0;
                return;
            }

            var time = request.Proof.TimeElapsed;
            if (time <= request.Mission.SecondsFor3Stars)
            {
                request.StarsCount = 3;
            }
            else if (time <= request.Mission.SecondsFor2Stars)
            {
                request.StarsCount = 2;
            }
            else if (time <= request.Mission.SecondsFor1Star)
            {
                request.StarsCount = 1;
            }
            else
            {
                request.StarsCount = 0;
            }
        }

        private async Task<MissionCompletionResult> ProcessAutoApprovedMissions(MissionRequest missionRequest)
        {
            var oldUserPoints = missionRequest.User.Points;
            MissionCompletionResult result;
            switch (missionRequest.Mission.ExecutionType)
            {
                case ExecutionType.RightAnswer:
                    {
                        result = await ProcessRightAnswer(missionRequest);
                        break;
                    }

                case ExecutionType.Path:
                    {
                        result = await ProcessGeoCoordinate(missionRequest);
                        break;
                    }

                case ExecutionType.CommonPlace:
                    {
                        result = await ProcessCommonPlace(missionRequest);
                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(missionRequest), "Incorrect execution type");
                    }
            }

            if (result.Status != OperationResultStatus.Success
                || result.MissionCompletionStatus == MissionCompletionStatus.IntermediateFail)
            {
                return result;
            }

            //after any mission completion user should be updated
            var userUpdateResult = await _userRepository.UpdateUser(missionRequest.User);
            if (userUpdateResult.Status != OperationResultStatus.Error)
            {
                // new points should always have value
                // ReSharper disable once PossibleInvalidOperationException
                await _ratingService.UpdateUserRating(missionRequest.User, oldUserPoints, missionRequest.User.Points.Value);
            }

            return userUpdateResult.Status == OperationResultStatus.Success
                       ? result
                       : MissionCompletionResult.FromOperationResult(userUpdateResult);
        }

        private async Task<MissionCompletionResult> ProcessCommonPlace(MissionRequest request)
        {
            var commonPlaceAlias = request.Mission.CommonPlaceAlias;
            var passedCoordinate = request.Proof.Coordinates.First();
            if (request.User.HomeCoordinate.GetDistanceTo(passedCoordinate) < Core.Constants.GameConstants.Mission.CommonPlaceMinDistanceFromHome) {
                var incorrectResult = await ProcessIncorrectTry(request, _missionRepository, _missionRequestRepository);
                if (incorrectResult.MissionCompletionStatus == MissionCompletionStatus.IntermediateFail)
                    incorrectResult.Description = "StillHome";
                return incorrectResult;
            }

            var realCommonPlace = await _commonPlacesRepository.GetCommonPlaceByAlias(request.UserId, commonPlaceAlias);
            if (realCommonPlace == null)
            {
                return await ProcessTemporaryCommonPlace(request, commonPlaceAlias, passedCoordinate);
            }

            var distance = passedCoordinate.GetDistanceTo(realCommonPlace.Coordinate);
            if (distance > request.Mission.AccuracyRadius)
            {
                var incorrectResult = await ProcessIncorrectTry(request, _missionRepository, _missionRequestRepository);
                if (incorrectResult.MissionCompletionStatus == MissionCompletionStatus.IntermediateFail)
                    incorrectResult.Description = distance > request.Mission.AccuracyRadius * 2 ? "Incorrect" : "IsNear";
                return incorrectResult;
            }

            await SetStarsAccordingToTries(request, _missionRequestRepository);
            var completePoints =
                await
                RewardsCalculator.UpdateUserAfterMissionCompletion(request, _missionRepository, _appCountersService);
            var finalResult = new MissionCompletionResult
                                  {
                                      Points = completePoints,
                                      StarsCount = request.StarsCount,
                                      MissionCompletionStatus = MissionCompletionStatus.Success
                                  };

            var requestResult = await _missionRequestRepository.AddMissionRequest(request);
            return requestResult.Status != OperationResultStatus.Success
                       ? MissionCompletionResult.FromOperationResult(requestResult)
                       : finalResult;
        }

        private async Task<MissionCompletionResult> ProcessGeoCoordinate(MissionRequest request)
        {
            SetStarsAccordingToTime(request);
            var missionFailed = request.StarsCount == 0;
            int? recievedPoints = null;
            if (missionFailed)
            {
                await RewardsCalculator.UpdateUserAfterMissionDecline(request, _missionRepository, _appCountersService);
            }
            else
            {
                recievedPoints = await RewardsCalculator.UpdateUserAfterMissionCompletion(
                                         request,
                                         _missionRepository,
                                         _appCountersService);
            }


            var requestResult = await _missionRequestRepository.AddMissionRequest(request);
            if (requestResult.Status == OperationResultStatus.Success)
            {
                return new MissionCompletionResult
                           {
                               Points = recievedPoints,
                               StarsCount = request.StarsCount,
                               MissionCompletionStatus =
                                   missionFailed
                                       ? MissionCompletionStatus.Fail
                                       : MissionCompletionStatus.Success
                           };
            }

            return MissionCompletionResult.FromOperationResult(requestResult);
        }

        private async Task<RightAnswerMissionCompletionResult> ProcessRightAnswer(MissionRequest request)
        {
            var userAnswers = request.Proof.CreatedText.Trim().SplitStringByDelimiter();
            if (!userAnswers.AnyValues())
            {
                return new RightAnswerMissionCompletionResult(OperationResultStatus.Error, "Ответ не может быть пустым");
            }

            var rightAnswers = AnswerModelHelper.SplitAnswer(request.Mission.CorrectAnswers);
            List<string> userAnswerStatuses;
            var invalidAnswersCount = ParseUserAnswers(rightAnswers, userAnswers, request.Mission.ExactAnswer ?? false , out userAnswerStatuses);
            var rightAnswersCount = userAnswers.Count - invalidAnswersCount;
            if (rightAnswersCount == 0 || rightAnswersCount < request.Mission.AnswersCount)
            {
                var result = await ProcessIncorrectTry(request, _missionRepository, _missionRequestRepository);
                var rightAnswerResult = RightAnswerMissionCompletionResult.FromMissionCompletionResult(result);
                request.DeclineReason = userAnswerStatuses.JoinToString();
                rightAnswerResult.AnswerStatuses = userAnswerStatuses;

                return rightAnswerResult;
            }

            await SetStarsAccordingToTries(request, _missionRequestRepository);
            var recievedPoints =
                await
                RewardsCalculator.UpdateUserAfterMissionCompletion(request, _missionRepository, _appCountersService);
            var requestResult = await _missionRequestRepository.AddMissionRequest(request);
            if (requestResult.Status != OperationResultStatus.Success)
            {
                return
                    RightAnswerMissionCompletionResult.FromMissionCompletionResult(
                        MissionCompletionResult.FromOperationResult(requestResult));
            }

            return new RightAnswerMissionCompletionResult
                       {
                           Points = recievedPoints,
                           StarsCount = request.StarsCount,
                           MissionCompletionStatus = MissionCompletionStatus.Success
                       };
        }

        private async Task<MissionCompletionResult> ProcessTemporaryCommonPlace(
            MissionRequest request,
            string commonPlaceAlias,
            GeoCoordinate passedCoordinate)
        {
            var result =
                await _commonPlacesRepository.AddCommonPlace(request.UserId, commonPlaceAlias, passedCoordinate);
            if (result.Status != OperationResultStatus.Success)
            {
                return new MissionCompletionResult(
                    OperationResultStatus.Error,
                    "Невозможно добавить временное общее место");
            }

            request.StarsCount = 3;
            var recievedPoints =
                await
                RewardsCalculator.UpdateUserAfterMissionCompletion(request, _missionRepository, _appCountersService);
            var requestResult = await _missionRequestRepository.AddMissionRequest(request);
            if (requestResult.Status != OperationResultStatus.Success)
            {
                return MissionCompletionResult.FromOperationResult(requestResult);
            }

            return new MissionCompletionResult
                       {
                           Points = recievedPoints,
                           StarsCount = request.StarsCount,
                           MissionCompletionStatus = MissionCompletionStatus.Success
                       };
        }
    }
}