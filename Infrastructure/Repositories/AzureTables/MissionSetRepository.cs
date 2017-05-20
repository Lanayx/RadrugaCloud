namespace Infrastructure.Repositories.AzureTables
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;
    using Microsoft.Practices.Unity;
    using Microsoft.WindowsAzure.Storage.Table;

    using AzureTablesObjects;
    using AzureTablesObjects.TableEntities;

    using Core.Tools;

    /// <summary>
    ///     Class MissionSetRepository
    /// </summary>
    public sealed class MissionSetRepository : IMissionSetRepository
    {
        #region Fields

        private readonly AzureTableStorageManager _azureManager;

        private IMissionRepository _missionRepository;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionSetRepository" /> class.
        /// </summary>
        public MissionSetRepository()
        {
            _azureManager = new AzureTableStorageManager(AzureTableName.MissionSets);
        }

        #endregion

        #region Properties

        private IMissionRepository MissionRepository => _missionRepository ??
            (_missionRepository = IocConfig.GetConfiguredContainer().Resolve<IMissionRepository>());

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the MissionSet.
        /// </summary>
        /// <param name="missionSet">
        ///     The MissionSet.
        /// </param>
        /// <returns>
        ///     Task{AddResult}.
        /// </returns>
        public async Task<IdResult> AddMissionSet(MissionSet missionSet)
        {
            missionSet.Id = Guid.NewGuid().ToString("N");
            var azureModel = missionSet.ToAzureModel();
            var missionLinks = await GenerateMissionDependentLinks(missionSet, azureModel);

            var batch = new List<MissionSetAzure> { azureModel };
            if (missionLinks.Any())
            {
                batch.AddRange(missionLinks);
            }

            return await _azureManager.AddEntityBatchAsync(batch);
        }

        /// <summary>
        ///     Deletes the MissionSet.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <returns>
        ///     Task{OperationResult}.
        /// </returns>
        public async Task<OperationResult> DeleteMissionSet(string id)
        {
            var entities = await _azureManager.GetEntitiesForCompleteDelete<MissionSetAzure>(id);
            var missionIds = entities.Where(l => l.IsMissionLink).Select(l => l.MissionId).ToList();
            await ClearBackReference(missionIds);
            if (entities.Any())
            {
                var result = await _azureManager.DeleteEntitiesBatchAsync(entities);
                return result;
            }

            return new OperationResult(OperationResultStatus.Warning, "No entities to delete");
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //nothing to dispose
        }

        /// <summary>
        ///     Gets the MissionSet.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <returns>
        ///     Task{MissionSet}.
        /// </returns>
        public async Task<MissionSet> GetMissionSet(string id)
        {
            var relatedEntities =
                await _azureManager.GetEntitiesAsync(new TableQuery<MissionSetAzure>().Where(id.GetFilterById()));
            return await ConvertToMissionSet(relatedEntities, true);
        }

        /// <summary>
        ///     Gets the MissionSets.
        /// </summary>
        /// <param name="options">
        ///     The options.
        /// </param>
        /// <returns>
        ///     Task{IEnumerable{MissionSet}}.
        /// </returns>
        public async Task<List<MissionSet>> GetMissionSets(QueryOptions<MissionSet> options)
        {
            string expandFilter;
            bool needExpand = CheckExpandGetFilter(options.Expand, out expandFilter);
            var tableQuery = options.GenerateTableQuery<MissionSet, MissionSetAzure>(expandFilter);

            var azureMissionSets = await _azureManager.GetEntitiesAsync(tableQuery);

            var missionSetsQuery =
                azureMissionSets.GroupBy(m => m.PartitionKey)
                    .Select(group => ConvertToMissionSet(group.ToIList(), needExpand));

            var missionSets = await Task.WhenAll(missionSetsQuery);

            var filteredMissionSets = missionSets.ToList().FilterCollectionPostFactum(options);

            return filteredMissionSets;
        }

        /// <summary>
        ///     Refreshes the mission dependent links.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> RefreshMissionDependentLinks(string id)
        {
            var missionSet = await GetMissionSet(id);
            if (missionSet == null)
            {
                return new OperationResult(OperationResultStatus.Error, "Can't find specified mission set");
            }

            var result = await UpdateMissionSet(missionSet);
            return result;
        }

        /// <summary>
        ///     Updates the MissionSet.
        /// </summary>
        /// <param name="missionSet">
        ///     The MissionSet.
        /// </param>
        /// <returns>
        ///     Task{OperationResult}.
        /// </returns>
        public async Task<OperationResult> UpdateMissionSet(MissionSet missionSet)
        {
            var relatedEntities =
                await
                _azureManager.GetEntitiesAsync(new TableQuery<MissionSetAzure>().Where(missionSet.Id.GetFilterById()));
            var missionSetAzure = relatedEntities.FirstOrDefault(m => m.IsMissionSetEntity);
            if (missionSetAzure == null)
            {
                return new OperationResult(OperationResultStatus.Error, "Can't find MissionSet for update");
            }

            var updatedMissionSet = missionSet.ToAzureModel();
            updatedMissionSet.CopyToTableEntity(missionSetAzure);

            var entitiesToDelete = relatedEntities.Where(m => !m.IsMissionSetEntity).ToList();
            await ClearBackReferenceForMissionsExcludedFromMissionSet(entitiesToDelete, missionSet);
            var entitiesToAdd = await GenerateMissionDependentLinks(missionSet, missionSetAzure);
            var entitiesToUpdate = AzureTableExtensions.FilterUpdatableLinks(entitiesToAdd, entitiesToDelete);
            entitiesToUpdate.Add(missionSetAzure);

            return await _azureManager.UpdateEntityBatchAsync(entitiesToUpdate, entitiesToAdd, entitiesToDelete);
        }



        #endregion

        #region Methods

        private bool CheckExpandGetFilter(IEnumerable<string> expand, out string expandFilter)
        {
            // if expand is not empty, related entites should be selected so we don't need additional filter
            if (expand != null
                && expand.Any(e => string.Equals(e, "PersonQualities", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(e, "Missions", StringComparison.InvariantCultureIgnoreCase)))
            {
                expandFilter = string.Empty;
                return true;
            }

            // if expand is empty, we need to add row filter to NOT select related entities
            expandFilter = TableQuery.GenerateFilterCondition(
                AzureTableConstants.RowKey, 
                QueryComparisons.Equal, 
                AzureTableConstants.MissionSetRowKey);
            return false;
        }

        private async Task ClearBackReference(List<string> missionIds)
        {
            var updateResult = await MissionRepository.SetMissionSetForMissions(missionIds, null);
            if (updateResult.Status != OperationResultStatus.Success)
            {
                Trace.TraceError("ClearBackConnection failed: {0}", updateResult.Description);
            }
        }

        private async Task ClearBackReferenceForMissionsExcludedFromMissionSet(
            List<MissionSetAzure> linksToBeDeleted, 
            MissionSet missionSet)
        {
            var currentMissions = linksToBeDeleted.Where(l => l.IsMissionLink).Select(l => l.MissionId);
            var includedMissions =
                (missionSet.Missions ?? new List<MissionWithOrder>()).Select(m => m.Mission.Id);
            var missionsToClearBackReference = currentMissions.Where(m => !includedMissions.Contains(m)).ToList();
            await ClearBackReference(missionsToClearBackReference);
        }

        private async Task<MissionSet> ConvertToMissionSet(IList<MissionSetAzure> relatedEntities, bool needExpand)
        {
            var missionSetAzure = relatedEntities.FirstOrDefault(m => m.IsMissionSetEntity);
            var missionSet = missionSetAzure.FromAzureModel();
            if (missionSet == null)
            {
                return null;
            }
            if (needExpand)
            {
                var missionLinks = relatedEntities.Where(u => u.IsMissionLink).Select(m => m.MissionId);
                var query = new QueryOptions<Mission> {Expand = new List<string> { "Hints" } };
                var missions = (await MissionRepository.GetMissions(query)).Where(m => missionLinks.Contains(m.Id));
                missionSet.Missions =
                    relatedEntities.Where(u => u.IsMissionLink && missions.Any(m => m.Id == u.MissionId))
                        .Select(
                            u =>
                            new MissionWithOrder
                                {
                                    Order = (byte)u.OrderInSet, 
                                    Mission = missions.First(m => m.Id == u.MissionId)
                                }).ToList();

                missionSet.PersonQualities =
                    relatedEntities.Where(u => u.IsPersonQualityLink)
                        .Select(
                            u =>
                            new PersonQualityIdWithScore
                                {
                                    Score = u.PersonQualityScore ?? 0, 
                                    PersonQualityId = u.PersonQualityId
                                }).ToList();
            }
            return missionSet;
        }

        private void FillAgesForMissionSet(IList<Mission> missions, MissionSetAzure missionSetAzure)
        {
            var fromAges =
                missions.Where(m => m.AgeFrom.HasValue && m.AgeFrom > 0)
                    .Select(m => m.AgeFrom)
                    .Distinct()
                    .OrderByDescending(m => m);
            missionSetAzure.AgeFrom = fromAges.FirstOrDefault();

            var toAges =
                missions.Where(m => m.AgeTo.HasValue && m.AgeTo > 0).Select(m => m.AgeTo).Distinct().OrderBy(m => m);
            missionSetAzure.AgeTo = toAges.FirstOrDefault();
        }

        private async Task<List<MissionSetAzure>> GenerateMissionDependentLinks(MissionSet missionSet, MissionSetAzure missionSetAzure)
        {
            if (missionSet.Missions == null)
            {
                return new List<MissionSetAzure>();
            }
            var orderedMissionIds =
                missionSet.Missions.OrderBy(m => m.Order)
                    .Where(t => !string.IsNullOrEmpty(t.Mission?.Id))
                    .Select(m => m.Mission.Id)
                    .Distinct()
                    .ToList();
            var query = new QueryOptions<Mission>
                            {
                                Expand = new List<string> { "PersonQualities" },
                            };
            var missions =
                (await MissionRepository.GetMissions(query)).Where(m => orderedMissionIds.Contains(m.Id)).ToList();
            var validOrderedMissionIds = orderedMissionIds.Where(o => missions.Any(m => m.Id == o));
            byte order = 0;
            var missionLinks =
                validOrderedMissionIds.Select(t => MissionSetAzure.CreateMissionLink(missionSet.Id, t, order++));
            var personQualitiesLinks = GeneratePersonQualitiesLinks(missionSet, missions);
            var allLinks = missionLinks.Concat(personQualitiesLinks);
            FillAgesForMissionSet(missions, missionSetAzure);
            await RefreshBackReferencesForMissions(missions, missionSet.Id);
            return allLinks.ToList();
        }



        private async Task RefreshBackReferencesForMissions(List<Mission> missions, string missionSetId)
        {
            var missionsForRefresh = missions.Where(m => !string.Equals(m.MissionSetId, missionSetId)).Select(m => m.Id).ToList();
            var updateResult = await MissionRepository.SetMissionSetForMissions(missionsForRefresh, missionSetId);
            if (updateResult.Status != OperationResultStatus.Success)
            {
                Trace.TraceError("RefreshBackReferencesForMissions failed: {0}", updateResult.Description);
            }
        }

        private IEnumerable<MissionSetAzure> GeneratePersonQualitiesLinks(
            MissionSet missionSet, 
            IEnumerable<Mission> missions)
        {
            var allPersonQualitiesGrouped =
                missions.Where(m => m.PersonQualities != null)
                    .SelectMany(m => m.PersonQualities)
                    .GroupBy(p => p.PersonQualityId);
            var personQualitiesLinks =
                allPersonQualitiesGrouped.Select(
                    g =>
                    MissionSetAzure.CreateLinkToPersonQuality(
                        missionSet.Id, 
                        g.Key,
                        g.Sum(p => p.Score)));
            return personQualitiesLinks;
        }
        #endregion
    }
}