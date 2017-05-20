namespace Infrastructure.Repositories.AzureTables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;

    using Infrastructure.AzureTablesObjects;
    using Infrastructure.AzureTablesObjects.TableEntities;

    public sealed class HintRequestRepository : IHintRequestRepository
    {
        #region Fields

        private readonly AzureTableStorageManager _azureManager;

        #endregion

        #region Constructors and Destructors

        public HintRequestRepository()
        {
            _azureManager = new AzureTableStorageManager(AzureTableName.HintRequests);            
        }

        #endregion

        #region Public Methods and Operators
        /// <summary>
        /// Adds the hint request.
        /// </summary>
        /// <param name="hintRequest">The hint request.</param>
        /// <returns></returns>
        public async Task<IdResult> AddHintRequest(HintRequest hintRequest)
        {            
            return await _azureManager.AddEntityAsync(hintRequest.ToAzureModel());
        }

        /// <summary>
        /// Return the list of hint requests.
        /// </summary>
        /// <param name="options">Options for table query.</param>
        /// <returns></returns>
        public async Task<List<HintRequest>> GetHintRequests(QueryOptions<HintRequest> options)
        {
            var tableQuery = options.GenerateTableQuery<HintRequest, HintRequestAzure>();
            var hintRequests = await _azureManager.GetEntitiesAsync(tableQuery);
            return hintRequests.Select(t => t.FromAzureModel()).ToList();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //nothing to dispose
        }
        #endregion
    }
}
