namespace Core.Interfaces.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;

    /// <summary>
    /// Interface IMissionSetRepository
    /// </summary>
    public interface IMissionSetRepository : IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        /// Adds the mission set.
        /// </summary>
        /// <param name="missionSet">The mission set.</param>
        /// <returns>Task{AddResult}.</returns>
        Task<IdResult> AddMissionSet(MissionSet missionSet);

        /// <summary>
        /// Deletes the mission set.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{OperationResult}.</returns>
        Task<OperationResult> DeleteMissionSet(string id);
        
        /// <summary>
        /// Gets the mission set.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{MissionSet}.</returns>
        Task<MissionSet> GetMissionSet(string id);

        /// <summary>
        /// Gets the mission sets.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{MissionSet}}.</returns>
        Task<List<MissionSet>> GetMissionSets(QueryOptions<MissionSet> options);

        /// <summary>
        /// Updates the mission set.
        /// </summary>
        /// <param name="missionSet">The mission set.</param>
        /// <returns>Task{OperationResult}.</returns>
        Task<OperationResult> UpdateMissionSet(MissionSet missionSet);

        /// <summary>
        ///     Refreshes the mission dependent links.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{OperationResult}.</returns>
        Task<OperationResult> RefreshMissionDependentLinks(string id);

        #endregion
    }
}