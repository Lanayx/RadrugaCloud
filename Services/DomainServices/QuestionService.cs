namespace Services.DomainServices
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;

    using Services.BL;

    /// <summary>
    ///     The question service.
    /// </summary>
    public sealed class QuestionService
    {
        private readonly AssignmentService _assignmentService;

        private readonly IMissionRepository _missionRepository;

        /// <summary>
        ///     The _question repository.
        /// </summary>
        private readonly IQuestionRepository _questionRepository;

        /// <summary>
        ///     The _user repository
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="QuestionService" /> class.
        /// </summary>
        /// <param name="questionRepository">The question repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="missionRepository">The mission repository.</param>
        /// <param name="assignmentService">The assignment service.</param>
        public QuestionService(
            IQuestionRepository questionRepository,
            IUserRepository userRepository,
            IMissionRepository missionRepository,
            AssignmentService assignmentService)
        {
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _assignmentService = assignmentService;
            _missionRepository = missionRepository;
        }

        /// <summary>
        ///     Adds new question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task<IdResult> AddNewQuestion(Question question)
        {
            return _questionRepository.AddQuestion(question);
        }

        /// <summary>
        ///     User has answered the question.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="questionsAnswers">The questions answers.</param>
        /// <returns>Task{OperationResult}.</returns>
        /// <exception cref="System.ArgumentNullException">Wrong input</exception>
        public async Task<ColorResult> AnswerQuestions(string userId, PersonQualityIdWithScore[] questionsAnswers)
        {
            //TODO check if user has already answered questions
            if (string.IsNullOrEmpty(userId) || questionsAnswers == null)
            {
                throw new ArgumentNullException(
                    string.IsNullOrEmpty(userId) ? "userId" : "questionsAnswers",
                    "Wrong input");
            }

            var user = await _userRepository.GetUser(userId);
            if (user == null)
            {
                return (ColorResult)OperationResult.NotFound;
            }

            RewardsCalculator.UpdateUserAfterAnsweringQuestion(questionsAnswers, user);
            await _assignmentService.AssignMissionSetLine(user);
            await RewardsCalculator.SetNewMissionSets(user, _missionRepository);
            
            RewardsCalculator.UpdateRadrugaColor(user, true);

            var userUpdateResult = await _userRepository.UpdateUser(user);
            return userUpdateResult.Status == OperationResultStatus.Success
                       ? new ColorResult(user.RadrugaColor)
                       : (ColorResult)userUpdateResult;
        }

        /// <summary>
        ///     Deletes the question.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task{OperationResult}.</returns>
        public Task<OperationResult> DeleteQuestion(string id)
        {
            return _questionRepository.DeleteQuestion(id);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            _questionRepository.Dispose();
        }

        /// <summary>
        ///     Gets the question.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task{Question}.</returns>
        public Task<Question> GetQuestion(string id)
        {
            return _questionRepository.GetQuestion(id);
        }

        /// <summary>
        ///     Gets the questions.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{Question}}.</returns>
        public Task<List<Question>> GetQuestions(QueryOptions<Question> options = null)
        {
            return _questionRepository.GetQuestions(options);
        }

        /// <summary>
        ///     Updates the question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns>Task{OperationResult}.</returns>
        public Task<OperationResult> UpdateQuestion(Question question)
        {
            return _questionRepository.UpdateQuestion(question);
        }
    }
}