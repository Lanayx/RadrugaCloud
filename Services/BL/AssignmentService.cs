namespace Services.BL
{
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.Constants;
    using Core.DomainModels;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;
    using Core.Tools;

    /// <summary>
    /// Class AssignmentService
    /// </summary>
    public class AssignmentService
    {
        private readonly IMissionSetRepository _missionSetRepository;
        private readonly IMissionRepository _missionRepository;
        public AssignmentService(IMissionSetRepository missionSetRepository, IMissionRepository missionRepository)
        {
            _missionSetRepository = missionSetRepository;
            _missionRepository = missionRepository;
        }



        public async Task AssignMissionSetLine(User user)
        {
            var currrentAge = DateTime.UtcNow.Year - user.DateOfBirth.Value.Year;// DateOfBirth should not be null
            var allMissionSets = (await _missionSetRepository.GetMissionSets(new QueryOptions<MissionSet>
                                                                       {
                                                                           Expand = new List<string> { "PersonQualities", "" }
                                                                       })).Where(missionSet =>
                                                                              (missionSet.AgeFrom <= currrentAge) &&
                                                                               (missionSet.AgeTo >= currrentAge)).ToIList();//TODO move inside parent filter

            var missionSets =
                allMissionSets.Where(
                    missionSet =>
                    (missionSet.Id != GameConstants.MissionSet.FirstSetId)
                    && (missionSet.Id != GameConstants.MissionSet.SecondSetId)
                    && (missionSet.Id != GameConstants.MissionSet.ThirdSetId)
                    && (missionSet.Id != GameConstants.MissionSet.LastSetId)).ToIList();

            var easySets = missionSets.Where(set => GetEasySetCondition(set.PersonQualities, user)).Select(set => set.Id);
            var hardSets = missionSets.Where(set => GetHardSetCondition(set.PersonQualities, user)).Select(set => set.Id);
            var neutralSets = missionSets.Where(set => !easySets.Contains(set.Id) && !hardSets.Contains(set.Id)).Select(set => set.Id);
            var tempList = new List<string>();
            tempList.AddRange(easySets);
            tempList.AddRange(neutralSets);
            tempList.AddRange(hardSets);
            var finalList = GetSetIdsAfterDependenciesCheck(tempList, allMissionSets);

            var list = new List<MissionSetIdWithOrder>();
            for (var i = 0; i < finalList.Count; i++)
            {
                list.Add(new MissionSetIdWithOrder { MissionSetId = finalList[i], Order = i });
            }

            user.MissionSetIds = list;
        }

        private List<string> GetSetIdsAfterDependenciesCheck(List<string> assignedSetIds, IList<MissionSet> missionSets)
        {
            var dependings = new List<MissionSetIdWithDepends>();
            var missions = missionSets.SelectMany(ms => ms.Missions);
            var newMissionSetIds = new List<string> { GameConstants.MissionSet.FirstSetId, GameConstants.MissionSet.SecondSetId, GameConstants.MissionSet.ThirdSetId };

            foreach (var assignedSetId in assignedSetIds)
            {
                var setWithDepends = new MissionSetIdWithDepends();
                var missionSet = missionSets.First(ms => ms.Id == assignedSetId);

                setWithDepends.MissionSetId = missionSet.Id;

                var dependentMissionIds = missionSet.Missions.SelectMany(m => m.Mission.DependsOn ?? new List<string>());

                setWithDepends.DependentSetIds =
                    dependentMissionIds
                        .Select(missionId => missions.FirstOrDefault(m => m.Mission.Id == missionId))
                        .Where(m => m != null)
                        .Select(m => m.Mission.MissionSetId)
                        .Distinct()
                        .Where(setId => setId != assignedSetId)
                        .ToList();

                if (!setWithDepends.DependentSetIds.AnyValues() || isSubset(newMissionSetIds, setWithDepends.DependentSetIds))
                {
                    newMissionSetIds.Add(setWithDepends.MissionSetId);
                    TryAddDependants(newMissionSetIds, dependings);
                }
                else
                {
                    dependings.Add(setWithDepends);
                }
            }
            newMissionSetIds.Add(GameConstants.MissionSet.LastSetId);
            return newMissionSetIds;
        }

        private void TryAddDependants(List<string> newMissionSetIds, List<MissionSetIdWithDepends> dependings)
        {
            if (dependings.Count == 0)
                return;

            var applied = false;
            for (int i = 0; i < dependings.Count; i++)
            {
                var depending = dependings[i];
                if (isSubset(newMissionSetIds, depending.DependentSetIds))
                {
                    newMissionSetIds.Add(depending.MissionSetId);
                    dependings.Remove(depending);
                    applied = true;
                    break;
                }
            }

            if (applied)
                TryAddDependants(newMissionSetIds, dependings);
        }

        private class MissionSetIdWithDepends
        {
            public string MissionSetId;

            public List<string> DependentSetIds;
        }


        private bool isSubset(List<string> arr1, List<string> arr2)
        {
            int i;
            int n = arr2.Count, m = arr1.Count;
            for (i = 0; i < n; i++)
            {
                int j;
                for (j = 0; j < m; j++)
                {
                    if (arr2[i] == arr1[j])
                        break;
                }

                /* If the above inner loop was not broken at all then
                   arr2[i] is not present in arr1[] */
                if (j == m)
                    return false;
            }

            /* If we reach here then all elements of arr2[] 
              are present in arr1[] */
            return true;
        }


        private bool GetEasySetCondition(List<PersonQualityIdWithScore> qualityWithScores, User user)
        {
            return GetQualityMatch(qualityWithScores, user, GameConstants.PersonQuality.ActivityQualityId) &&
                    GetQualityMatch(qualityWithScores, user, GameConstants.PersonQuality.CommunicationQualityId);
        }

        private bool GetHardSetCondition(List<PersonQualityIdWithScore> qualityWithScores, User user)
        {
            return !GetQualityMatch(qualityWithScores, user, GameConstants.PersonQuality.ActivityQualityId) &&
                    !GetQualityMatch(qualityWithScores, user, GameConstants.PersonQuality.CommunicationQualityId);
        }

        private bool GetQualityMatch(
            List<PersonQualityIdWithScore> qualityWithScores,
            User user,
            string qualityId)
        {
            var communicationQualityInSet = qualityWithScores.FirstOrDefault(q => q.PersonQualityId == qualityId);
            var communicationInUser = user.PersonQualitiesWithScores.FirstOrDefault(q => q.PersonQualityId == qualityId);
            var communicationMatch = communicationQualityInSet == null || communicationInUser == null
                                     || communicationQualityInSet.Score * communicationInUser.Score >= 0;
            return communicationMatch;
        }
    }
}
