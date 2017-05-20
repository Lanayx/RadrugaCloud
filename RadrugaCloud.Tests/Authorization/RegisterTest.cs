namespace RadrugaCloud.Tests.Authorization
{
    using System;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.Enums;
    using Core.Interfaces.Repositories;

    using Infrastructure.Repositories.Memory;

    using NUnit.Framework;

    using RadrugaCloud.Controllers.Api;
    using RadrugaCloud.Models.Api;

    using Services.AuthorizationServices;
    using Services.BL;
    using Services.DomainServices;

    /// <summary>
    /// Class RegisterTest
    /// </summary>
    [TestFixture]
    public sealed class RegisterTest : IDisposable
    {
        private IUserIdentityRepository _repository;

        private UserIdentityService _service;

        private UserService _userService;

        private IUserRepository _userRepository;

        private UserIdentityController _controller;

        private IRatingRepository _ratingRepository;

        private IMissionRepository _missionRepository;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [SetUp]
        public void Init()
        {
            _repository = new UserIdentityRepository();
            _missionRepository = new MissionRepository();
            _userRepository = new UserRepository();
            _ratingRepository = new RatingRepository();
            _service = new UserIdentityService(_repository);
            _userService = new UserService(_userRepository, _missionRepository, _ratingRepository, new AppCountersService(new AppCountersRepository()));
            _controller = new UserIdentityController(_userService, _service, new MailService(null, _repository), null);
        }

        /// <summary>
        /// Vk_register_with_warnings this instance.
        /// </summary>
        [Test]
        public async Task VkRegisterErrorIfNoToken()
        {
            // Arrange
            var vkModel = new RegisterVkModel
            {
                Id = 205387401,
                City = new VkReference { Id = 5331 },
                Country = new VkReference { Id = 9 },
                Counters = new Counters { Audios = 0, Followers = 215146, Friends = 127, Photos = 229, Videos = 4 },
                SexId = 2,
                NickName2 = "tomcruise",
                UniversityId = 0
            };

            // Act
            IdResult addResult = await _controller.RegisterVk(vkModel);

            // Assert
            Assert.IsTrue(addResult.Status == OperationResultStatus.Error);
        }


        [TearDown]
        public void Dispose()
        {
            _userRepository.Dispose();
            _missionRepository.Dispose();
            _controller.Dispose();
        }
    }
}
