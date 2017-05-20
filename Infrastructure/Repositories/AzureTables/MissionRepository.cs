namespace Infrastructure.Repositories.AzureTables
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Practices.Unity;
    using Microsoft.WindowsAzure.Storage.Table;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.Tools;
    using AzureTablesObjects;
    using AzureTablesObjects.TableEntities;
    
    using InfrastructureTools.Azure;

    /// <summary>
    ///     Class MissionRepository
    /// </summary>
    public sealed class MissionRepository : IMissionRepository
    {
        #region Fields

        private readonly AzureTableStorageManager _azureManager;

        private readonly ImageProvider _imagesProvider;

        private IMissionSetRepository _missionSetRepository;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionRepository" /> class.
        /// </summary>
        public MissionRepository()
        {
            _azureManager = new AzureTableStorageManager(AzureTableName.Missions);
            _imagesProvider = new ImageProvider();
        }

        #endregion

        #region Properties

        private IMissionSetRepository MissionSetRepository => _missionSetRepository ?? 
            (_missionSetRepository = IocConfig.GetConfiguredContainer().Resolve<IMissionSetRepository>());

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the mission.
        /// </summary>
        /// <param name="mission">
        ///     The mission.
        /// </param>
        /// <returns>
        ///     Task{AddResult}.
        /// </returns>
        public async Task<IdResult> AddMission(Mission mission)
        {
            mission.Id = Guid.NewGuid().ToString("N");
            var azureModel = mission.ToAzureModel();
            azureModel.PhotoUrl =
                await _imagesProvider.SaveImageToProductionBlobOnAdd(azureModel.PhotoUrl, BlobContainer.MissionImages);
            var typeLinks = GeneratePersonQualityLinks(mission);

            var batch = new List<MissionAzure> { azureModel };
            if (typeLinks.Any())
            {
                batch.AddRange(typeLinks);
            }

            var hints = GenerateHintLinks(mission);
            if (hints.Any())
            {
                batch.AddRange(hints);
            }
            // Currently setting mission to MissionSet on Add is impossible and seems not required.
            return await _azureManager.AddEntityBatchAsync(batch);
        }

        /// <summary>
        ///     Deletes the mission.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <returns>
        ///     Task{OperationResult}.
        /// </returns>
        public async Task<OperationResult> DeleteMission(string id)
        {
            var entities = await _azureManager.GetEntitiesForCompleteDelete<MissionAzure>(id);
            var missionAzure = entities.FirstOrDefault(m => m.IsMissionEntity);
            string missionSetId = null;
            if (missionAzure != null)
            {
                missionSetId = missionAzure.MissionSetId;
                await _imagesProvider.DeleteImage(missionAzure.PhotoUrl);
            }

            if (!entities.Any())
            {
                return new OperationResult(OperationResultStatus.Warning, "No entities to delete");
            }

            var result = await _azureManager.DeleteEntitiesBatchAsync(entities);

            var refreshResult = await TryRefreshMissionSet(missionSetId);
            if (refreshResult.Status != OperationResultStatus.Success)
            {
                Trace.TraceError(refreshResult.Description);
            }

            return result;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //nothing to dispose
        }

        /// <summary>
        ///     Gets the mission.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <returns>
        ///     Task{Mission}.
        /// </returns>
        public async Task<Mission> GetMission(string id)
        {
            var azureMissions = await _azureManager.GetEntitiesAsync(new TableQuery<MissionAzure>().Where(id.GetFilterById()));
            return Converters.ConvertToMission(azureMissions, true);
        }

        /// <summary>
        ///     Gets the missions.
        /// </summary>
        /// <param name="options">
        ///     The options.
        /// </param>
        /// <returns>
        ///     Task{IEnumerable{Mission}}.
        /// </returns>
        public async Task<List<Mission>> GetMissions(QueryOptions<Mission> options)
        {
            string expandFilter;
            bool needExpand = CheckExpandGetFilter(options.Expand, out expandFilter);
            var tableQuery = options.GenerateTableQuery<Mission, MissionAzure>(expandFilter);

            var azureMissions = await _azureManager.GetEntitiesAsync(tableQuery);

            var missions =
                azureMissions.GroupBy(m => m.PartitionKey).Select(group => Converters.ConvertToMission(group.ToList(), needExpand)).ToList();

            return missions.FilterCollectionPostFactum(options);
        }

        /// <summary>
        ///     Removes the type of the links to deleted person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> RemoveLinksToDeletedPersonQuality(string personQualityId)
        {
            var azureMissions = await _azureManager.GetEntitiesAsync(new TableQuery<MissionAzure>());
            var entitiesToDelete =
                azureMissions.Where(m => m.IsPersonQualityLink && string.Equals(m.PersonQualityId, personQualityId)).ToList();
            var groups = entitiesToDelete.GroupBy(e => e.PartitionKey);
            var error = new StringBuilder();
            foreach (var group in groups)
            {
                var deleteResult = await _azureManager.DeleteEntitiesBatchAsync(group);
                if (deleteResult.Status != OperationResultStatus.Success)
                {
                    var message = $"{group.Key} delete error: {deleteResult.Description}. ";
                    error.AppendLine(message);
                }

                await ProcessMissionSetRefresh(azureMissions, group, error);
            }

            if (error.Capacity <= 0)
            {
                return new OperationResult(OperationResultStatus.Success);
            }

            return new OperationResult(OperationResultStatus.Error, error.ToString());
        }

        /// <summary>
        ///     Clears the links to mission set.
        /// </summary>
        /// <param name="missionIds">The mission ids.</param>
        /// <param name="missionSetId">The mission set id.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> SetMissionSetForMissions(List<string> missionIds, string missionSetId)
        {
            if (!missionIds.AnyValues())
            {
                return new OperationResult(OperationResultStatus.Success);
            }

            var missions =
                (await _azureManager.GetEntitiesAsync(new TableQuery<MissionAzure>())).Where(
                    m => m.IsMissionEntity && missionIds.Contains(m.Id));

            var failedMissions = new List<string>();
            foreach (var missionAzure in missions)
            {
                missionAzure.MissionSetId = missionSetId;
                var updateResult = await _azureManager.UpdateEntityAsync(missionAzure);
                if (updateResult.Status != 0)
                {
                    failedMissions.Add($"Update failed for mission: {missionAzure.Id}: {updateResult.Description}");
                }
            }

            if (failedMissions.Any())
            {
                return new OperationResult(OperationResultStatus.Error, failedMissions.JoinToString());
            }

            return new OperationResult(OperationResultStatus.Success);
        }

        /// <summary>
        ///     Updates the type of the links to person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <param name="personQualityName">Name of the person quality.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> UpdateLinksToPersonQuality(string personQualityId, string personQualityName)
        {
            var azureMissions = await _azureManager.GetEntitiesAsync(new TableQuery<MissionAzure>());
            var entitiesToUpdate =
                azureMissions.Where(m => m.IsPersonQualityLink && string.Equals(m.PersonQualityId, personQualityId))
                    .ToIList();

            foreach (var missionAzure in entitiesToUpdate)
            {
                missionAzure.Name = personQualityName;
            }

            var groups = entitiesToUpdate.GroupBy(e => e.PartitionKey);
            var error = new StringBuilder();
            foreach (var group in groups)
            {
                var updateResult = await _azureManager.UpdateEntityBatchAsync(group.ToIList());
                if (updateResult.Status != OperationResultStatus.Success)
                {
                    var message = $"{group.Key} update error: {updateResult.Description}. ";
                    error.AppendLine(message);
                }

                await ProcessMissionSetRefresh(azureMissions, group, error);
            }

            return error.Capacity <= 0 
                ? new OperationResult(OperationResultStatus.Success) 
                : new OperationResult(OperationResultStatus.Error, error.ToString());
        }

        /// <summary>
        ///     Updates the mission.
        /// </summary>
        /// <param name="mission">
        ///     The mission.
        /// </param>
        /// <returns>
        ///     Task{OperationResult}.
        /// </returns>
        public async Task<OperationResult> UpdateMission(Mission mission)
        {
            var relatedEntities =
                await _azureManager.GetEntitiesAsync(new TableQuery<MissionAzure>().Where(mission.Id.GetFilterById()));
            var missionAzure = relatedEntities.FirstOrDefault(m => m.IsMissionEntity);
            if (missionAzure == null)
            {
                return new OperationResult(OperationResultStatus.Error, "Can't find mission for update");
            }

            var updatedMission = mission.ToAzureModel();
            updatedMission.PhotoUrl =
                await
                _imagesProvider.SaveImageToProductionBlobOnUpdate(
                    missionAzure.PhotoUrl, 
                    updatedMission.PhotoUrl, 
                    BlobContainer.MissionImages);
            updatedMission.CopyToTableEntity(missionAzure);

            var entitiesToDelete = relatedEntities.Where(m => m.IsPersonQualityLink).ToList();
            var entitiesToAdd = GeneratePersonQualityLinks(mission);
            var entitiesToUpdate = AzureTableExtensions.FilterUpdatableLinks(entitiesToAdd, entitiesToDelete);
            entitiesToUpdate.Add(missionAzure);

            var hintsToDelete = relatedEntities.Where(m => m.IsHintLink).ToList();
            var hintsToAdd = GenerateHintLinks(mission);
            var hintsToUpdate = AzureTableExtensions.FilterUpdatableLinks(hintsToAdd, hintsToDelete);
            
            entitiesToUpdate.AddRange(hintsToUpdate);
            entitiesToAdd.AddRange(hintsToAdd);
            entitiesToDelete.AddRange(hintsToDelete);

            var updateResult = await _azureManager.UpdateEntityBatchAsync(entitiesToUpdate, entitiesToAdd, entitiesToDelete);
            var refreshResult = await TryRefreshMissionSet(missionAzure.MissionSetId);
            if (refreshResult.Status != OperationResultStatus.Success)
            {
                Trace.TraceError(refreshResult.Description);
            }

            return updateResult;
        }

        #endregion

        #region Methods

        private bool CheckExpandGetFilter(IEnumerable<string> expand, out string expandFilter)
        {
            // if expand is not empty, related entites should be selected so we don't need additional filter
            if (expand != null
                && expand.Any(e => string.Equals(e, "PersonQualities", StringComparison.InvariantCultureIgnoreCase) 
                                 || string.Equals(e, "Hints", StringComparison.InvariantCultureIgnoreCase)))
            {
                expandFilter = string.Empty;
                return true;
            }

            // if expand is empty, we need to add row filter to NOT select related entities, but we select related hints
            expandFilter = TableQuery.CombineFilters(
                AzureTableExtensions.GenerateStartsWithFilterCondition(AzureTableConstants.RowKey, AzureTableConstants.HintLinkRowKeyPrefix),
                TableOperators.Or,
                TableQuery.GenerateFilterCondition(
                    AzureTableConstants.RowKey, 
                    QueryComparisons.Equal, 
                    AzureTableConstants.MissionRowKey));
            return false;
        }

        private List<MissionAzure> GeneratePersonQualityLinks(Mission mission)
        {
            if (mission.PersonQualities == null)
            {
                return new List<MissionAzure>();
            }

            return 
                 mission.PersonQualities.Where(
                    t =>
                    !string.IsNullOrEmpty(t.PersonQualityId))
                    .GroupBy(p => p.PersonQualityId)
                    .Select(
                        t =>
                        MissionAzure.CreateLinkToPersonQuality(
                            mission.Id, 
                            t.First().PersonQualityId,
                            t.First().Score))
                    .ToList();
        }

        private List<MissionAzure> GenerateHintLinks(Mission mission)
        {
            if (mission.Hints == null)
            {
                return new List<MissionAzure>();
            }

            return
                 mission.Hints.Select(t => MissionAzure.CreateLinkToHint(mission.Id,t)).ToList();
        }

        private async Task ProcessMissionSetRefresh(
            IEnumerable<MissionAzure> azureMissions, 
            IGrouping<string, MissionAzure> group, 
            StringBuilder error)
        {
            var refreshResult =
                await
                TryRefreshMissionSet(
                    azureMissions.FirstOrDefault(m => m.IsMissionEntity && string.Equals(m.Id, group.Key)));
            if (refreshResult.Status != OperationResultStatus.Success)
            {
                var message = $"{group.Key} refresh mission set error: {refreshResult.Description}. ";
                error.AppendLine(message);
            }
        }

        private async Task<OperationResult> TryRefreshMissionSet(MissionAzure missionAzure)
        {
            if (missionAzure != null)
            {
                return await TryRefreshMissionSet(missionAzure.MissionSetId);
            }

            return new OperationResult(OperationResultStatus.Success);
        }

        private async Task<OperationResult> TryRefreshMissionSet(string missionSetId)
        {
            if (!string.IsNullOrEmpty(missionSetId))
            {
                return await MissionSetRepository.RefreshMissionDependentLinks(missionSetId);
            }

            return new OperationResult(OperationResultStatus.Success);
        }

        #endregion
    }
}