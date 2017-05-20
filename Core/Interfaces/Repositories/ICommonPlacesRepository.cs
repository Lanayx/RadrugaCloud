namespace Core.Interfaces.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;

    /// <summary>
    ///     Interface ISharedPropertiesRepository
    /// </summary>
    public interface ICommonPlacesRepository : IDisposable
    {
        /// <summary>
        ///     Adds the alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>Task{AddResult}.</returns>
        Task<IdResult> AddAlias(CommonPlaceAlias alias);

        /// <summary>
        ///     Adds the temp common place.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="commonPlaceAlias">The common place alias.</param>
        /// <param name="coordinate">The coordinate.</param>
        /// <returns>Task.</returns>
        Task<IdResult> AddCommonPlace(string userId, string commonPlaceAlias, GeoCoordinate coordinate);

        /// <summary>
        ///     Checks the alias exist.
        /// </summary>
        /// <param name="aliasName">Name of the alias.</param>
        /// <returns>Task{System.Boolean}.</returns>
        Task<bool> CheckAliasExist(string aliasName);

        /// <summary>
        ///     Deletes the alias.
        /// </summary>
        /// <param name="aliasName">Name of the alias.</param>
        /// <returns>Task{OperationResult}.</returns>
        Task<OperationResult> DeleteAlias(string aliasName);

        /// <summary>
        ///     Gets the alias.
        /// </summary>
        /// <param name="aliasName">Name of the alias.</param>
        /// <returns>Task{CommonPlaceAlias}.</returns>
        Task<CommonPlaceAlias> GetAlias(string aliasName);

        /// <summary>
        ///     Gets the aliases.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{CommonPlaceAlias}}.</returns>
        Task<IEnumerable<CommonPlaceAlias>> GetAliases(QueryOptions<CommonPlaceAlias> options);

        /// <summary>
        ///     Gets the common place.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="aliasName">The common place alias.</param>
        /// <returns>CommonPlace.</returns>
        Task<CommonPlace> GetCommonPlaceByAlias(string userId, string aliasName);

        /// <summary>
        ///     Gets the common places by alias.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="commonPlaceAlias">The common place alias.</param>
        /// <returns>Task{IEnumerable{CommonPlace}}.</returns>
        Task<IEnumerable<CommonPlace>> GetCommonPlacesByAlias(string userId, string commonPlaceAlias);

        /// <summary>
        ///     Updates the name of the aliase.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>Task{OperationResult}.</returns>
        Task<OperationResult> UpdateAliaseName(CommonPlaceAlias alias);
    }
}