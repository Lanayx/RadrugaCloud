namespace Services.DomainServices
{
    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Interfaces.Repositories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The AppErrorInfo service.
    /// </summary>
    public sealed class AppErrorInfoService
    {
        #region Fields

        /// <summary>
        /// The _AppErrorInfo repository.
        /// </summary>
        private readonly IAppErrorInfoRepository _appErrorInfoRepository;

        #endregion Fields

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppErrorInfoService"/> class.
        /// </summary>
        /// <param name="appErrorInfoRepository">
        /// The AppErrorInfo repository.
        /// </param>
        public AppErrorInfoService(IAppErrorInfoRepository appErrorInfoRepository)
        {
            _appErrorInfoRepository = appErrorInfoRepository;
        }

        #endregion Constructors and Destructors

        #region Public Methods and Operators

        /// <summary>
        /// Adds new AppErrorInfo.
        /// </summary>
        /// <param name="appErrorInfo">The AppErrorInfo.</param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        public Task<IdResult> AddNewAppErrorInfo(AppErrorInfo appErrorInfo)
        {
            return _appErrorInfoRepository.AddAppErrorInfo(appErrorInfo);
        }



        /// <summary>
        /// Gets the AppErrorInfo.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Task<AppErrorInfo> GetAppErrorInfo(string id)
        {
            return _appErrorInfoRepository.GetAppErrorInfo(id);
        }


        /// <summary>
        /// Gets the AppErrorInfos.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public Task<IEnumerable<AppErrorInfo>> GetAppErrorInfos(QueryOptions<AppErrorInfo> options = null)
        {
            return _appErrorInfoRepository.GetAppErrorInfos(options);
        }

    

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            _appErrorInfoRepository.Dispose();
        }

        #endregion Public Methods and Operators
    }
}