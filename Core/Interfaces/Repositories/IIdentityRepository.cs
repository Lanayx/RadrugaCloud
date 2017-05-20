namespace Core.Interfaces.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Core.AuthorizationModels;
    using Core.CommonModels.Query;
    using Core.CommonModels.Results;

    /// <summary>
    ///     Interface IUserIdentityRepository
    /// </summary>
    public interface IUserIdentityRepository: IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Adds the user identity.
        /// </summary>
        /// <param name="userIdentity">The user identity.</param>
        /// <returns>Task{AddResult}.</returns>
        Task<IdResult> AddUserIdentity(UserIdentity userIdentity);

        /// <summary>
        ///     Gets the user identity.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{UserIdentity}.</returns>
        Task<UserIdentity> GetUserIdentity(string id);

        /// <summary>
        ///     Gets the users identities.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{UserIdentity}}.</returns>
        Task<IEnumerable<UserIdentity>> GetUsersIdentities(QueryOptions<UserIdentity> options);

        /// <summary>
        ///     Updates the user identity.
        /// </summary>
        /// <param name="userIdentity">The user identity.</param>
        /// <returns>Task{OperationResult}.</returns>
        Task<OperationResult> UpdateUserIdentity(UserIdentity userIdentity);

        #endregion
    }
}