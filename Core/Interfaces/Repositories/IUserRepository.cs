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
    /// The UserRepository interface.
    /// </summary>
    public interface IUserRepository : IDisposable, IPersonQualityDependent
    {
        #region Public Methods and Operators

        /// <summary>
        /// The add user.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IdResult> AddUser(User user);

        /// <summary>
        /// The get user.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<User> GetUser(string id);


        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        Task<IEnumerable<User>> GetUsers(QueryOptions<User> options);

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="replace">if set to <c>true</c> [replace].</param>
        /// <returns>Task{OperationResult}.</returns>
        Task<OperationResult> UpdateUser(User user, bool replace = true);


        /// <summary>
        /// Decreases the kind action scales for all users.
        /// </summary>
        /// <returns></returns>
        Task<OperationResult> DecreaseKindActionScales();


        /// <summary>
        /// Updates the last ratings places for all users.
        /// </summary>
        /// <returns></returns>
        Task<OperationResult> UpdateLastRatingsPlaces();

        #endregion
    }
}