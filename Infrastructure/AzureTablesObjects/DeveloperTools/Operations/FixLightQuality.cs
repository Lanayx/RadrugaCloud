using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.AzureTablesObjects.DeveloperTools.Operations
{
    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.Constants;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Providers;
    using Core.Interfaces.Repositories;
    using Core.Tools;

    using Microsoft.Practices.Unity;

    class FixLightQuality : IDeveloperOperation
    {
        /// <summary>
        /// Excecutes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>OperationResult.</returns>
        public async Task<OperationResult> Excecute(params object[] args)
        {
            var repo = IocConfig.GetConfiguredContainer().Resolve<IUserRepository>();
            var missionRepository = IocConfig.GetConfiguredContainer().Resolve<IMissionRepository>();
            var kindActionsRepository = IocConfig.GetConfiguredContainer().Resolve<IKindActionRepository>();

            var usersToUpdate = await repo.GetUsers(new QueryOptions<User>() { Expand = new List<string> { "PersonQualities" } });
            var missions = await missionRepository.GetMissions(new QueryOptions<Mission>() { Expand = new List<string> { "PersonQualities" } });

            foreach (var user in usersToUpdate)
            {
                var lightScore = user.PersonQualitiesWithScores?.FirstOrDefault(q => q.PersonQualityId == GameConstants.PersonQuality.LightQualityId);
                if (lightScore == null) {
                    lightScore = new PersonQualityIdWithScore { PersonQualityId = GameConstants.PersonQuality.LightQualityId };
                    if (user.PersonQualitiesWithScores == null) {
                        user.PersonQualitiesWithScores = new List<PersonQualityIdWithScore>();
                    }
                    user.PersonQualitiesWithScores.Add(lightScore);
                }
                lightScore.Score = 0;
                if (user.CompletedMissionIds.AnyValues())
                    foreach (var missionId in user.CompletedMissionIds)
                    {
                        var mission = missions.First(m => m.Id == missionId);
                        var missionLightScore = mission.PersonQualities?.FirstOrDefault(pq => pq.PersonQualityId == GameConstants.PersonQuality.LightQualityId);
                        if (missionLightScore != null) {
                            lightScore.Score += missionLightScore.Score;
                        }
                    }
                if (user.FailedMissionIds.AnyValues())
                    foreach (var missionId in user.FailedMissionIds)
                    {
                        var mission = missions.First(m => m.Id == missionId);
                        var missionLightScore = mission.PersonQualities?.FirstOrDefault(pq => pq.PersonQualityId == GameConstants.PersonQuality.LightQualityId);
                        if (missionLightScore != null)
                        {
                            lightScore.Score -= missionLightScore.Score;
                        }
                    }

                await kindActionsRepository.RemoveDuplicateKindActions(user.Id);

                var userKindActionsCount =
                    await kindActionsRepository.GetKindActions(
                        new QueryOptions<KindAction> { Filter = (kindAction) => kindAction.UserId == user.Id });
                lightScore.Score += userKindActionsCount.Count * GameConstants.PersonQuality.IncreasePerKindAction;
                if (lightScore.Score > 0)           
                    await repo.UpdateUser(user);

            }
            return new OperationResult(OperationResultStatus.Success);
        }
    }
}
