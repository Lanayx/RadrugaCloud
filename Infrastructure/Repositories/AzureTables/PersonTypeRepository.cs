namespace Infrastructure.Repositories.AzureTables
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using AzureTablesObjects;
    using AzureTablesObjects.TableEntities;

    using Core.Interfaces.Repositories.Common;

    using Microsoft.Practices.Unity;

    using Core.Tools;

    /// <summary>
    ///     Class AzurePersonQualityRepository
    /// </summary>
    public sealed class PersonQualityRepository : IPersonQualityRepository
    {
        #region Fields

        private readonly AzureTableStorageManager _azureManager;

        private IMissionDraftRepository _missionDraftRepository;

        private IMissionRepository _missionRepository;

        private IUserRepository _userRepository;

        private IQuestionRepository _questionRepository;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PersonQualityRepository" /> class.
        /// </summary>
        public PersonQualityRepository()
        {
            _azureManager = new AzureTableStorageManager(AzureTableName.PersonQualities);
        }

        private List<IPersonQualityDependent> RepositoriesDependentOnPersonQuality
            =>
                new List<IPersonQualityDependent>
                    {
                        _missionDraftRepository
                        ?? (_missionDraftRepository = IocConfig.GetConfiguredContainer().Resolve<IMissionDraftRepository>()),
                        _missionRepository
                        ?? (_missionRepository = new MissionRepository()),
                        _userRepository ?? (_userRepository = IocConfig.GetConfiguredContainer().Resolve<IUserRepository>()),
                        _questionRepository
                        ?? (_questionRepository = IocConfig.GetConfiguredContainer().Resolve<IQuestionRepository>()),
                    };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the type of the person.
        /// </summary>
        /// <param name="personQuality">Type of the person.</param>
        /// <returns>Task{AddResult}.</returns>
        public async Task<IdResult> AddPersonQuality(PersonQuality personQuality)
        {
            personQuality.Id = Guid.NewGuid().ToString("N");
            return await _azureManager.AddEntityAsync(personQuality.ToAzureModel());
        }

        /// <summary>
        ///     Deletes the type of the person.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> DeletePersonQuality(string id)
        {
            var personQualityAzure =
                await
                _azureManager.GetEntityByIdAndRowKeyAsync<PersonQualityAzure>(id, AzureTableConstants.PersonQualityRowKey);
            if (personQualityAzure == null)
            {
                return new OperationResult(OperationResultStatus.Error, "No person quality for delete");
            }

            var deleteResult = await _azureManager.DeleteEntityAsync(personQualityAzure);
            if (deleteResult.Status == OperationResultStatus.Success)
            {
                var warnings = new StringBuilder();
                foreach (var personQualityDependent in RepositoriesDependentOnPersonQuality)
                {
                    var refreshMissionDraftsResult = await personQualityDependent.RemoveLinksToDeletedPersonQuality(id);
                    if (refreshMissionDraftsResult.Status != OperationResultStatus.Success)
                    {
                        var warning =
                            $"Links to deleted person quality are not removed from {personQualityDependent.GetType().Name}! ";
                        warnings.AppendLine(warning);
                    }
                }

                if (warnings.Length > 0)
                {
                    return new OperationResult(OperationResultStatus.Warning, warnings.ToString());
                }
            }

            return deleteResult;
        }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            //nothing to dispose
        }

        /// <summary>
        ///     Gets the type of the person.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{PersonQuality}.</returns>
        public async Task<PersonQuality> GetPersonQuality(string id)
        {
            var result =
                await
                _azureManager.GetEntityByIdAndRowKeyAsync<PersonQualityAzure>(id, AzureTableConstants.PersonQualityRowKey);
            return result.FromAzureModel();
        }

        /// <summary>
        ///     Gets the person qualities.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{PersonQuality}}.</returns>
        public async Task<List<PersonQuality>> GetPersonQualities(QueryOptions<PersonQuality> options)
        {
            var tableQuery = options.GenerateTableQuery<PersonQuality,PersonQualityAzure>();
            var result = await _azureManager.GetEntitiesAsync(tableQuery);
            var personQualities = result.Select(az => az.FromAzureModel()).ToList();
            return personQualities.FilterCollectionPostFactum(options);
        }

        /// <summary>
        ///     Updates the type of the person.
        /// </summary>
        /// <param name="personQuality">The mission.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> UpdatePersonQuality(PersonQuality personQuality)
        {
            var updatedPersonQuality = personQuality.ToAzureModel();
            var personQualityAzure =
                await
                _azureManager.GetEntityByIdAndRowKeyAsync<PersonQualityAzure>(
                    updatedPersonQuality.PartitionKey, 
                    updatedPersonQuality.RowKey);
            if (personQualityAzure == null)
            {
                return new OperationResult(OperationResultStatus.Error, "No person quality for update");
            }

            updatedPersonQuality.CopyToTableEntity(personQualityAzure);
            var updateResult = await _azureManager.UpdateEntityAsync(personQualityAzure);
            if (updateResult.Status == OperationResultStatus.Success)
            {
                var warnings = new StringBuilder();
                foreach (var personQualityDependent in RepositoriesDependentOnPersonQuality)
                {
                    var refreshMissionDraftsResult = await personQualityDependent.UpdateLinksToPersonQuality(personQualityAzure.Id, personQualityAzure.Name);
                    if (refreshMissionDraftsResult.Status != OperationResultStatus.Success)
                    {
                        var warning =
                            $"Links to updated person quality are not refreshed in {personQualityDependent.GetType().Name}! ";
                        warnings.AppendLine(warning);
                    }
                }

                if (warnings.Length > 0)
                {
                    return new OperationResult(OperationResultStatus.Warning, warnings.ToString());
                }
            }

            return updateResult;
        }

        #endregion
    }
}