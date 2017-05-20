namespace Infrastructure.Repositories.AzureTables
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Table;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;
    using AzureTablesObjects;
    using AzureTablesObjects.TableEntities;

    using Core.Interfaces.Repositories.Common;
    using Core.Tools;

    using InfrastructureTools.Azure;

    /// <summary>
    ///     The azure storage mission draft repository.
    /// </summary>
    public sealed class MissionDraftRepository : IMissionDraftRepository
    {
        #region Fields

        private readonly AzureTableStorageManager _azureManager;
        private readonly ImageProvider _imagesProvider;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionDraftRepository" /> class.
        /// </summary>
        public MissionDraftRepository()
        {
            _azureManager = new AzureTableStorageManager(AzureTableName.MissionDrafts);
            _imagesProvider = new ImageProvider();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the mission draft.
        /// </summary>
        /// <param name="mission">
        ///     The mission.
        /// </param>
        /// <returns>
        ///     Task{AddResult}.
        /// </returns>
        public async Task<IdResult> AddMissionDraft(MissionDraft mission)
        {
            mission.Id = Guid.NewGuid().ToString();
            var missionDraft = mission.ToAzureModel();
            var typeLinks = GeneratePersonQualityLinks(mission);

            missionDraft.PhotoUrl = await _imagesProvider.SaveImageToProductionBlobOnAdd(missionDraft.PhotoUrl, BlobContainer.MissionDraftImages);

            if (!typeLinks.Any())
            {
                return await _azureManager.AddEntityAsync(missionDraft);
            }

            typeLinks.Insert(0, missionDraft);
            return await _azureManager.AddEntityBatchAsync(typeLinks);
        }

        /// <summary>
        /// Deletes the mission draft.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> DeleteMissionDraft(string id)
        {
            var entities = await _azureManager.GetEntitiesForCompleteDelete<MissionDraftAzure>(id);
            var draftAzure = entities.FirstOrDefault(m => !m.IsPersonQualityLink);
            if (draftAzure != null)
            {
                await _imagesProvider.DeleteImage(draftAzure.PhotoUrl);
            }

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
        ///     Gets the mission draft.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <returns>
        ///     Task{MissionDraft}.
        /// </returns>
        public async Task<MissionDraft> GetMissionDraft(string id)
        {
            var draftRelatedEntities =
                await
                _azureManager.GetEntitiesAsync(new TableQuery<MissionDraftAzure>().Where(id.GetFilterById()));
            return ConvertToMissionDraft(draftRelatedEntities, true);
        }

        /// <summary>
        ///     Gets the mission drafts.
        /// </summary>
        /// <param name="options">
        ///     The options.
        /// </param>
        /// <returns>
        ///     Task{IEnumerable{MissionDraft}}.
        /// </returns>
        public async Task<List<MissionDraft>> GetMissionDrafts(QueryOptions<MissionDraft> options)
        {
            string expandFilter;
            bool needExpand = CheckExpandGetFilter(options.Expand, out expandFilter);
            var tableQuery = options.GenerateTableQuery<MissionDraft, MissionDraftAzure>(expandFilter);

            var azureMissionDrafts = await _azureManager.GetEntitiesAsync(tableQuery);

            var missionDrafts =
                azureMissionDrafts.GroupBy(m => m.PartitionKey)
                    .Select(group => ConvertToMissionDraft(group.ToList(), needExpand)).ToList();
            return missionDrafts.FilterCollectionPostFactum(options);
        }

        /// <summary>
        ///     Removes the type of the links to deleted person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> RemoveLinksToDeletedPersonQuality(string personQualityId)
        {
            var azureMissionDrafts = await _azureManager.GetEntitiesAsync(new TableQuery<MissionDraftAzure>());
            var entitiesToDelete =
                azureMissionDrafts.Where(m => m.IsPersonQualityLink && string.Equals(m.PersonQualityId, personQualityId));
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
            }

            return error.Capacity <= 0 
                ? new OperationResult(OperationResultStatus.Success) 
                : new OperationResult(OperationResultStatus.Error, error.ToString());
        }

        /// <summary>
        ///     Updates the type of the links to person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <param name="personQualityName">Name of the person quality.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> UpdateLinksToPersonQuality(string personQualityId, string personQualityName)
        {
            var azureMissionDrafts = await _azureManager.GetEntitiesAsync(new TableQuery<MissionDraftAzure>());
            var entitiesToUpdate =
                azureMissionDrafts.Where(m => m.IsPersonQualityLink && string.Equals(m.PersonQualityId, personQualityId))
                    .ToIList();
            foreach (var missionDraftAzure in entitiesToUpdate)
            {
                missionDraftAzure.Name = personQualityName;
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
            }

            if (error.Capacity <= 0)
            {
                return new OperationResult(OperationResultStatus.Success);
            }

            return new OperationResult(OperationResultStatus.Error, error.ToString());
        }

        /// <summary>
        ///     Updates the mission draft.
        /// </summary>
        /// <param name="mission">
        ///     The mission.
        /// </param>
        /// <returns>
        ///     Task{OperationResult}.
        /// </returns>
        public async Task<OperationResult> UpdateMissionDraft(MissionDraft mission)
        {
            var draftRelatedEntities =
                await
                _azureManager.GetEntitiesAsync(
                    new TableQuery<MissionDraftAzure>().Where(mission.Id.GetFilterById()));
            var missionDraftAzure = draftRelatedEntities.FirstOrDefault(m => !m.IsPersonQualityLink);
            if (missionDraftAzure == null)
            {
                return new OperationResult(OperationResultStatus.Error, "Can't find mission draft for update");
            }

            var newMissionDraft = mission.ToAzureModel();
            newMissionDraft.PhotoUrl = await _imagesProvider.SaveImageToProductionBlobOnUpdate(missionDraftAzure.PhotoUrl, newMissionDraft.PhotoUrl, BlobContainer.MissionDraftImages);
            newMissionDraft.CopyToTableEntity(missionDraftAzure);
            var entitiesToDelete = draftRelatedEntities.Where(m => m.IsPersonQualityLink).ToList();
            var entitiesToAdd = GeneratePersonQualityLinks(mission);
            var entitiesToUpdate = AzureTableExtensions.FilterUpdatableLinks(entitiesToAdd, entitiesToDelete);
            entitiesToUpdate.Add(missionDraftAzure);

            return await _azureManager.UpdateEntityBatchAsync(entitiesToUpdate, entitiesToAdd, entitiesToDelete);
        }
        
        #endregion

        #region Methods

        private bool CheckExpandGetFilter(IEnumerable<string> expand, out string expandFilter)
        {
            // if expand is not empty, related entites should be selected so we don't need additional filter
            if (expand != null
                && expand.Any(e => string.Equals(e, "PersonQualities", StringComparison.InvariantCultureIgnoreCase)))
            {
                expandFilter = string.Empty;
                return true;
            }

            // if expand is empty, we need to add row filter to NOT select related entities
            expandFilter = TableQuery.GenerateFilterCondition(
                AzureTableConstants.RowKey, 
                QueryComparisons.Equal, 
                AzureTableConstants.DraftRowKey);
            return false;
        }

        private MissionDraft ConvertToMissionDraft(List<MissionDraftAzure> draftRelatedEntities, bool needExpand)
        {
            var missionDraftAzure = draftRelatedEntities.FirstOrDefault(m => m.IsMissionDraftEntity);
            var missionDraft = missionDraftAzure.FromAzureModel();
            if (missionDraft == null)
            {
                return null;
            }

            if (needExpand)
            {
                missionDraft.PersonQualities =
                    draftRelatedEntities.Where(u => u.IsPersonQualityLink)
                        .Select(
                            u =>
                            new PersonQualityIdWithScore
                                {
                                    Score = u.PersonQualityScore ?? 0,
                                    PersonQualityId =  u.PersonQualityId
                                }).ToList();
            }

            return missionDraft;
        }

        private List<MissionDraftAzure> GeneratePersonQualityLinks(MissionDraft mission)
        {
            if (mission.PersonQualities == null)
            {
                return new List<MissionDraftAzure>();
            }

            return mission.PersonQualities.Where(
                     t =>
                     !string.IsNullOrEmpty(t.PersonQualityId))
                     .GroupBy(p => p.PersonQualityId)
                     .Select(
                         t =>
                         MissionDraftAzure.CreateLinkToPersonQuality(
                             mission.Id,
                             t.First().PersonQualityId,
                             t.First().Score))
                     .ToList();
        }
        
        #endregion
    }
}