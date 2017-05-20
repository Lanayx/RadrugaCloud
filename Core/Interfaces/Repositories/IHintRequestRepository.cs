namespace Core.Interfaces.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    /// <summary>
    /// The HintRequestRepository interface
    /// </summary>
    public interface IHintRequestRepository : IDisposable
    {
        /// <summary>
        /// Adds the hint request.
        /// </summary>
        /// <param name="hintRequest">The hint request.</param>
        /// <returns></returns>
        Task<IdResult> AddHintRequest(HintRequest hintRequest);
        /// <summary>
        /// Gets the hint requests.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        Task<List<HintRequest>> GetHintRequests(QueryOptions<HintRequest> options);
    }
}
