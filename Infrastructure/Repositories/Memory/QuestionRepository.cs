namespace Infrastructure.Repositories.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;
    using Core.Tools.CopyHelper;

    using InfrastructureTools;

    /// <summary>
    /// The question repository.
    /// </summary>
    public sealed class QuestionRepository : IQuestionRepository
    {
        #region Static Fields

        /// <summary>
        /// The all questions.
        /// </summary>
        private readonly List<Question> _allQuestions = new List<Question>();

        #endregion

        #region Constructors and Destructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionRepository"/> class.
        /// </summary>
        public QuestionRepository()
        {
            if (!_allQuestions.Any())
            {
                _allQuestions.Add(
                    new Question
                        {
                            Id = "Id_Question1",
                            Name = "Age test",
                            Text = "How old are you?",
                            Options =
                                new List<QuestionOption>
                                    {
                                        new QuestionOption
                                            {
                                                Text = "Before 13",
                                                Number = 1,
                                                PersonQualitiesWithScores =
                                                    GetPersonQualitiesWithScoresTriple(new List<int> { 1 })
                                            },
                                        new QuestionOption
                                            {
                                                Text = "13-19",
                                                Number = 2,
                                                PersonQualitiesWithScores =
                                                    GetPersonQualitiesWithScoresTriple(new List<int> { 0, 2 }),
                                            },
                                        new QuestionOption
                                            {
                                                Text = "Older then 19",
                                                Number = 3,
                                                PersonQualitiesWithScores =
                                                    GetPersonQualitiesWithScoresTriple(new List<int> { 2 })
                                            }
                                    }
                        });
                _allQuestions.Add(
                    new Question
                        {
                            Id = "Id_Question2",
                            Name = "Iq test",
                            Text = "What is your iq?",
                            Options =
                                new List<QuestionOption>
                                    {
                                        new QuestionOption
                                            {
                                                Text = "Below 90",
                                                Number = 1,
                                                PersonQualitiesWithScores =
                                                    GetPersonQualitiesWithScoresDouble(-1)
                                            },
                                        new QuestionOption
                                            {
                                                Text = "90-120",
                                                Number = 2,
                                                PersonQualitiesWithScores =
                                                    GetPersonQualitiesWithScoresDouble(1)
                                            },
                                        new QuestionOption
                                            {
                                                Text = "Over 120",
                                                Number = 3,
                                                PersonQualitiesWithScores =
                                                    GetPersonQualitiesWithScoresDouble(3)
                                            }
                                    }
                        });
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<IdResult> AddQuestion(Question question)
        {
            question.Id = Guid.NewGuid().ToString();
            await Task.Factory.StartNew(() => _allQuestions.Add(question));
            return new IdResult(question.Id);
        }

        /// <summary>
        /// The delete question.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<OperationResult> DeleteQuestion(string id)
        {
            var question = await GetQuestion(id);
            _allQuestions.Remove(question);
            return new OperationResult(OperationResultStatus.Success);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            // nothing to dispose
        }

        /// <summary>
        /// The get question.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<Question> GetQuestion(string id)
        {
            return Task.Factory.StartNew(() => _allQuestions.Find(question => question.Id == id));
        }

        /// <summary>
        /// The get questions.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<List<Question>> GetQuestions(QueryOptions<Question> options)
        {
            if (options == null)
            {
                return Task.Factory.StartNew(() => _allQuestions);
            }

            return Task.Factory.StartNew(() => options.SimpleApply(_allQuestions.AsQueryable()).ToList());
        }
       

        /// <summary>
        /// The update question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<OperationResult> UpdateQuestion(Question question)
        {
            var existingQuestion = await GetQuestion(question.Id);
            if (existingQuestion == null)
            {
                return OperationResult.NotFound;
            }

            question.CopyTo(existingQuestion);
            return new OperationResult(OperationResultStatus.Success);
        }

        #endregion

        #region Methods
        private List<PersonQualityIdWithScore> GetPersonQualitiesWithScoresTriple(List<int> positivePositions)
        {
            var personQualitiesWithScores = new List<PersonQualityIdWithScore>
                                                { 
                                                    new PersonQualityIdWithScore { PersonQualityId = "Id_target" ,Score = -1 },
                                                    new PersonQualityIdWithScore { PersonQualityId = "Id_young", Score = -1 },
                                                    new PersonQualityIdWithScore { PersonQualityId = "Id_old" , Score = -1 }
                                                };
            foreach (var position in positivePositions)
            {
                personQualitiesWithScores[position].Score = 1;
            }

            return personQualitiesWithScores;
        }

        private List<PersonQualityIdWithScore> GetPersonQualitiesWithScoresDouble(double score)
        {
            var personQualitiesWithScores = new List<PersonQualityIdWithScore>
                                                { 
                                                    new PersonQualityIdWithScore { PersonQualityId = "PersonQuality1", Score = score },
                                                    new PersonQualityIdWithScore { PersonQualityId = "PersonQuality2", Score = score }
                                                };

            return personQualitiesWithScores;
        } 

        #endregion

        /// <summary>
        ///     Removes the type of the links to deleted person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <returns>Task{OperationResult}.</returns>
        public Task<OperationResult> RemoveLinksToDeletedPersonQuality(string personQualityId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Updates the type of the links to person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <param name="personQualityName">Name of the person quality.</param>
        /// <returns>Task{OperationResult}.</returns>
        public Task<OperationResult> UpdateLinksToPersonQuality(string personQualityId, string personQualityName)
        {
            throw new NotImplementedException();
        }
    }
}