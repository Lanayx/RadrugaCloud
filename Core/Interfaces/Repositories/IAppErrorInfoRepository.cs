namespace Core.Interfaces.Repositories
{
    using System;
    using System.Collections.Generic;
    using Core.CommonModels.Query;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.DomainModels;

    /// <summary>
    /// The AppErrorInfoRepository interface.
    /// </summary>
    public interface IAppErrorInfoRepository : IDisposable
    {
        #region Public Methods and Operators


        /// <summary>
        /// Adds the appErrorInfo.
        /// </summary>
        /// <param name="appErrorInfo">The appErrorInfo.</param>
        /// <returns></returns>
        Task<IdResult> AddAppErrorInfo(AppErrorInfo appErrorInfo);

      

        /// <summary>
        /// Gets the appErrorInfo.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<AppErrorInfo> GetAppErrorInfo(string id);


        /// <summary>
        /// Gets the appErrorInfos.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<AppErrorInfo>> GetAppErrorInfos(QueryOptions<AppErrorInfo> options);
     

        #endregion
    }
}