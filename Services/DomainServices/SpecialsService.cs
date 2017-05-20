namespace Services.DomainServices
{
    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Interfaces.Repositories;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    /// <summary>
    /// The SpecialsService service.
    /// </summary>
    public sealed class SpecialsService
    {
        #region Fields

        /// <summary>
        /// The _AppErrorInfo repository.
        /// </summary>
        private readonly IUserDataRepository _userDataRepository;

        #endregion Fields

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialsService"/> class.
        /// </summary>
        /// <param name="userDataRepository">The user data repository.</param>
        public SpecialsService(IUserDataRepository userDataRepository)
        {
            _userDataRepository = userDataRepository;
        }

        #endregion Constructors and Destructors

        #region Public Methods and Operators


        /// <summary>
        /// Posts the message to developers.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public Task<OperationResult> PostMessageToDevelopers(string userId, string message)
        {
            return _userDataRepository.PostMessageToDevelopers(userId, message);
        }


        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            _userDataRepository.Dispose();
        }

        #endregion Public Methods and Operators

        public Task<string> GetRatePageUrl(string userId, string platform)
        {
            Trace.TraceInformation($"User {userId} started rating app on {platform}");
            string appIdIos = "1111998952",
                    appIdAndroid = "com.radruga.app",
                    appIdWindows = "50f06be2-f715-4e0e-9aae-7af45c9c7f02";

            string url;
            switch (platform)
            {
                case "Android":
                    url = "market://details?id=" + appIdAndroid;
                    break;
                case "iOS":
                    url = "itms-apps://itunes.apple.com/app/id" + appIdIos;
                    break;
                default:
                    url = "ms-windows-store:reviewapp?appid=" + appIdWindows;
                    break;
            }
            return Task.FromResult(url);
        }
    }
}