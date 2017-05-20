using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Memory
{
    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Interfaces.Repositories;

    using Infrastructure.InfrastructureTools;

    public class HintRequestRepository : IHintRequestRepository
    {
        /// <summary>
        ///     The all hint requests.
        /// </summary>
        private readonly List<HintRequest> _allHintRequests = new List<HintRequest>();

        public HintRequestRepository()
        {
            
        }            
        public async Task<IdResult> AddHintRequest(HintRequest hintRequest)
        {            
            await Task.Factory.StartNew(() => _allHintRequests.Add(hintRequest));
            return new IdResult(hintRequest.HintId);
        }

        public Task<List<HintRequest>> GetHintRequests(QueryOptions<HintRequest> options)
        {
            if (options == null)
            {
                return Task.Factory.StartNew(() => _allHintRequests);
            }
            return Task.Factory.StartNew(() => options.SimpleApply(_allHintRequests.AsQueryable()).ToList());
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            // nothing to dispose
        }
    }
}
