namespace Core.Interfaces.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Core.CommonModels.Results;
    using Core.DomainModels;

    public interface IAppCountersRepository:IDisposable
    {
        /// <summary>
        /// Gets the application counters.
        /// </summary>
        /// <returns></returns>
        Task<AppCounters> GetAppCounters();

        /// <summary>
        /// Updates the application counters.
        /// </summary>
        /// <param name="appCounters">The application counters.</param>
        /// <returns></returns>
        Task<OperationResult> UpsertAppCounters(AppCounters appCounters);

    }
}
