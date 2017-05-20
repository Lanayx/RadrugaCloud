namespace Core.Interfaces.Repositories
{
    using System;
    using System.Collections.Generic;
    using Core.CommonModels.Query;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Interfaces.Repositories.Common;

    /// <summary>
    /// The QuestionRepository interface.
    /// </summary>
    public interface IQuestionRepository : IDisposable, IPersonQualityDependent
    {
        #region Public Methods and Operators


        /// <summary>
        /// Adds the question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns></returns>
        Task<IdResult> AddQuestion(Question question);


        /// <summary>
        /// Deletes the question.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<OperationResult> DeleteQuestion(string id);


        /// <summary>
        /// Gets the question.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<Question> GetQuestion(string id);


        /// <summary>
        /// Gets the questions.
        /// </summary>
        /// <returns></returns>
        Task<List<Question>> GetQuestions(QueryOptions<Question> options);


        /// <summary>
        /// Updates the question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns></returns>
        Task<OperationResult> UpdateQuestion(Question question);
        
        #endregion
    }
}