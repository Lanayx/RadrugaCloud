namespace Core.Interfaces.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CommonModels.Query;
    using CommonModels.Results;
    using Core.Interfaces.Repositories.Common;
    using DomainModels;

    /// <summary>
    /// The MissionRepository interface.
    /// </summary>
    public interface IMissionRepository : IDisposable, IPersonQualityDependent
    {
        #region Public Methods and Operators

        /// <summary>
        /// The add mission.
        /// </summary>
        /// <param name="mission">
        /// The mission.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IdResult> AddMission(Mission mission);

        /// <summary>
        /// The delete mission.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<OperationResult> DeleteMission(string id);

        /// <summary>
        /// Clears the links to mission set.
        /// </summary>
        /// <param name="missionIds">The mission ids.</param>
        /// <param name="missionSetId">The mission set id.</param>
        /// <returns>Task{OperationResult}.</returns>
        Task<OperationResult> SetMissionSetForMissions(List<string> missionIds, string missionSetId);

        /// <summary>
        /// The get mission.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<Mission> GetMission(string id);

        /// <summary>
        /// Gets the missions.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{Mission}}.</returns>
        Task<List<Mission>> GetMissions(QueryOptions<Mission> options);
        
        /// <summary>
        /// The update mission.
        /// </summary>
        /// <param name="mission">
        /// The mission.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<OperationResult> UpdateMission(Mission mission);

        #endregion
    }
}