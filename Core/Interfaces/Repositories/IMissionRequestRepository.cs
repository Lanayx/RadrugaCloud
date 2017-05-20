namespace Core.Interfaces.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;

    /// <summary>
    /// The MissionRequestRepository interface.
    /// </summary>
    public interface IMissionRequestRepository : IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        /// The add mission request.
        /// </summary>
        /// <param name="missionRequest">
        /// The mission request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IdResult> AddMissionRequest(MissionRequest missionRequest);

        /// <summary>
        /// The delete mission request.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<OperationResult> DeleteMissionRequest(string id);

        /// <summary>
        /// The get mission request.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<MissionRequest> GetMissionRequest(string id);
       

        /// <summary>
        /// The get mission requests.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The <see cref="Task" />.</returns>
        Task<List<MissionRequest>> GetMissionRequests(QueryOptions<MissionRequest> options);

        /// <summary>
        /// The update mission request.
        /// </summary>
        /// <param name="missionRequest">
        /// The mission request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<OperationResult> UpdateMissionRequest(MissionRequest missionRequest);

        #endregion
    }
}