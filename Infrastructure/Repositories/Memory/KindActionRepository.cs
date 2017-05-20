namespace Infrastructure.Repositories.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.Tools.CopyHelper;

    using Infrastructure.InfrastructureTools;

    /// <summary>
    /// Class KindActionRepository
    /// </summary>
    public sealed class KindActionRepository : IKindActionRepository
    {
        #region Static Fields

        /// <summary>
        /// All mission drafts
        /// </summary>
        private readonly List<KindAction> KindActions = new List<KindAction>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KindActionRepository"/> class.
        /// </summary>
        public KindActionRepository()
        {
            if (!KindActions.Any())
            {
                KindActions.Add(
                    new KindAction
                        {
                            Id = "Id_KindAction1",
                            UserId = "User1Id",
                            Description = "Today I managed to clean up my computer",
                            DateAdded = DateTime.UtcNow
                        });
                KindActions.Add(
                    new KindAction
                    {
                        Id = "Id_KindAction2",
                        UserId = "User2Id",
                        Description = "Today I managed to clean up my computer",
                        DateAdded = DateTime.UtcNow,
                        Likes = new List<string> { "User1Id" }
                    });
                KindActions.Add(
                    new KindAction
                    {
                        Id = "Id_KindAction3",
                        UserId = "User2Id",
                        Description = "Today I managed to clean up my computer",
                        DateAdded = DateTime.UtcNow,
                        Likes = new List<string> { "User3Id" }
                    });
            }
        }

        #endregion

        /// <summary>
        /// Adds the kind action.
        /// </summary>
        /// <param name="kindAction">The kind action.</param>
        /// <returns>Task{AddResult}.</returns>
        public Task<IdResult> AddKindAction(KindAction kindAction)
        {
            kindAction.Id = Guid.NewGuid().ToString();
            KindActions.Add(kindAction);
            return Task.FromResult(new IdResult(kindAction.Id));
        }

        /// <summary>
        /// Removes the kind actions.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public Task RemoveKindActions(string userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the kind action.
        /// </summary>
        /// <param name="kindActionId">The kind action identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public Task<KindAction> GetKindAction(string kindActionId, string userId)
        {
            return Task.FromResult(KindActions.FirstOrDefault(kindAction => kindAction.Id == kindActionId));
        }

        /// <summary>
        /// Gets the kind actions.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task{IEnumerable{KindAction}}.</returns>
        public Task<List<KindAction>> GetKindActions(Core.CommonModels.Query.QueryOptions<KindAction> options)
        {
            if (options == null)
            {
                return Task.FromResult(KindActions);
            }

            return Task.FromResult(options.SimpleApply(KindActions.AsQueryable()).ToList());
        }

        /// <summary>
        /// Updates the specified kind action.
        /// </summary>
        /// <param name="kindAction">The kind action.</param>
        /// <returns></returns>
        public async Task<OperationResult> UpdateLikes(KindAction kindAction)
        {
            var existingKindAction = await GetKindAction(kindAction.Id, kindAction.UserId);
            if (existingKindAction == null)
            {
                return OperationResult.NotFound;
            }

            kindAction.CopyTo(existingKindAction);
            return new OperationResult(OperationResultStatus.Success);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // nothing to dispose
        }

        public Task RemoveDuplicateKindActions(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
