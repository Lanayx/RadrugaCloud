namespace Services.DomainServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Interfaces.Repositories;

    /// <summary>
    ///     Class CommonPlacesService
    /// </summary>
    public class CommonPlacesService
    {
        #region Fields

        /// <summary>
        ///     The _common places repository
        /// </summary>
        private readonly ICommonPlacesRepository _commonPlacesRepository;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommonPlacesService" /> class.
        /// </summary>
        /// <param name="commonPlacesRepository">The common places repository.</param>
        public CommonPlacesService(ICommonPlacesRepository commonPlacesRepository)
        {
            _commonPlacesRepository = commonPlacesRepository;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>Task{AddResult}.</returns>
        public async Task<IdResult> AddAlias(CommonPlaceAlias alias)
        {
            return await _commonPlacesRepository.AddAlias(alias);
        }

        /// <summary>
        ///     Checks the alias exist.
        /// </summary>
        /// <param name="aliasName">Name of the alias.</param>
        /// <returns>Task{System.Boolean}.</returns>
        public async Task<bool> CheckAliasExist(string aliasName)
        {
            return await _commonPlacesRepository.CheckAliasExist(aliasName);
        }

        /// <summary>
        ///     Deletes the alias.
        /// </summary>
        /// <param name="aliasName">Name of the alias.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> DeleteAlias(string aliasName)
        {
            return await _commonPlacesRepository.DeleteAlias(aliasName);
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            _commonPlacesRepository.Dispose();
        }

        /// <summary>
        ///     Gets the alias.
        /// </summary>
        /// <param name="aliasName">Name of the alias.</param>
        /// <returns>Task{CommonPlaceAlias}.</returns>
        public async Task<CommonPlaceAlias> GetAlias(string aliasName)
        {
            return await _commonPlacesRepository.GetAlias(aliasName);
        }

        /// <summary>
        ///     Gets the aliases.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{CommonPlaceAlias}}.</returns>
        public async Task<IEnumerable<CommonPlaceAlias>> GetAliases(QueryOptions<CommonPlaceAlias> options)
        {
            return await _commonPlacesRepository.GetAliases(options);
        }

        /// <summary>
        ///     Updates the name of the aliase.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> UpdateAliaseName(CommonPlaceAlias alias)
        {
            return await _commonPlacesRepository.UpdateAliaseName(alias);
        }

        #endregion
    }
}