namespace RadrugaCloud.Tests.Controllers.Missions
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;

    using Infrastructure.InfrastructureTools.Memory;
    using Infrastructure.InfrastructureTools.Stub;
    using Infrastructure.Repositories.AzureTables;
    using Infrastructure.Repositories.Memory;

    using NUnit.Framework;

    using RadrugaCloud.Controllers.Api;

    using Services.BL;
    using Services.DomainServices;

    using AppCountersRepository = Infrastructure.Repositories.AzureTables.AppCountersRepository;
    using MissionRepository = Infrastructure.Repositories.Memory.MissionRepository;
    using MissionRequestRepository = Infrastructure.Repositories.Memory.MissionRequestRepository;
    using MissionSetRepository = Infrastructure.Repositories.Memory.MissionSetRepository;
    using UserRepository = Infrastructure.Repositories.Memory.UserRepository;
    using CommonPlacesRepository = Infrastructure.Repositories.Memory.CommonPlacesRepository;
    using HintRequestRepository = Infrastructure.Repositories.Memory.HintRequestRepository;

    /// <summary>
    ///     The user controller test.
    /// </summary>
    [TestFixture]
    public sealed class AutoApprovedMissionsTest : IDisposable
    {
        private MissionController _controller;

        private IUserRepository _userRepository;

        private UserService _service;
        private MissionRepository _missionRepository;
        private ImageService _imageService;
        private RatingService _ratingService;

        private MissionRequestService _missionRequestService;

        private MissionRequestRepository _missionRequestRepository;

        private MissionSetRepository _missionSetRepository;

        private MissionService _missionService;

        private AppCountersRepository _appCountersRepository;

        private AppCountersService _appCountersService;

        private CommonPlacesRepository _commonPlacesRepository;

        private RatingRepository _ratingRepository;

        private HintRequestRepository _hintRequestRepository;

        /// <summary>
        ///     The init.
        /// </summary>
        [SetUp]
        public void Init()
        {
            _userRepository = new UserRepository();
            _commonPlacesRepository = new CommonPlacesRepository();
            _missionRepository = new MissionRepository();
            _missionSetRepository = new MissionSetRepository();
            _missionRequestRepository = new MissionRequestRepository();
            _appCountersRepository = new AppCountersRepository();
            _hintRequestRepository = new HintRequestRepository();
            _ratingRepository = new RatingRepository();
            var imageProvider = new ImageProvider();
            _imageService = new ImageService(imageProvider, _userRepository);
            _ratingService = new RatingService(_userRepository, _ratingRepository, true);
            _appCountersService = new AppCountersService(_appCountersRepository);
            _service = new UserService(_userRepository, _missionRepository, _ratingRepository, _appCountersService);
            _missionService = new MissionService(_missionRepository, _userRepository, _missionSetRepository, _missionRequestRepository, _hintRequestRepository, _ratingService, _commonPlacesRepository);
            _missionRequestService = new MissionRequestService(_missionRequestRepository, _missionRepository, _userRepository, _commonPlacesRepository,
                _ratingService, new NotificationService(new NotificationProvider()), _appCountersService);

            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Sid, "User1Id") }));
            _controller = new MissionController(_missionService, _missionRequestService, _imageService) { User = principal };
        }

        /// <summary>
        /// Called when [success_ three_stars].
        /// </summary>
        [Test]
        public async Task OneAnswer_Success_ThreeStars()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("2ddf9168-b030-4b6c-a038-72593e7a75f2", 
                new MissionProof { CreatedText = "Евгений Онегин" });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual(MissionCompletionStatus.Success, result.MissionCompletionStatus);
            Assert.AreEqual(3, result.StarsCount);
            Assert.Greater(user.Points, oldPoints);

        }

        [Test]
        public async Task One_answer_Success_One_star()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPoints = user.Points;

            await _controller.CompleteMission("2ddf9168-b030-4b6c-a038-72593e7a75f1",
                new MissionProof { CreatedText = "Евгений Онегин Александр Пушкин" });

            user = await _userRepository.GetUser("User1Id");
            Assert.AreEqual(user.Points, oldPoints);

            await _controller.CompleteMission("2ddf9168-b030-4b6c-a038-72593e7a75f1",
                new MissionProof { CreatedText = "Евгений Онегин Александр Пушкин" });
            var result = await _controller.CompleteMission("2ddf9168-b030-4b6c-a038-72593e7a75f1",
                new MissionProof { CreatedText = "Евгений Онегин" });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Success);
            Assert.AreEqual(result.StarsCount, 1);

            Assert.Greater(user.Points, oldPoints);
        }

        [Test]
        public async Task OneAnswer_FromTwo_Success_ThreeStars()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("3c27b903-cf03-447e-9682-f756e70ca908",
                new MissionProof { CreatedText = "Дарт Вейдер" });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Success);
            Assert.AreEqual(result.StarsCount, 3);
            Assert.Greater(user.Points, oldPoints);
        }

        [Test]
        public async Task TwoAnswers_FromTwo_Success_ThreeStars()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("33f075f4-4e8c-47bc-b0d7-9661f4ecdd6d",
                new MissionProof { CreatedText = "Сила тяжести;Центробежная сила" });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Success);
            Assert.AreEqual(result.StarsCount, 3);
            Assert.Greater(user.Points, oldPoints);
        }

        [Test]
        public async Task TwoAnswers_FromTwo_PartialSuccess()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("33f075f4-4e8c-47bc-b0d7-9661f4ecdd7d",
                new MissionProof { CreatedText = "Сила притяжения;Центробежная сила" });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.IntermediateFail);
            Assert.AreEqual(user.Points, oldPoints);
        }

        [Test]
        public async Task TwoAnswers_FromTwo_Fail()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("33f075f4-4e8c-47bc-b0d7-9661f4ecdd8d",
                new MissionProof { CreatedText = "Сила притяжения;Сила отталкивания" });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.IntermediateFail);
            Assert.IsTrue(String.IsNullOrEmpty(result.Description));
            Assert.AreEqual(user.Points, oldPoints);
        }

        [Test]
        public async Task TwoAnswers_FromTwo_Fail_And_TotalFail()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("33f075f4-4e8c-47bc-b0d7-9661f4ecdd5d",
                new MissionProof { CreatedText = "Сила притяжения;Центробежная сила" });
            Assert.AreEqual(MissionCompletionStatus.IntermediateFail,result.MissionCompletionStatus);
            Assert.AreEqual(1, result.TryCount);
            await _controller.CompleteMission("33f075f4-4e8c-47bc-b0d7-9661f4ecdd5d",
                new MissionProof { CreatedText = "Сила притяжения;Центробежная сила" });
            result = await _controller.CompleteMission("33f075f4-4e8c-47bc-b0d7-9661f4ecdd5d",
                new MissionProof { CreatedText = "Сила притяжения;Центробежная сила" });
            Assert.AreEqual(MissionCompletionStatus.IntermediateFail, result.MissionCompletionStatus);
            Assert.AreEqual(3, result.TryCount);
            await _controller.CompleteMission("33f075f4-4e8c-47bc-b0d7-9661f4ecdd5d",
                new MissionProof { CreatedText = "Сила притяжения;Центробежная сила" });
            await _controller.CompleteMission("33f075f4-4e8c-47bc-b0d7-9661f4ecdd5d",
                new MissionProof { CreatedText = "Сила притяжения;Центробежная сила" });

            user = await _userRepository.GetUser("User1Id");
            Assert.Less(user.Points, oldPoints);
        }

        [Test]
        public async Task All_Words_Included_Success()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("fe9b1f3a-059b-4b4e-8f08-0a22a21a1ed0",
                new MissionProof { CreatedText = "того, кто изучает один удар десять тысяч раз" });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Success);
            Assert.Greater(user.Points, oldPoints);
        }

        [Test]
        public async Task All_Words_Included_Fail()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("fe9b1f3a-059b-4b4e-8f08-0a22a21a1ed0",
                new MissionProof { CreatedText = "того, кто изучает один удар столько раз" });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.IntermediateFail);
            Assert.AreEqual(user.Points, oldPoints);
        }

        [Test]
        public async Task Common_Place_Success()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("68d75498-e44f-4bf8-a2ee-13b4998c1725",
                new MissionProof { Coordinates = new List<GeoCoordinate>{new GeoCoordinate(0,0)}});

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual(MissionCompletionStatus.Success, result.MissionCompletionStatus);
            Assert.Greater(user.Points, oldPoints);
        }

        [Test]
        public async Task Common_Place_Success_2_Stars()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPoints = user.Points;

            await _controller.CompleteMission("68d75498-e44f-4bf8-a2ee-13b4998c17d3",
                new MissionProof { Coordinates = new List<GeoCoordinate> { new GeoCoordinate(50, 50) } });
            await _controller.CompleteMission("68d75498-e44f-4bf8-a2ee-13b4998c17d3",
                new MissionProof { Coordinates = new List<GeoCoordinate> { new GeoCoordinate(50, 50) } });
            var result = await _controller.CompleteMission("68d75498-e44f-4bf8-a2ee-13b4998c17d3",
                new MissionProof { Coordinates = new List<GeoCoordinate> { new GeoCoordinate(0, 0) } });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual(MissionCompletionStatus.Success, result.MissionCompletionStatus);
            Assert.Greater(user.Points, oldPoints);
            Assert.AreEqual(2, result.StarsCount);
        }

        [Test]
        public async Task Common_Place_Fail()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("68d75498-e44f-4bf8-a2ee-13b4998c17d3",
                new MissionProof { Coordinates = new List<GeoCoordinate> { new GeoCoordinate(80, 80) } });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual(MissionCompletionStatus.IntermediateFail, result.MissionCompletionStatus);
            Assert.AreEqual(user.Points, oldPoints);
        }

        [Test]
        public async Task Common_Place_Temporary_Success()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldCommonPlaces = (await _commonPlacesRepository.GetCommonPlacesByAlias("User1Id", "CM 3")).Count();
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("68d75498-e44f-4bf8-a2ee-13b4998c17d4",
                new MissionProof { Coordinates = new List<GeoCoordinate> { new GeoCoordinate(0, 0) } });

            user = await _userRepository.GetUser("User1Id");
            var refreshedCommonPlaces = (await _commonPlacesRepository.GetCommonPlacesByAlias("User1Id", "CM 3")).Count();

            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual(MissionCompletionStatus.Success, result.MissionCompletionStatus);
            Assert.Greater(user.Points, oldPoints);
            Assert.Greater(refreshedCommonPlaces, oldCommonPlaces);
        }

        [Test]
        public async Task Common_Place_Temporary_Success_CreatedApprovedCoordinate()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldCommonPlaces = (await _commonPlacesRepository.GetCommonPlacesByAlias("User1Id", "CM 4")).Count();
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("68d75498-e44f-4bf8-a2ee-13b4998c17d5",
                new MissionProof { Coordinates = new List<GeoCoordinate> { new GeoCoordinate(0, 0) } });

            user = await _userRepository.GetUser("User1Id");
            var refreshedCommonPlaces = (await _commonPlacesRepository.GetCommonPlacesByAlias("User1Id", "CM 4")).Count();
            var approvedCommonPlace = await _commonPlacesRepository.GetCommonPlaceByAlias("User1Id", "CM 4");

            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual(MissionCompletionStatus.Success, result.MissionCompletionStatus);
            Assert.IsNotNull(approvedCommonPlace);
            Assert.Greater(user.Points, oldPoints);
            Assert.Greater(refreshedCommonPlaces, oldCommonPlaces);
        }


        [TearDown]
        public void Dispose()
        {
            _appCountersRepository.Dispose();
            _missionRepository.Dispose();
            _missionSetRepository.Dispose();
            _missionRequestRepository.Dispose();
            _commonPlacesRepository.Dispose();
            _userRepository.Dispose();
            _service.Dispose();
            _controller.Dispose();
        }
    }
}