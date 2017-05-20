namespace Core.Interfaces.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Interfaces.Repositories.Common;

    /// <summary>
    ///     The MissionDraftRepository interface.
    /// </summary>
    public interface IMissionDraftRepository : IDisposable, IPersonQualityDependent
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Adds the mission draft.
        /// </summary>
        /// <param name="missionDraft">The mission draft.</param>
        /// <returns>Task{AddResult}.</returns>
        Task<IdResult> AddMissionDraft(MissionDraft missionDraft);

        /// <summary>
        /// Deletes the mission draft.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task{OperationResult}.</returns>
        Task<OperationResult> DeleteMissionDraft(string id);

        /// <summary>
        ///     Gets the mission draft.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task{MissionDraft}.</returns>
        Task<MissionDraft> GetMissionDraft(string id);

        /// <summary>
        ///     Gets the mission drafts.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{MissionDraft}}.</returns>
        Task<List<MissionDraft>> GetMissionDrafts(QueryOptions<MissionDraft> options);

        /// <summary>
        ///     Updates the mission draft.
        /// </summary>
        /// <param name="missionDraft">The mission draft.</param>
        /// <returns>Task{OperationResult}.</returns>
        Task<OperationResult> UpdateMissionDraft(MissionDraft missionDraft);

        #endregion
    }
}