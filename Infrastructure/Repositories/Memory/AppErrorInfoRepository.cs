namespace Infrastructure.Repositories.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Interfaces.Repositories;

    using Infrastructure.InfrastructureTools;

    /// <summary>
    ///     The appErrorInfo repository.
    /// </summary>
    public sealed class AppErrorInfoRepository : IAppErrorInfoRepository
    {
        /// <summary>
        ///     The all appErrorInfos.
        /// </summary>
        private readonly List<AppErrorInfo> _allAppErrorInfos = new List<AppErrorInfo>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="AppErrorInfoRepository" /> class.
        /// </summary>
        public AppErrorInfoRepository()
        {
            if (!_allAppErrorInfos.Any())
            {
                _allAppErrorInfos.Add(
                    new AppErrorInfo
                        {
                            Id = "Id_AppErrorInfo1",
                            UserId = "User1Id",
                            CurrentViewName = "Mission",
                            DeviceModel = "Passion",
                            DevicePlatform = "Android",
                            DeviceVersion = "4.3",
                            AppVersion = "1.1",
                            ErrorData = "Connection failed",
                            ErrorTime = DateTime.UtcNow
                        });
            }
        }

        /// <summary>
        ///     The add appErrorInfo.
        /// </summary>
        /// <param name="appErrorInfo">
        ///     The appErrorInfo.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<IdResult> AddAppErrorInfo(AppErrorInfo appErrorInfo)
        {
            appErrorInfo.Id = Guid.NewGuid().ToString();
            await Task.Factory.StartNew(() => _allAppErrorInfos.Add(appErrorInfo));
            return new IdResult(appErrorInfo.Id);
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            // nothing to dispose
        }

        /// <summary>
        ///     The get appErrorInfo.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task<AppErrorInfo> GetAppErrorInfo(string id)
        {
            return Task.Factory.StartNew(() => _allAppErrorInfos.Find(appErrorInfo => appErrorInfo.Id == id));
        }

        /// <summary>
        ///     The get appErrorInfos.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task<IEnumerable<AppErrorInfo>> GetAppErrorInfos(QueryOptions<AppErrorInfo> options)
        {
            if (options == null)
            {
                return Task.Factory.StartNew(() => _allAppErrorInfos.AsEnumerable());
            }
            return Task.Factory.StartNew(() => options.SimpleApply(_allAppErrorInfos.AsQueryable()).AsEnumerable());
        }
    }
}