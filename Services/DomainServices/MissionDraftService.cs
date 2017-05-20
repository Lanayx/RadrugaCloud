namespace Services.DomainServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Interfaces.Repositories;

    /// <summary>
    /// The mission draft service.
    /// </summary>
    public sealed class MissionDraftService
    {
        #region Fields

        /// <summary>
        /// The _mission draft repository.
        /// </summary>
        private readonly IMissionDraftRepository _missionDraftRepository;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MissionDraftService"/> class.
        /// </summary>
        /// <param name="missionDraftRepository">
        /// The mission draft repository.
        /// </param>
        public MissionDraftService(IMissionDraftRepository missionDraftRepository)
        {
            _missionDraftRepository = missionDraftRepository;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add new mission draft.
        /// </summary>
        /// <param name="missionDraft">
        /// The mission draft.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<IdResult> AddNewMissionDraft(MissionDraft missionDraft)
        {
            return _missionDraftRepository.AddMissionDraft(missionDraft);
        }

        /// <summary>
        /// The delete mission draft.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<OperationResult> DeleteMissionDraft(string id)
        {
            return _missionDraftRepository.DeleteMissionDraft(id);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            _missionDraftRepository.Dispose();
        }

        /// <summary>
        /// The get mission draft.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<MissionDraft> GetMissionDraft(string id)
        {
            return _missionDraftRepository.GetMissionDraft(id);
        }

        /// <summary>
        /// The get mission drafts.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<List<MissionDraft>> GetMissionDrafts(QueryOptions<MissionDraft> options = null)
        {
            return _missionDraftRepository.GetMissionDrafts(options);
        }

        /// <summary>
        /// The update mission draft.
        /// </summary>
        /// <param name="missionDraft">
        /// The mission draft.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<OperationResult> UpdateMissionDraft(MissionDraft missionDraft)
        {
            return _missionDraftRepository.UpdateMissionDraft(missionDraft);
        }

        #endregion
    }
}