namespace Infrastructure.Repositories.Memory
{

    using System.Threading.Tasks;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;

    public sealed class AppCountersRepository: IAppCountersRepository
    {
        #region Fields

        private AppCounters AppCounters;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppCountersRepository"/> class.
        /// </summary>
        public AppCountersRepository()
        {
            AppCounters = new AppCounters
                              {
                                  FinishedUsers = 10,
                                  KindActionsSubmited = 200,
                                  OneMissionPassedUsers = 100,
                                  RegisteredUsers = 150,
                                  TestPassed = 80,
                                  VkReposts = 30
                              };
        }

        #endregion

        #region Public Methods and Operators

        public Task<AppCounters> GetAppCounters()
        {
            return Task.Factory.StartNew(() => AppCounters);
        }

        public async Task<OperationResult> UpsertAppCounters(AppCounters appCounters)
        {
            await Task.Factory.StartNew(() => { AppCounters = appCounters; });
            return new OperationResult(OperationResultStatus.Success);
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
