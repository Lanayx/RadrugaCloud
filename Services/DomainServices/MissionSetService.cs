namespace Services.DomainServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Interfaces.Repositories;

    /// <summary>
    /// Class MissionSetService
    /// </summary>
    public sealed class MissionSetService
    {
        #region Fields

        private readonly IMissionSetRepository _missionSetRepository;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MissionSetService" /> class.
        /// </summary>
        /// <param name="missionSetRepository">The MissionSet repository.</param>
        public MissionSetService(IMissionSetRepository missionSetRepository)
        {
            _missionSetRepository = missionSetRepository;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the new MissionSet.
        /// </summary>
        /// <param name="misionSet">The mision.</param>
        /// <returns>Task{AddResult}.</returns>
        public Task<IdResult> AddNewMissionSet(MissionSet misionSet)
        {
            return _missionSetRepository.AddMissionSet(misionSet);
        }

        /// <summary>
        ///     Deletes the MissionSet.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{OperationResult}.</returns>
        public Task<OperationResult> DeleteMissionSet(string id)
        {
            return _missionSetRepository.DeleteMissionSet(id);
        }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            _missionSetRepository.Dispose();
        }

        /// <summary>
        ///     Gets the MissionSet.
        /// </summary>
        /// <param name="missionSetId">The MissionSet id.</param>
        /// <returns>Task{MissionSet}.</returns>
        public async Task<MissionSet> GetMissionSet(string missionSetId)
        {
            return await _missionSetRepository.GetMissionSet(missionSetId);
        }

        /// <summary>
        ///     Gets the MissionSets.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{MissionSet}}.</returns>
        public Task<List<MissionSet>> GetMissionSets(QueryOptions<MissionSet> options = null)
        {
            return _missionSetRepository.GetMissionSets(options);
        }

        /// <summary>
        ///     Updates the MissionSet.
        /// </summary>
        /// <param name="mision">The mision.</param>
        /// <returns>Task{OperationResult}.</returns>
        public Task<OperationResult> UpdateMissionSet(MissionSet mision)
        {
            return _missionSetRepository.UpdateMissionSet(mision);
        }

        #endregion
    }
}