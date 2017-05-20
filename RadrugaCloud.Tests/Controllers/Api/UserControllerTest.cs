namespace RadrugaCloud.Tests.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;

    using Infrastructure.InfrastructureTools.Memory;
    using Infrastructure.Repositories.Memory;

    using NUnit.Framework;
    using RadrugaCloud.Controllers.Api;

    using Services.AuthorizationServices;
    using Services.BL;
    using Services.DomainServices;

    using MissionRepository = Infrastructure.Repositories.Memory.MissionRepository;
    using UserIdentityRepository = Infrastructure.Repositories.Memory.UserIdentityRepository;
    using UserRepository = Infrastructure.Repositories.Memory.UserRepository;

    /// <summary>
    ///     The user controller test.
    /// </summary>
    [TestFixture]
    public sealed class UserControllerTest:IDisposable
    {
        /// <summary>
        ///     The _controller.
        /// </summary>
        private UserController _controller;

        /// <summary>
        ///     The _repository.
        /// </summary>
        private IUserRepository _repository;

        /// <summary>
        ///     The _service.
        /// </summary>
        private UserService _service;
        private MissionRepository _missionRepository;
        private ImageService _imageService;
        private RatingService _ratingService;

        private UserIdentityRepository _userIdentityRepository;

        private UserIdentityService _userIdentityService;

        private AppCountersRepository _appCountersRepository;

        private AppCountersService _appCountersService;

        private RatingRepository _ratingRepository;

        /// <summary>
        ///     The init.
        /// </summary>
        [SetUp]
        public void Init()
        {
            _repository = new UserRepository();
            _userIdentityRepository = new UserIdentityRepository();
            _missionRepository = new MissionRepository();
            _ratingRepository = new RatingRepository();
            var imageProvider = new ImageProvider();
            _imageService = new ImageService(imageProvider, _repository);
            _ratingService = new RatingService(_repository, _ratingRepository, true);
            _service = new UserService(_repository, _missionRepository, _ratingRepository, _appCountersService);
            _userIdentityService = new UserIdentityService(_userIdentityRepository);
            
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Sid, "User1Id") }));
            _controller = new UserController(_service, _imageService, _ratingService, _userIdentityService) { User = principal };
        }

        /// <summary>
        ///  Create user.
        /// </summary>
        [Test]
        public async Task Create()
        {
            var controller = _controller;
            //change default settings
            var userId = Guid.NewGuid().ToString();
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Sid, userId) }));
            _controller = new UserController(_service, _imageService, _ratingService, _userIdentityService) { User = principal };


            // Действие
            var user = new User
            {
                AvatarUrl = "http://cs540104.vk.me/c540102/v540102420/e88c/P64liS_pPNk.jpg",
                DateOfBirth = new DateTime(1990, 1, 1),
                NickName = "Updated",
                HomeCoordinate = new GeoCoordinate(53.9, 27.56667, 199),
                KindScale = 87,
                Points = 120,
                EnablePushNotifications = true,
                Level = 7,
                Sex = Sex.Male,
                PersonQualitiesWithScores =
                new List<PersonQualityIdWithScore>
                                            {
                                                new PersonQualityIdWithScore{PersonQualityId = Guid.NewGuid().ToString(),Score = 2}
                                            }
            };

            OperationResult resultsNew = await _controller.Post(user);

            // Утверждение
            Assert.AreEqual(OperationResultStatus.Success, resultsNew.Status);


            //restore default settings
            _controller = controller;

        }

        /// <summary>
        ///   Create failed.
        /// </summary>
        [Test]
        public async Task CreateExisting()
        {
            // Действие
            var user = new User
            {
                AvatarUrl = "http://cs540104.vk.me/c540102/v540102420/e88c/P64liS_pPNk.jpg",
                DateOfBirth = new DateTime(1990, 1, 1),
                NickName = "Updated",
                HomeCoordinate = new GeoCoordinate(53.9, 27.56667, 199),
                KindScale = 87,
                Points = 120,
                EnablePushNotifications = true,
                Level = 7,
                Sex = Sex.Male,
                PersonQualitiesWithScores =
                new List<PersonQualityIdWithScore>
                                            {
                                                new PersonQualityIdWithScore{PersonQualityId = Guid.NewGuid().ToString(),Score = 2}
                                            }
            };

            IdResult resultsNew = await _controller.Post(user);

            // Утверждение
            Assert.NotNull(resultsNew);
            Assert.AreEqual(resultsNew.Status, OperationResultStatus.Warning);
        }

        /// <summary>
        ///     The get current user.
        /// </summary>
        [Test]
        public async Task GetCurrentUser()
        {
            // Действие
            User user = await _controller.Get();
            Assert.NotNull(user);
            Assert.AreEqual(user.Id, "User1Id");
        }

        [Test]
        public async Task Update_Nickname()
        {
            User user = await _controller.Get();
            var oldAvatarUrl = user.AvatarUrl;

            var result = await _controller.ChangeNickName("SomeNewNickName");
            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            var updatedUser = await _controller.Get();

            Assert.AreEqual("SomeNewNickName", updatedUser.NickName);
            Assert.AreEqual(oldAvatarUrl, updatedUser.AvatarUrl);

        }
        [Test]
        public async Task Update_Nickname_TooLong()
        {
            var result = await _controller.ChangeNickName("SomeNewNickName of a veeeery long hyper name!!!!!!!!!!");
            Assert.AreEqual(result.Status, OperationResultStatus.Error);
        }

        [Test]
        public async Task Get_Ratings_Check_Nickname_Change()
        {
            var result = await _controller.GetRatings(RatingType.Common);

            Assert.AreNotEqual(result.Ratings[0].NickName, "Test");
            await _controller.ChangeNickName("Test");
            var newResult = await _controller.GetRatings(RatingType.Common);

            Assert.AreEqual(newResult.Ratings[0].NickName, "Test");
        }


        [Test]
        public async Task Get_Ratings_Check_Avatar_Change()
        {
            var result = await _controller.GetRatings(RatingType.Common);

            Assert.AreNotEqual(result.Ratings[0].AvatarUrl, "NewAvatarUrl");
            await _service.ChangeUserAvatar("User1Id", "NewAvatarUrl");
            var newResult = await _controller.GetRatings(RatingType.Common);

            Assert.AreEqual(newResult.Ratings[0].AvatarUrl, "NewAvatarUrl");
        }

        [TearDown]
        public void Dispose()
        {
            _missionRepository.Dispose();
            _appCountersRepository.Dispose();
            _repository.Dispose();
            _service.Dispose();
            _controller.Dispose();
        }
    }
}