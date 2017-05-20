namespace Infrastructure.Repositories.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.AuthorizationModels;
    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.Interfaces.Repositories;
    using Core.Tools;

    using Infrastructure.InfrastructureTools;

    /// <summary>
    ///     Class UserIdentityRepository
    /// </summary>
    public class UserIdentityRepository : IUserIdentityRepository
    {
        #region Static Fields

        private readonly List<UserIdentity> AllUserIdentities = new List<UserIdentity>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserIdentityRepository" /> class.
        /// </summary>
        public UserIdentityRepository()
        {
            if (!AllUserIdentities.Any())
            {
                AllUserIdentities.Add(
                    new UserIdentity
                        {
                            Id = "User1Id", 
                            LoginEmail = "User1Login", 
                            HashedPassword =
                                HashHelper.GetPasswordHash("User1Password"), 
                            HashType = HashHelper.CurrentHashType
                        });
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds the user identity.
        /// </summary>
        /// <param name="userIdentity">The user identity.</param>
        /// <returns>Task{AddResult}.</returns>
        public async Task<IdResult> AddUserIdentity(UserIdentity userIdentity)
        {
            userIdentity.Id = Guid.NewGuid().ToString();
            await Task.Factory.StartNew(() => AllUserIdentities.Add(userIdentity));
            return new IdResult(userIdentity.Id);
        }

        /// <summary>
        /// Gets the user identiy.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task{UserIdentity}.</returns>
        public Task<UserIdentity> GetUserIdentity(string id)
        {
            return Task.Factory.StartNew(() => AllUserIdentities.Find(uid => uid.Id == id));
        }

        /// <summary>
        /// Gets the identities.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{UserIdentity}}.</returns>
        public Task<IEnumerable<UserIdentity>> GetUsersIdentities(QueryOptions<UserIdentity> options)
        {
            if (options == null)
            {
                return Task.Factory.StartNew(() => AllUserIdentities.AsEnumerable());
            }

            return Task.Factory.StartNew(() => options.SimpleApply(AllUserIdentities.AsQueryable()).AsEnumerable());
        }

        /// <summary>
        /// Updates the user identity.
        /// </summary>
        /// <param name="userIdentity">The user identity.</param>
        /// <returns>Task{AddResult}.</returns>
        public async Task<OperationResult> UpdateUserIdentity(UserIdentity userIdentity)
        {
            await Task.Factory.StartNew(() => { });
            return new IdResult(userIdentity.Id);
        }


        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            // nothing to dispose
        }

        #endregion
    }
}