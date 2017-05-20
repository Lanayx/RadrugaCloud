namespace RadrugaCloud.Tests.Controllers.Api
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.Tools.CopyHelper;

    using Infrastructure.InfrastructureTools.Memory;
    using Infrastructure.InfrastructureTools.Stub;
    using Infrastructure.Repositories.Memory;

    using NUnit.Framework;
    using RadrugaCloud.Controllers.Api;

    using Services.BL;
    using Services.DomainServices;

    using AppCountersRepository = Infrastructure.Repositories.Memory.AppCountersRepository;
    using MissionRepository = Infrastructure.Repositories.Memory.MissionRepository;
    using UserRepository = Infrastructure.Repositories.Memory.UserRepository;
    using CommonPlacesRepository = Infrastructure.Repositories.Memory.CommonPlacesRepository;
    using MissionRequestRepository = Infrastructure.Repositories.Memory.MissionRequestRepository;
    using MissionSetRepository = Infrastructure.Repositories.Memory.MissionSetRepository;
    using HintRequestRepository = Infrastructure.Repositories.Memory.HintRequestRepository;

    /// <summary>
    ///     The mission controller test.
    /// </summary>
    [TestFixture]
    public sealed class MissionControllerTest : IDisposable
    {
        /// <summary>
        ///     The init.
        /// </summary>
        [SetUp]
        public void Init()
        {
            _repository = new MissionRepository();
            _userRepository = new UserRepository();
            _ratingRepository = new RatingRepository();
            _commonPlacesRepository = new CommonPlacesRepository();
            _missionRequestRepository = new MissionRequestRepository();
            _appCountersRepository = new AppCountersRepository();
            _appCountersService = new AppCountersService(_appCountersRepository);
            _ratingService = new RatingService(_userRepository, _ratingRepository, true);
            _hintRequestRepository = new HintRequestRepository();
            _missionRequestService = new MissionRequestService(_missionRequestRepository, _repository, _userRepository, _commonPlacesRepository, _ratingService,
                new NotificationService(new NotificationProvider()), _appCountersService);
            _service = new MissionService(_repository, _userRepository,new MissionSetRepository(), new MissionRequestRepository(), _hintRequestRepository, _ratingService, _commonPlacesRepository);
            _userService = new UserService(_userRepository, _repository, _ratingRepository, _appCountersService);
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Sid, "User1Id") }));
            _controller = new MissionController(_service,_missionRequestService,new ImageService(new ImageProvider(),_userRepository )) { User = principal };
        }

        private MissionController _controller;

        private IMissionRepository _repository;

        private MissionService _service;
        
        private UserRepository _userRepository;

        private UserService _userService;

        private RatingService _ratingService;
        private MissionRequestRepository _missionRequestRepository;
        private MissionRequestService _missionRequestService;

        private AppCountersRepository _appCountersRepository;

        private AppCountersService _appCountersService;

        private RatingRepository _ratingRepository;

        private CommonPlacesRepository _commonPlacesRepository;

        private HintRequestRepository _hintRequestRepository;

        /// <summary>
        /// Tests the decline mission.
        /// </summary>
        [Test]
        public async Task Test_Decline_Mission()
        {
            var oldUser = (await _userService.GetUser("User1Id")).Copy();
            var firstMissionId = oldUser.ActiveMissionIds.First().MissionId;
            var declineResult = await _controller.DeclineMission(firstMissionId);
            var user = await _userService.GetUser("User1Id");

            Assert.AreEqual(OperationResultStatus.Success,declineResult.Status);
            Assert.Less(user.Points, oldUser.Points);
            Assert.False(user.ActiveMissionIds.Select(m => m.MissionId).Contains(firstMissionId));
            Assert.Contains(firstMissionId, user.FailedMissionIds);
            if (user.Level != 0 || user.Points != 0)
            {
                Assert.Less(user.Points, oldUser.Points);
            }
        }


        [Test]
        public async Task Get_Ratings_Check_Mission_Declined()
        {
            var result = await _ratingService.GetRatings("User1Id", RatingType.Common);
            var oldRatings = result.Ratings[0].Points;
            var oldPlace = result.Ratings[0].Place;

            var oldUser = (await _userService.GetUser("User1Id")).Copy();
            var lastMissionId = oldUser.ActiveMissionIds.First().MissionId;
            await _controller.DeclineMission(lastMissionId);

            var newResult = await _ratingService.GetRatings("User1Id", RatingType.Common);
            var newRatings = newResult.Ratings[0].Points;
            var newPlace = newResult.Ratings[0].Place;

            Assert.Less(newRatings, oldRatings);
            Assert.GreaterOrEqual(newPlace, oldPlace);

        }

        [Test]
        public async Task Get_All_MissionSets()
        {
            var result = await _service.GetActiveMissionSets("User1Id");

            Assert.AreEqual(result.Count(), 2);
        }

        [Test]
        public async Task Get_Text_Hint()
        {
            var result = await _controller.GetHint("Mission1", "TextHint");
            var selectOptions = new QueryOptions<HintRequest>
            {
                Filter = h => (h.HintId == "TextHint") &&
                                (h.MissionId == "Mission1") &&
                                (h.UserId == "User1Id")
            };

            var hintRequests = await _hintRequestRepository.GetHintRequests(selectOptions);
            var hintRequest = hintRequests.First();

            Assert.AreEqual(result.Status, OperationResultStatus.Success);            
            Assert.AreEqual(hintRequest.Status, HintRequestStatus.Success);
            Assert.AreEqual(result.Hint, "The text hint for first mission");
        }        

        [Test]
        public async Task Get_Common_Place_Hint()
        {
            var result = await _controller.GetHint("68d75498-e44f-4bf8-a2ee-13b4998c17d6", "CommonPlaceHint");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.Hint, "24, 77");
        }

        [Test]
        public async Task Get_Direction_Hint()
        {
            var result = await _controller.GetHint("Mission1", "DirectionHint");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.Hint, string.Empty);
        }

        [Test]
        public async Task Get_Hint_From_Nonexistent_Mission()
        {
            var result = await _controller.GetHint("NonexistentMission", "TextHint");

            var selectOptions = new QueryOptions<HintRequest>
            {
                Filter = h => (h.HintId == "TextHint") &&
                                (h.MissionId == "NonexistentMission") &&
                                (h.UserId == "User1Id")
            };
            var hintRequests = await _hintRequestRepository.GetHintRequests(selectOptions);
            var hintRequest = hintRequests.First();
            
            Assert.AreEqual(hintRequest.Status, HintRequestStatus.UserDontHaveThatMissionInActiveStatus);
            Assert.AreEqual(result.Status, OperationResultStatus.Error);
            Assert.AreEqual(result.Hint, string.Empty);
        }

        [Test]
        public async Task Get_Nonexistent_Hint()
        {
            var result = await _controller.GetHint("Mission1", "NonexistentHint");

            var selectOptions = new QueryOptions<HintRequest>
            {
                Filter = h => (h.HintId == "NonexistentHint") &&
                                (h.MissionId == "Mission1") &&
                                (h.UserId == "User1Id")
            };
            var hintRequests = await _hintRequestRepository.GetHintRequests(selectOptions);
            var hintRequest = hintRequests.First();

            Assert.AreEqual(hintRequest.Status, HintRequestStatus.HintNotFound);
            Assert.AreEqual(result.Status, OperationResultStatus.Error);
            Assert.AreEqual(result.Hint, string.Empty);
        }

        [Test]
        public void Get_Hint_With_Nonexistent_Common_Place()
        {
            var aggregateException = Assert.Throws<AggregateException>(() =>
                    {
                       _controller.GetHint("68d75498-e44f-4bf8-a2ee-13b4998c17d7", "NonexistentCommonPlaceHint").Wait();
                    });
            var argumentException = aggregateException.InnerExceptions
                .First(x => x.GetType() == typeof(ArgumentException)) as ArgumentException;
            Assert.That(argumentException, Is.Not.Null);
            Assert.IsTrue(argumentException?.Message.StartsWith("Alias invalid"));

        }

        [Test]
        public async Task Get_Hint_When_User_Do_Not_Have_Payment()
        {
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Sid, "User2Id") }));
            _controller.User = principal;
            var result = await _controller.GetHint("Mission1", "TextHint");
            var selectOptions = new QueryOptions<HintRequest>
            {
                Filter = h => (h.HintId == "TextHint") &&
                                (h.MissionId == "Mission1") &&
                                (h.UserId == "User2Id")
            };
            
            var hintRequests = await _hintRequestRepository.GetHintRequests(selectOptions);
            var hintRequest = hintRequests.First();

            Assert.AreEqual(hintRequest.Status, HintRequestStatus.UserDontHaveCoins);
            Assert.AreEqual(result.Status, OperationResultStatus.Error);
            Assert.AreEqual(result.Hint, string.Empty);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [TearDown]
        public void Dispose()
        {
            _userRepository.Dispose();
            _appCountersRepository.Dispose();
            _commonPlacesRepository.Dispose();
            _missionRequestRepository.Dispose();
            _repository.Dispose();
            _service.Dispose();
            _controller.Dispose();
        }
        
    }
}