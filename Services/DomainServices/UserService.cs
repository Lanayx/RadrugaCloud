namespace Services.DomainServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.Constants;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;

    using Services.BL;
    using Core.Tools;

    /// <summary>
    ///     Service for user interactions
    /// </summary>
    public sealed class UserService
    {
        private readonly AppCountersService _appCountersService;

        private readonly IMissionRepository _missionRepository;

        private readonly IRatingRepository _ratingRepository;

        /// <summary>
        ///     The _user repository
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserService" /> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="missionRepository">The mission repository.</param>
        /// <param name="ratingRepository">The rating repository.</param>
        /// <param name="appCountersService">The application counters service.</param>
        public UserService(
            IUserRepository userRepository,
            IMissionRepository missionRepository,
            IRatingRepository ratingRepository,
            AppCountersService appCountersService)
        {
            _userRepository = userRepository;
            _ratingRepository = ratingRepository;
            _missionRepository = missionRepository;
            _appCountersService = appCountersService;
        }

        /// <summary>
        ///     Adds the new user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{AddResult}.</returns>
        public async Task<IdResult> AddNewUser(User user)
        {
            var existingUser = await GetUser(user.Id);
            if (existingUser != null)
            {
                return new IdResult(OperationResultStatus.Warning, "User exists");
            }

            SetDefaultFields(user);
            await SetStartMissions(user);
            var result = await _userRepository.AddUser(user);
            return result;
        }

        /// <summary>
        ///     Changes the name of the nick.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="nickName">Name of the nick.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> ChangeNickName(string userId, string nickName)
        {
            var user = new User { Id = userId, NickName = nickName };
            var result = await _userRepository.UpdateUser(user, false);
            if (result.Status != OperationResultStatus.Error)
            {
                await _ratingRepository.UpdateNickname(user);
            }
            return result;
        }

        /// <summary>
        ///     Changes the notifications settings.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="notificationsEnabled">if set to <c>true</c> [notifications enabled].</param>
        /// <returns></returns>
        public async Task<OperationResult> ChangeNotificationsSettings(string userId, bool notificationsEnabled)
        {
            var user = new User { Id = userId, EnablePushNotifications = notificationsEnabled };
            return await _userRepository.UpdateUser(user, false);
        }  

        /// <summary>
        ///     Updates the user avatar.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="avatarUrl">The avatar URL.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> ChangeUserAvatar(string userId, string avatarUrl)
        {
            var user = new User { Id = userId, AvatarUrl = avatarUrl };
            var result = await _userRepository.UpdateUser(user, false);
            if (result.Status != OperationResultStatus.Error)
            {
                await _ratingRepository.UpdateAvatar(user);
            }
            return result;
        }

        /// <summary>
        ///     Decreases the kind action scales for all users
        /// </summary>
        /// <returns></returns>
        public Task<OperationResult> DecreaseKindActionScales()
        {
            return _userRepository.DecreaseKindActionScales();
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            _userRepository.Dispose();
        }

        /// <summary>
        /// Updates the color of the radruga. (for development only)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task UpdateRadrugaColor()
        {
            var users = await _userRepository.GetUsers(new QueryOptions<User>() { Expand = new List<string> { "PersonQualities" } });
            var rnd = new Random();
            foreach (var user in users)
            {
                if (!String.IsNullOrEmpty(user.RadrugaColor))
                {
                    if (!user.PersonQualitiesWithScores.AnyValues(pq => pq.PersonQualityId == GameConstants.PersonQuality.ActivityQualityId)
                        && !user.PersonQualitiesWithScores.AnyValues(pq => pq.PersonQualityId == GameConstants.PersonQuality.CommunicationQualityId))
                    {
                        user.PersonQualitiesWithScores = user.PersonQualitiesWithScores ?? new List<PersonQualityIdWithScore>();
                        user.PersonQualitiesWithScores.Add(new PersonQualityIdWithScore() { PersonQualityId = GameConstants.PersonQuality.ActivityQualityId, Score = rnd.Next(-30, 30) });
                        user.PersonQualitiesWithScores.Add(new PersonQualityIdWithScore() { PersonQualityId = GameConstants.PersonQuality.CommunicationQualityId, Score = rnd.Next(-30, 30) });
                    }
                }

                RewardsCalculator.UpdateRadrugaColor(user, true);
                await _userRepository.UpdateUser(user);
            }
        }

        /// <summary>
        ///     Gets the achievements.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<List<Achievement>> GetAchievements(string userId)
        {
            var user = await GetUser(userId);
            return user.GetAchievements();
        }

        /// <summary>
        ///     Gets the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task{User}.</returns>
        public Task<User> GetUser(string id)
        {
            return _userRepository.GetUser(id);
        }

        /// <summary>
        ///     Increases the vk repost count.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<OperationResult> IncreaseVkRepostCount(string userId)
        {
            var user = await GetUser(userId);
            user.VkRepostCount = user.VkRepostCount.HasValue ? user.VkRepostCount + 1 : 1;
            //TODO run in parallel
            await _appCountersService.VkRepost();
            return await _userRepository.UpdateUser(user);
        }

        /// <summary>
        ///     Updates the last ratings places for all users.
        /// </summary>
        /// <returns></returns>
        public Task<OperationResult> UpdateLastRatingsPlaces()
        {
            return _userRepository.UpdateLastRatingsPlaces();
        }

        /// <summary>
        ///     Adds the new user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{AddResult}.</returns>
        public async Task<OperationResult> UpdateUser(User user)
        {
            var result = await _userRepository.UpdateUser(user);
            if (result.Status != OperationResultStatus.Error)
            {
                await _ratingRepository.UpdateNickname(user);
            }
            return result;
        }

        private void SetDefaultFields(User user)
        {
            user.KindScale = 0;
            user.Level = 1;
            user.LevelPoints = 0;
            user.EnablePushNotifications = false;
            user.CoinsCount = GameConstants.Coins.Start;
        }

        private async Task SetStartMissions(User user)
        {
            user.MissionSetIds = new List<MissionSetIdWithOrder>
                                     {
                                         new MissionSetIdWithOrder
                                             {
                                                 MissionSetId = GameConstants.MissionSet.FirstSetId,
                                                 Order = 0
                                             },
                                         new MissionSetIdWithOrder
                                             {
                                                 MissionSetId = GameConstants.MissionSet.SecondSetId,
                                                 Order = 1
                                             },
                                         new MissionSetIdWithOrder
                                             {
                                                 MissionSetId = GameConstants.MissionSet.ThirdSetId,
                                                 Order = 2
                                             }
                                     };
            user.ActiveMissionIds =
                (await
                 _missionRepository.GetMissions(
                     new QueryOptions<Mission>
                         {
                             Filter =
                                 mission =>
                                 mission.MissionSetId == GameConstants.MissionSet.FirstSetId
                                 || mission.MissionSetId == GameConstants.MissionSet.SecondSetId
                                 || mission.MissionSetId == GameConstants.MissionSet.ThirdSetId
                         }))
                    .Select(m => new MissionIdWithSetId { MissionId = m.Id, MissionSetId = m.MissionSetId }).ToList();
            user.ActiveMissionSetIds = new List<string>
                                           {
                                               GameConstants.MissionSet.FirstSetId, GameConstants.MissionSet.SecondSetId,
                                               GameConstants.MissionSet.ThirdSetId
                                           };
        }
    }
}