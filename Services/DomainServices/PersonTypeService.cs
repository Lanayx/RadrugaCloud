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
    public sealed class PersonQualityService
    {
        #region Fields

        /// <summary>
        /// The _mission draft repository.
        /// </summary>
        private readonly IPersonQualityRepository _personQualityRepository;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonQualityService"/> class.
        /// </summary>
        /// <param name="personQualityRepository">
        /// The mission draft repository.
        /// </param>
        public PersonQualityService(IPersonQualityRepository personQualityRepository)
        {
            _personQualityRepository = personQualityRepository;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add new mission draft.
        /// </summary>
        /// <param name="personQuality">
        /// The mission draft.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<IdResult> AddNewPersonQuality(PersonQuality personQuality)
        {
            return _personQualityRepository.AddPersonQuality(personQuality);
        }

        /// <summary>
        /// The delete mission draft.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<OperationResult> DeletePersonQuality(string id)
        {
            return _personQualityRepository.DeletePersonQuality(id);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            _personQualityRepository.Dispose();
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
        public Task<PersonQuality> GetPersonQuality(string id)
        {
            return _personQualityRepository.GetPersonQuality(id);
        }

        /// <summary>
        /// The get mission drafts.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<List<PersonQuality>> GetPersonQualities(QueryOptions<PersonQuality> options = null)
        {
            return _personQualityRepository.GetPersonQualities(options);
        }

        /// <summary>
        /// The update mission draft.
        /// </summary>
        /// <param name="personQuality">
        /// The mission draft.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<OperationResult> UpdatePersonQuality(PersonQuality personQuality)
        {
            return _personQualityRepository.UpdatePersonQuality(personQuality);
        }

        #endregion
    }
}