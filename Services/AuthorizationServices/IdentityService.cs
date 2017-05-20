namespace Services.AuthorizationServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Core.AuthorizationModels;
    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.Interfaces.Repositories;

    /// <summary>
    ///     Service for operating with user identity
    /// </summary>
    public class UserIdentityService
    {
        #region Fields

        private readonly IUserIdentityRepository _identityRepository;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserIdentityService" /> class.
        /// </summary>
        /// <param name="identityRepository">The identity repository.</param>
        public UserIdentityService(IUserIdentityRepository identityRepository)
        {
            _identityRepository = identityRepository;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the user identity.
        /// </summary>
        /// <param name="userIdentity">The user identity.</param>
        /// <returns>Task{AddResult}.</returns>
        public Task<IdResult> AddUserIdentity(UserIdentity userIdentity)
        {
            return _identityRepository.AddUserIdentity(userIdentity);
        }

        /// <summary>
        ///     Gets the users identity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task{UserIdentity}.</returns>
        public Task<UserIdentity> GetUserIdentity(string id)
        {
            return _identityRepository.GetUserIdentity(id);
        }

        /// <summary>
        ///     Gets the user's identities.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{UserIdentity}}.</returns>
        public Task<IEnumerable<UserIdentity>> GetUsersIdentities(QueryOptions<UserIdentity> options = null)
        {
            return _identityRepository.GetUsersIdentities(options);
        }

        /// <summary>
        ///     Updates the user identity.
        /// </summary>
        /// <param name="userIdentity">The user identity.</param>
        /// <returns>Task{AddResult}.</returns>
        public Task<OperationResult> UpdateUserIdentity(UserIdentity userIdentity)
        {
            return _identityRepository.UpdateUserIdentity(userIdentity);
        }

        #endregion
    }
}