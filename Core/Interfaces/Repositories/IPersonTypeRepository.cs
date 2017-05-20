namespace Core.Interfaces.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;

    /// <summary>
    /// The PersonQualityRepository interface.
    /// </summary>
    public interface IPersonQualityRepository : IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        /// Adds the person quality.
        /// </summary>
        /// <param name="personQuality">The personQuality.</param>
        /// <returns>Task{AddResult}.</returns>
        Task<IdResult> AddPersonQuality(PersonQuality personQuality);

        /// <summary>
        /// Deletes the person quality.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task{OperationResult}.</returns>
        Task<OperationResult> DeletePersonQuality(string id);

        /// <summary>
        /// Gets the person quality.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task{PersonQuality}.</returns>
        Task<PersonQuality> GetPersonQuality(string id);

        /// <summary>
        /// Gets the person qualities.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{PersonQuality}}.</returns>
        Task<List<PersonQuality>> GetPersonQualities(QueryOptions<PersonQuality> options);

        /// <summary>
        /// Updates the person quality.
        /// </summary>
        /// <param name="personQuality">The person quality.</param>
        /// <returns>Task{OperationResult}.</returns>
        Task<OperationResult> UpdatePersonQuality(PersonQuality personQuality);

        #endregion
    }
}