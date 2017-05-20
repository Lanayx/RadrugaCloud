// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable PossibleInvalidOperationException

namespace Services.BL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using ColorMine.ColorSpaces;

    using Core.CommonModels.Query;
    using Core.Constants;
    using Core.DomainModels;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;
    using Core.Tools;
    using DomainServices;

    /// <summary>
    ///     The rewards calculator. Should never update or add anything to db!
    /// </summary>
    public static class RewardsCalculator
    {
        private static Dictionary<byte, ushort> levelMap;

        static RewardsCalculator()
        {
            InitLevelMap();
        }

        /// <summary>
        ///     Sets the new mission sets.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="missionRepository">The mission repository.</param>
        /// <returns></returns>
        public static async Task SetNewMissionSets(User user, IMissionRepository missionRepository)
        {
            if (user.ActiveMissionSetIds != null && user.ActiveMissionSetIds.Count >= GameConstants.MissionSet.MaxSetsPerUser)
            {
                return;
            }

            MissionSetIdWithOrder newSet;
            var currentOrder = GameConstants.MissionSet.MaxSetsPerUser;
            do
            {
                newSet = user.MissionSetIds.First(s => s.Order == currentOrder);
                user.ActiveMissionSetIds = user.ActiveMissionSetIds ?? new List<string>();
                user.ActiveMissionSetIds.Add(newSet.MissionSetId);
                // go to db to load missions from set, not ideal but for now is ok
                await LoadMissionsFromNewSet(missionRepository, newSet.MissionSetId, user);
                currentOrder++;
            }
            while (user.ActiveMissionSetIds.Count < GameConstants.MissionSet.MaxSetsPerUser
                   && newSet.MissionSetId != GameConstants.MissionSet.LastSetId);
        }

        /// <summary>
        /// Updates the color of the radruga
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="createNew">if set to <c>true</c> [create new].</param>
        public static void UpdateRadrugaColor(User user, bool createNew)
        {
            if (!createNew && String.IsNullOrEmpty(user.RadrugaColor))
                return;

            double l = 0, a = 0, b = 0;
            foreach (var userQuality in user.PersonQualitiesWithScores)
            {
                if (userQuality.PersonQualityId == GameConstants.PersonQuality.LightQualityId)
                {
                    l = userQuality.Score;
                }
                else if (userQuality.PersonQualityId == GameConstants.PersonQuality.ActivityQualityId)
                {
                    a = userQuality.Score * 3.3;
                }
                else if (userQuality.PersonQualityId == GameConstants.PersonQuality.CommunicationQualityId)
                {
                    b = userQuality.Score * 3.3;
                }
            }
            user.RadrugaColor = Lab2Rgb(l, a, b);
        }

        /// <summary>
        ///     Updates the user after answering question.
        /// </summary>
        /// <param name="questionsOptions">The questions options.</param>
        /// <param name="user">The user.</param>
        public static void UpdateUserAfterAnsweringQuestion(PersonQualityIdWithScore[] questionsOptions, User user)
        {
            user.PersonQualitiesWithScores = user.PersonQualitiesWithScores ?? new List<PersonQualityIdWithScore>();

            foreach (var questionOption in questionsOptions)
            {
                var existingScore =
                     user.PersonQualitiesWithScores.FirstOrDefault(score => score.PersonQualityId == questionOption.PersonQualityId);
                if (existingScore == null)
                {
                    existingScore = new PersonQualityIdWithScore { PersonQualityId = questionOption.PersonQualityId };
                    user.PersonQualitiesWithScores.Add(existingScore);
                }
                existingScore.Score = questionOption.Score;
            }
        }

        /// <summary>
        ///     Updates the user ater kind action.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="kindAction">The kind action.</param>
        public static void UpdateUserAfterKindAction(User user, KindAction kindAction)
        {
            user.KindActionsCount = (user.KindActionsCount ?? 0) + 1;
            var withPhoto = !string.IsNullOrEmpty(kindAction.ImageUrl);
            //Update kind scale
            var oldKindScale = user.KindScale;
            user.KindScale += withPhoto ? GameConstants.KindScale.WithPhotoReward : GameConstants.KindScale.BaseReward;
            if (user.KindScale >= GameConstants.KindScale.MaxUserPoints)
            {
                user.KindScale = GameConstants.KindScale.MaxUserPoints;
                if (oldKindScale < GameConstants.KindScale.MaxUserPoints)
                {
                    //increasing coins count once right after reaching 100%
                    user.CoinsCount = (user.CoinsCount ?? 0) + GameConstants.Coins.IncreasePerKindActionMaxing;
                }
            }
            if (oldKindScale < GameConstants.KindScale.MaxUserPoints)
            {
                //Update light quality
                var light =
                    user.PersonQualitiesWithScores.FirstOrDefault(
                        pqws => pqws.PersonQualityId == GameConstants.PersonQuality.LightQualityId);
                if (light == null)
                {
                    user.PersonQualitiesWithScores.Add(
                            new PersonQualityIdWithScore
                            {
                                PersonQualityId = GameConstants.PersonQuality.LightQualityId,
                                Score = GameConstants.PersonQuality.IncreasePerKindAction
                            });
                }
                else
                {
                    light.Score += GameConstants.PersonQuality.IncreasePerKindAction;
                }

                UpdateRadrugaColor(user, false);

                //Update total score
                user.Points = (user.Points ?? 0) + GameConstants.Points.IncreasePerKindAction;
            }

        }

        /// <summary>
        ///     Updates the user ater mission completion.
        /// </summary>
        /// <param name="missionRequest">The mission request.</param>
        /// <param name="missionRepository">The mission repository.</param>
        /// <param name="appCountersService">The application counters service.(need to be passed for missions from the last set)</param>
        /// <returns>
        ///     Points increase
        /// </returns>
        public static async Task<int> UpdateUserAfterMissionCompletion(
            MissionRequest missionRequest,
            IMissionRepository missionRepository,
            AppCountersService appCountersService = null)
        {
            var currentLevelMax = levelMap[missionRequest.User.Level.Value]; //user level can't be null
            var expIncrease = (ushort)(missionRequest.Mission.Difficulty * GameConstants.Experience.PerDifficultyPoint);
            var pointsIncrease =
                (int)Math.Round(expIncrease * GameConstants.Points.ExtraMultiplier * missionRequest.StarsCount.Value);
            //stars after completion can't be null
            missionRequest.User.LevelPoints += expIncrease;
            missionRequest.User.Points = (missionRequest.User.Points ?? 0) + pointsIncrease;
            if (missionRequest.User.LevelPoints >= currentLevelMax)
            {
                missionRequest.User.Level++;
                missionRequest.User.LevelPoints = (ushort)(missionRequest.User.LevelPoints - currentLevelMax);
            }

            CalculatePersonQualities(missionRequest, true);

            await MissionFinished(missionRequest, true, missionRepository, appCountersService);

            return expIncrease;
        }

        /// <summary>
        ///     Updates the user ater mission decline.
        /// </summary>
        /// <param name="missionRequest">The mission request.</param>
        /// <param name="missionRepository">The mission repository.</param>
        /// <param name="appCountersService">The application counters service.</param>
        /// <returns>
        ///     If not empty returns the set id, so active missions need to be populated
        /// </returns>
        public static async Task UpdateUserAfterMissionDecline(
            MissionRequest missionRequest,
            IMissionRepository missionRepository,
            AppCountersService appCountersService = null)
        {
            var pointsDecrease =
                (ushort)
                (missionRequest.Mission.Difficulty * GameConstants.Experience.PerDifficultyPoint
                 * GameConstants.Points.ExtraMultiplier);
            missionRequest.User.Points = (missionRequest.User.Points ?? 0) - pointsDecrease;

            CalculatePersonQualities(missionRequest, false);

            await MissionFinished(missionRequest, false, missionRepository, appCountersService);
        }

        private static void AutoDeclineActiveMissions(
            string declinedMissionId,
            List<Mission> activeMissions,
            User user,
            List<MissionIdWithSetId> autoRemovedActiveMissions)
        {
            foreach (var activeMission in activeMissions)
            {
                if (activeMission.DependsOn.AnyValues() && activeMission.DependsOn.Contains(declinedMissionId))
                {
                    var userActiveMission = user.ActiveMissionIds.FirstOrDefault(m => m.MissionId == activeMission.Id);
                    if (userActiveMission != null)
                    {
                        user.ActiveMissionIds.Remove(userActiveMission);
                        autoRemovedActiveMissions.Add(userActiveMission);
                    }

                    if (!user.FailedMissionIds.Contains(activeMission.Id))
                    {
                        user.FailedMissionIds.Add(activeMission.Id);
                    }

                    AutoDeclineActiveMissions(
                        activeMission.Id,
                        activeMissions,
                        user,
                        autoRemovedActiveMissions);
                }
            }
        }

        private static void AutoDeclineNewMissions(
            string newSetId,
            List<Mission> newMissions,
            User user)
        {
            foreach (var mission in newMissions)
            {
                if (user.FailedMissionIds.AnyValues(fm => mission.DependsOn != null && mission.DependsOn.Contains(fm)))
                {
                    if (!user.FailedMissionIds.Contains(mission.Id))
                    {
                        user.FailedMissionIds.Add(mission.Id);
                        if (user.ActiveMissionIds.AnyValues())
                        {
                            var alreadyInActive =
                                user.ActiveMissionIds.FirstOrDefault(
                                    m => m.MissionId == mission.Id && m.MissionSetId == newSetId);
                            if (alreadyInActive != null)
                            {
                                user.ActiveMissionIds.Remove(alreadyInActive);
                            }
                        }
                        AutoDeclineNewMissions(newSetId, newMissions, user);
                    }
                }
                else
                {
                    if (!user.ActiveMissionIds.AnyValues(m => m.MissionId == mission.Id && m.MissionSetId == newSetId))
                    {
                        user.ActiveMissionIds = user.ActiveMissionIds ?? new List<MissionIdWithSetId>();
                        user.ActiveMissionIds.Add(new MissionIdWithSetId { MissionId = mission.Id, MissionSetId = newSetId });
                    }
                }
            }
        }

        private static void CalculatePersonQualities(MissionRequest missionRequest, bool afterSuccess)
        {
            if (missionRequest.Mission.PersonQualities != null && missionRequest.Mission.PersonQualities.Any())
            {
                var lightQuality =
                    missionRequest.Mission.PersonQualities.FirstOrDefault(
                        q => q.PersonQualityId == GameConstants.PersonQuality.LightQualityId);
                if (lightQuality != null)
                {

                    var lightScore = lightQuality.Score * missionRequest.StarsCount
                                     ?? (double)0 / GameConstants.PersonQuality.StarsNeededForMissionValue;
                    if (missionRequest.User.PersonQualitiesWithScores != null)
                    {

                        var userLightQuality =
                            missionRequest.User.PersonQualitiesWithScores.FirstOrDefault(pq => pq.PersonQualityId == GameConstants.PersonQuality.LightQualityId);
                        if (userLightQuality != null)
                        {
                            if (afterSuccess)
                            {
                                userLightQuality.Score += lightScore;
                                if (userLightQuality.Score > GameConstants.PersonQuality.LightScoreMax)
                                {
                                    userLightQuality.Score = GameConstants.PersonQuality.LightScoreMax;
                                }
                            }
                            else
                            {
                                userLightQuality.Score -= lightScore;
                                if (userLightQuality.Score < GameConstants.PersonQuality.LightScoreMin)
                                {
                                    userLightQuality.Score = GameConstants.PersonQuality.LightScoreMin;
                                }
                            }
                        }
                        else if (afterSuccess)
                        {

                            missionRequest.User.PersonQualitiesWithScores
                                .Add(
                                    new PersonQualityIdWithScore
                                    {
                                        PersonQualityId = GameConstants.PersonQuality.LightQualityId,
                                        Score = lightScore
                                    });
                        }
                    }
                    else if (afterSuccess)
                    {
                        missionRequest.User.PersonQualitiesWithScores = new List<PersonQualityIdWithScore>
                                                                            {
                                                                                new PersonQualityIdWithScore
                                                                                    {
                                                                                        PersonQualityId = GameConstants.PersonQuality.LightQualityId,
                                                                                        Score = lightScore
                                                                                    }
                                                                            };
                    }
                    UpdateRadrugaColor(missionRequest.User, false);
                }
            }
        }

        private static async Task<List<MissionIdWithSetId>> CheckActiveMissionDependencies(
            MissionRequest missionRequest,
            bool success,
            IMissionRepository missionRepository)
        {
            var autoRemovedActiveMissions = new List<MissionIdWithSetId>();
            if (success)
            {
                return autoRemovedActiveMissions;
            }

            var user = missionRequest.User;
            var declinedMissionId = missionRequest.Mission.Id;
            var activeMissionsIds = user.ActiveMissionIds.Select(m => m.MissionId).ToIList();
            var activeMissions =
                (await
                 missionRepository.GetMissions(
                     new QueryOptions<Mission>
                     {
                         Filter =
                                 ExpressionHelper.Expr((Mission x) => x.Id).In(activeMissionsIds),
                         Select = mission => new { mission.Id, mission.DependsOn }
                     }));

            AutoDeclineActiveMissions(
                declinedMissionId,
                activeMissions,
                user,
                autoRemovedActiveMissions);

            return autoRemovedActiveMissions;
        }

        private static async Task CheckLastSetCounters(
            MissionRequest missionRequest,
            AppCountersService appCountersService)
        {
            if (appCountersService != null && !missionRequest.User.ActiveMissionIds.Any()
                && !missionRequest.User.ActiveMissionSetIds.Any()
                && !string.IsNullOrEmpty(missionRequest.User.RadrugaColor))
            {
                await appCountersService.UserHasFinished();
            }
        }

        private static void InitLevelMap()
        {
            levelMap = new Dictionary<byte, ushort>
                           {
                               { 1, GameConstants.Experience.OnFirstLevel },
                               { 2, GameConstants.Experience.OnSecondLevel }
                           };
            for (byte i = 3; i < GameConstants.Experience.UnreachableLevel; i++)
            {
                levelMap.Add(
                    i,
                    (ushort)
                    (2 * levelMap[(byte)(i - 1)] - levelMap[(byte)(i - 2)] + GameConstants.Experience.IncreasePerLevel));
            }
        }

        private static string Lab2Rgb(double l, double a, double b)
        {
            var lab = new Lab { L = l, A = a, B = b };
            var rgb = lab.To<Rgb>();

            return ((int)Math.Round(rgb.R)).ToString("X2") + ((int)Math.Round(rgb.G)).ToString("X2")
                   + ((int)Math.Round(rgb.B)).ToString("X2");
        }

        private static async Task LoadMissionsFromNewSet(
            IMissionRepository missionRepository,
            string newSetId,
            User user)
        {
            var newMissions =
                (await
                 missionRepository.GetMissions(
                     new QueryOptions<Mission>
                     {
                         Select = mission => new { mission.Id, mission.DependsOn },
                         Filter = mission => mission.MissionSetId == newSetId
                     }));
            AutoDeclineNewMissions(newSetId, newMissions, user);
        }

        private static async Task MissionFinished(
            MissionRequest missionRequest,
            bool success,
            IMissionRepository missionRepository,
            AppCountersService appCountersService)
        {
            var user = missionRequest.User;
            var mission = missionRequest.Mission;
            var changedSetIds = await GetChangedSetIds(missionRequest, success, missionRepository, user, mission);
            var lastSetExist = changedSetIds.Contains(GameConstants.MissionSet.LastSetId);
            foreach (var changedSetId in changedSetIds)
            {
                var hasOtherSetMissions = user.ActiveMissionIds.Any(m => m.MissionSetId == changedSetId);
                if (!hasOtherSetMissions)
                {
                    user.ActiveMissionSetIds.Remove(changedSetId);

                    user.CompletedMissionSetIds = user.CompletedMissionSetIds ?? new List<string>();
                    user.CompletedMissionSetIds.Add(changedSetId);

                    await CheckForLastSetAndLoadMissions(missionRequest, missionRepository, user, changedSetId, lastSetExist);
                }
            }

            await CheckLastSetCounters(missionRequest, appCountersService);
            UpdateThreeStarsAchievement(user, missionRequest.StarsCount);
        }

        private static async Task CheckForLastSetAndLoadMissions(
            MissionRequest missionRequest,
            IMissionRepository missionRepository,
            User user,
            string changedSetId,
            bool lastSetExist)
        {
            var finishedSet = user.MissionSetIds.First(s => s.MissionSetId == changedSetId);
            if (!lastSetExist)
            {
                MissionSetIdWithOrder newSet;
                var currentOrder = finishedSet.Order + 1;
                do
                {
                    newSet = user.MissionSetIds.FirstOrDefault(s => s.Order == currentOrder);
                    if (newSet == null) //after 3 sets completed before test passed
                    {
                        break;
                    }
                    currentOrder++;
                }
                while ((user.ActiveMissionSetIds.Contains(newSet.MissionSetId)
                        || user.CompletedMissionSetIds.Contains(newSet.MissionSetId)));

                if (newSet != null)
                {
                    user.ActiveMissionSetIds.Add(newSet.MissionSetId);
                    // go to db to load missions from set, not ideal but for now is ok
                    await LoadMissionsFromNewSet(missionRepository, newSet.MissionSetId, missionRequest.User);
                }
            }
        }

        private static async Task<IList<string>> GetChangedSetIds(
            MissionRequest missionRequest,
            bool success,
            IMissionRepository missionRepository,
            User user,
            Mission mission)
        {
            if (success)
            {
                user.CompletedMissionIds = user.CompletedMissionIds ?? new List<string>();
                user.CompletedMissionIds.Add(mission.Id);
            }
            else
            {
                user.FailedMissionIds = user.FailedMissionIds ?? new List<string>();
                user.FailedMissionIds.Add(mission.Id);
            }
            user.ActiveMissionIds.RemoveAll(m => m.MissionId == mission.Id);

            var autoRemovedActiveMissions = await CheckActiveMissionDependencies(missionRequest, success, missionRepository);
            var changedSetIds = autoRemovedActiveMissions.Select(m => m.MissionSetId).ToIList();
            if (!changedSetIds.Contains(mission.MissionSetId))
            {
                changedSetIds.Add(mission.MissionSetId);
            }
            return changedSetIds;
        }

        private static void UpdateThreeStarsAchievement(User user, byte? starsCount)
        {
            if (user.ThreeStarsMissionSpreeMaxCount == 5)
            {
                return;
            }

            if (starsCount == 3)
            {
                user.ThreeStarsMissionSpreeCurrentCount = (user.ThreeStarsMissionSpreeCurrentCount ?? 0) + 1;
                if (user.ThreeStarsMissionSpreeCurrentCount > (user.ThreeStarsMissionSpreeMaxCount ?? 0))
                {
                    user.ThreeStarsMissionSpreeMaxCount = user.ThreeStarsMissionSpreeCurrentCount;
                }
            }
            else
            {
                user.ThreeStarsMissionSpreeCurrentCount = 0;
            }
        }
    }
}