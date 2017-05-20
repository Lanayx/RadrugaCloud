namespace Core.Interfaces.Repositories
{
    using System;
    using System.Collections.Generic;
    using Core.CommonModels.Query;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.DomainModels;

    /// <summary>
    /// The KindActionRepository interface.
    /// </summary>
    public interface IKindActionRepository : IDisposable
    {
        #region Public Methods and Operators


        /// <summary>
        /// Adds the kind action.
        /// </summary>
        /// <param name="kindAction">The kind action.</param>
        /// <returns></returns>
        Task<IdResult> AddKindAction(KindAction kindAction);

        /// <summary>
        /// Removes the kind actions.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        Task RemoveKindActions(string userId);

        /// <summary>
        /// Gets the kind action.
        /// </summary>
        /// <param name="kindActionId">The kind action identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        Task<KindAction> GetKindAction(string kindActionId, string userId);


        /// <summary>
        /// Gets the kind actions.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        Task<List<KindAction>> GetKindActions(QueryOptions<KindAction> options);


        #endregion
        
        /// <summary>
        /// Updates the likes.
        /// </summary>
        /// <param name="kindAction">The kind action.</param>
        /// <returns></returns>
        Task<OperationResult> UpdateLikes(KindAction kindAction);

        /// <summary>
        /// Removes the duplicate kind actions.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        Task RemoveDuplicateKindActions(string userId);
    }
}