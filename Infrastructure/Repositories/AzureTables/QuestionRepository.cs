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

    using Core.Tools;

    /// <summary>
    ///     Class QuestionRepository
    /// </summary>
    public sealed class QuestionRepository : IQuestionRepository
    {
        #region Fields

        /// <summary>
        ///     The _azure manager
        /// </summary>
        private readonly AzureTableStorageManager _azureManager;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="QuestionRepository" /> class.
        /// </summary>
        public QuestionRepository()
        {
            _azureManager = new AzureTableStorageManager(AzureTableName.Questions);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns>Task{AddResult}.</returns>
        public async Task<IdResult> AddQuestion(Question question)
        {
            question.Id = Guid.NewGuid().ToString("N");
            var azureModel = question.ToAzureModel();
            var options = GenerateOptions(question);

            var batch = new List<QuestionAzure> { azureModel };

            if (options.Any())
            {
                batch.AddRange(options);
            }

            return await _azureManager.AddEntityBatchAsync(batch);
        }

        /// <summary>
        ///     Deletes the question.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> DeleteQuestion(string id)
        {
            var entities = await _azureManager.GetEntitiesForCompleteDelete<QuestionAzure>(id);
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
        ///     Gets the question.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task{Question}.</returns>
        public async Task<Question> GetQuestion(string id)
        {
            var relatedEntities =
                await _azureManager.GetEntitiesAsync(new TableQuery<QuestionAzure>().Where(GetFilterByPartitionKey(id)));
            return ConvertToQuestion(relatedEntities, true);
        }

        /// <summary>
        ///     Gets the questions.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{Question}}.</returns>
        public async Task<List<Question>> GetQuestions(QueryOptions<Question> options)
        {
            string expandFilter;
            bool needExpand = CheckExpandGetFilter(options.Expand, out expandFilter);
            var tableQuery = options.GenerateTableQuery<Question, QuestionAzure>(expandFilter);

            var relatedEntities = await _azureManager.GetEntitiesAsync(tableQuery);

            var questions =
                relatedEntities.GroupBy(m => m.PartitionKey)
                    .Select(group => ConvertToQuestion(group.ToList(), needExpand)).ToList();
            return questions.FilterCollectionPostFactum(options);
        }

        /// <summary>
        ///     Removes the type of the links to deleted person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> RemoveLinksToDeletedPersonQuality(string personQualityId)
        {
            var azureQuestions = await _azureManager.GetEntitiesAsync(new TableQuery<QuestionAzure>());
            var entitiesToDelete =
                azureQuestions.Where(m => m.IsPersonQualityLink && string.Equals(m.PersonQualityId, personQualityId));
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

            if (error.Capacity <= 0)
            {
                return new OperationResult(OperationResultStatus.Success);
            }

            return new OperationResult(OperationResultStatus.Error, error.ToString());
        }

        /// <summary>
        /// Sets next question.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        public Task<OperationResult> SetNextQuestion(User user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Updates the type of the links to person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <param name="personQualityName">Name of the person quality.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> UpdateLinksToPersonQuality(string personQualityId, string personQualityName)
        {
            var azureQuestions = await _azureManager.GetEntitiesAsync(new TableQuery<QuestionAzure>());
            var entitiesToUpdate =
                azureQuestions.Where(m => m.IsPersonQualityLink && string.Equals(m.PersonQualityId, personQualityId)).ToIList();
            foreach (var azureQuestion in entitiesToUpdate)
            {
                azureQuestion.PersonQualityName = personQualityName;
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

            return error.Capacity <= 0 
                ? new OperationResult(OperationResultStatus.Success) 
                : new OperationResult(OperationResultStatus.Error, error.ToString());
        }

        /// <summary>
        ///     Updates the question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> UpdateQuestion(Question question)
        {
            var relatedEntities =
                await
                _azureManager.GetEntitiesAsync(
                    new TableQuery<QuestionAzure>().Where(GetFilterByPartitionKey(question.Id)));
            var questionAzure = relatedEntities.FirstOrDefault(m => m.IsQuestion);
            var newQuestion = question.ToAzureModel();
            newQuestion.CopyToTableEntity(questionAzure);
            var entitiestToDelete = relatedEntities.Where(m => m.IsPersonQualityLink || m.IsOptionLink).ToList();
            var entitiesToAdd = GenerateOptions(question);
            var entitiesToUpdate = FilterUpdatableEntities(ref entitiesToAdd, ref entitiestToDelete);
            entitiesToUpdate.Add(questionAzure);
            return await _azureManager.UpdateEntityBatchAsync(entitiesToUpdate, entitiesToAdd, entitiestToDelete);
        }

        #endregion

        #region Methods

        private bool CheckExpandGetFilter(IEnumerable<string> expand, out string expandFilter)
        {
            // if expand is not empty, related entites should be selected so we don't need additional filter
            if (expand != null
                && expand.Any(e => string.Equals(e, "Options", StringComparison.InvariantCultureIgnoreCase)))
            {
                expandFilter = string.Empty;
                return true;
            }

            // if expand is empty, we need to add row filter to NOT select related entities
            expandFilter = TableQuery.GenerateFilterCondition(
                AzureTableConstants.RowKey, 
                QueryComparisons.Equal, 
                AzureTableConstants.QuestionRowKey);
            return false;
        }

        private Question ConvertToQuestion(IList<QuestionAzure> relatedEntities, bool needExpand)
        {
            var questionAzure = relatedEntities.FirstOrDefault(u => u.IsQuestion);
            var user = questionAzure.FromAzureModel();
            if (user == null)
            {
                return null;
            }

            if (needExpand)
            {
                var optionsResult = new List<QuestionOption>();
                var options = relatedEntities.Where(r => r.IsOptionLink).ToIList();
                var optionsPersonQualities =
                    relatedEntities.Where(r => r.IsPersonQualityLink).GroupBy(r => r.PersonQualityOptionNumber).ToIList();
                foreach (var option in options)
                {
                    var optionToAdd = new QuestionOption
                                          {
                                              Number = (byte)option.OptionNumber, 
                                              Text = option.OptionText,
                                              NextQuestionId = option.NextQuestionId
                                          };
                    var links = optionsPersonQualities.FirstOrDefault(g => g.Key == optionToAdd.Number);
                    if (links != null)
                    {
                        optionToAdd.PersonQualitiesWithScores =
                            links.Select(
                                u =>
                                new PersonQualityIdWithScore
                                    {
                                        Score = u.PersonQualityScore, 
                                        PersonQualityId = u.PersonQualityId 
                                    }).ToList();
                    }

                    optionsResult.Add(optionToAdd);
                }

                if (optionsResult.Any())
                {
                    user.Options = optionsResult;
                }
            }

            return user;
        }

        private List<QuestionAzure> FilterUpdatableEntities(
            ref List<QuestionAzure> entitiesToAdd, 
            ref List<QuestionAzure> entitiestToDelete)
        {
            var toDelete = entitiestToDelete;
            var sameLinks = entitiesToAdd.Where(l => toDelete.Any(dl => dl.RowKey == l.RowKey)).ToList();
            entitiesToAdd = entitiesToAdd.Where(l => sameLinks.All(sl => sl.RowKey != l.RowKey)).ToList();
            entitiestToDelete = entitiestToDelete.Where(l => sameLinks.All(sl => sl.RowKey != l.RowKey)).ToList();
            foreach (var sameLink in sameLinks)
            {
                sameLink.ETag = toDelete.First(e => e.RowKey == sameLink.RowKey).ETag;
            }
            return sameLinks;

        }

        private List<QuestionAzure> GenerateOptions(Question question)
        {
            var result = new List<QuestionAzure>();
            if (question.Options != null)
            {
                foreach (var questionOption in question.Options)
                {
                    var optionLink = QuestionAzure.CreateOption(questionOption, question.Id);
                    result.Add(optionLink);
                    var personQualityLinks = GeneratePersonQualityLinks(questionOption, question.Id);
                    if (personQualityLinks.Any())
                    {
                        result.AddRange(personQualityLinks);
                    }
                }
            }

            return result;
        }

        private List<QuestionAzure> GeneratePersonQualityLinks(QuestionOption option, string questionId)
        {
            if (option.PersonQualitiesWithScores == null)
            {
                return new List<QuestionAzure>();
            }

            return
                option.PersonQualitiesWithScores.Where(
                    t =>
                    !string.IsNullOrEmpty(t.PersonQualityId))
                    .Select(
                        t =>
                        QuestionAzure.CreateLinkToPersonQuality(
                            questionId, 
                            option.Number, 
                            t.PersonQualityId, 
                            t.Score))
                    .ToList();
        }

        private string GetFilterByPartitionKey(string partitionKey)
        {
            return TableQuery.GenerateFilterCondition(
                AzureTableConstants.PartitionKey, 
                QueryComparisons.Equal, 
                partitionKey);
        }

        #endregion
    }
}