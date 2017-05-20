namespace Services.DomainServices
{
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Interfaces.Repositories;

    /// <summary>
    ///     The AppCounters service.
    /// </summary>
    public sealed class AppCountersService
    {
        /// <summary>
        ///     The _AppCounters repository.
        /// </summary>
        private readonly IAppCountersRepository _appCountersRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AppCountersService" /> class.
        /// </summary>
        /// <param name="appCountersRepository">
        ///     The AppCounters repository.
        /// </param>
        public AppCountersService(IAppCountersRepository appCountersRepository)
        {
            _appCountersRepository = appCountersRepository;
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            _appCountersRepository.Dispose();
        }

        /// <summary>
        ///     Gets the AppCounters.
        /// </summary>
        /// <returns></returns>
        public Task<AppCounters> GetAppCounters()
        {
            return _appCountersRepository.GetAppCounters();
        }

        /// <summary>
        ///     Users submited a kind action
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult> KindActionSubmited()
        {
            //TODO concurrency management (with Etag in repository)
            var counters = await _appCountersRepository.GetAppCounters();
            var updatingCounter = new AppCounters { KindActionsSubmited = (counters.KindActionsSubmited ?? 0) + 1 };
            return await _appCountersRepository.UpsertAppCounters(updatingCounter);
        }

        /// <summary>
        ///     Users passed test
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult> QuestionsAnswered()
        {
            //TODO concurrency management (with Etag in repository)
            var counters = await _appCountersRepository.GetAppCounters();
            var updatingCounter = new AppCounters { TestPassed = (counters.TestPassed ?? 0) + 1 };
            return await _appCountersRepository.UpsertAppCounters(updatingCounter);
        }

        /// <summary>
        ///     User passed last mission.
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult> UserHasFinished()
        {
            //TODO concurrency management (with Etag in repository)
            var counters = await _appCountersRepository.GetAppCounters();
            var updatingCounter = new AppCounters { FinishedUsers = (counters.FinishedUsers ?? 0) + 1 };
            return await _appCountersRepository.UpsertAppCounters(updatingCounter);
        }

        /// <summary>
        ///     User passed first mission.
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult> UserPassedFirstMission()
        {
            //TODO concurrency management (with Etag in repository)
            var counters = await _appCountersRepository.GetAppCounters();
            var updatingCounter = new AppCounters { OneMissionPassedUsers = (counters.OneMissionPassedUsers ?? 0) + 1 };
            return await _appCountersRepository.UpsertAppCounters(updatingCounter);
        }

        /// <summary>
        ///     Users the registered.
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult> UserRegistered()
        {
            //TODO concurrency management (with Etag in repository)
            var counters = await _appCountersRepository.GetAppCounters();
            var updatingCounter = new AppCounters { RegisteredUsers = (counters.RegisteredUsers ?? 0) + 1 };
            return await _appCountersRepository.UpsertAppCounters(updatingCounter);
        }

        /// <summary>
        ///     Users made a vk repost.
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult> VkRepost()
        {
            //TODO concurrency management (with Etag in repository)
            var counters = await _appCountersRepository.GetAppCounters();
            var updatingCounter = new AppCounters { VkReposts = (counters.VkReposts ?? 0) + 1 };
            return await _appCountersRepository.UpsertAppCounters(updatingCounter);
        }
    }
}