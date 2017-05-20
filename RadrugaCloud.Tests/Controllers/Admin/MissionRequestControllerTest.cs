namespace RadrugaCloud.Tests.Controllers.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;

    using Infrastructure.InfrastructureTools.Stub;
    using Infrastructure.Repositories.AzureTables;
    using Infrastructure.Repositories.Memory;

    using NUnit.Framework;

    using RadrugaCloud.Areas.Admin.Controllers;

    using Services.BL;
    using Services.DomainServices;

    using AppCountersRepository = Infrastructure.Repositories.Memory.AppCountersRepository;
    using CommonPlacesRepository = Infrastructure.Repositories.Memory.CommonPlacesRepository;
    using MissionRepository = Infrastructure.Repositories.Memory.MissionRepository;
    using MissionRequestRepository = Infrastructure.Repositories.Memory.MissionRequestRepository;
    using MissionSetRepository = Infrastructure.Repositories.Memory.MissionSetRepository;
    using UserRepository = Infrastructure.Repositories.Memory.UserRepository;
    using HintRequestRepository = Infrastructure.Repositories.Memory.HintRequestRepository;

    /// <summary>
    ///     The mission request controller test.
    /// </summary>
    [TestFixture]
    public sealed class MissionRequestControllerTest:IDisposable
    {
        /// <summary>
        ///     The init.
        /// </summary>
        [SetUp]
        public void Init()
        {
            _repository = new MissionRequestRepository();
            _missonRepository = new MissionRepository();
            _commonPlacesRepository = new CommonPlacesRepository();
            _missionSetRepository = new MissionSetRepository();
            _userRepository = new UserRepository();
            _ratingRepository = new RatingRepository();
            _ratingService = new RatingService(_userRepository, _ratingRepository, true);
            _notificationService = new NotificationService(new NotificationProvider());
            _missionService = new MissionService(_missonRepository, _userRepository, _missionSetRepository, new MissionRequestRepository(), new HintRequestRepository(),  _ratingService, _commonPlacesRepository);
            _service = new MissionRequestService(_repository, _missonRepository, _userRepository, _commonPlacesRepository, _ratingService, _notificationService,
                new AppCountersService(new AppCountersRepository()));
            _controller = new MissionRequestController(_service);
        }

        private MissionRequestController _controller;

        private CommonPlacesRepository _commonPlacesRepository;

        private MissionService _missionService;

        private IMissionRepository _missonRepository;

        private IMissionRequestRepository _repository;

        private RatingService _ratingService;
        

        private MissionRequestService _service;
        private UserRepository _userRepository;

        private MissionSetRepository _missionSetRepository;

        private RatingRepository _ratingRepository;
        private NotificationService _notificationService;

        /// <summary>
        ///     The approve mission request.
        /// </summary>
        [Test]
        public async Task ApproveMissionRequest()
        {
            // Действие
            var result = (ViewResult)(await _controller.Index(null));
            var requests = result.Model as IEnumerable<MissionRequest>;
            var missionRequest = requests.First();
      
            if (missionRequest == null)
            {
                return;
            }

            string id = missionRequest.Id;
            var addResult = await _controller.Approve(id,2);

            // Утверждение
            Assert.IsTrue(addResult is RedirectToRouteResult);
            var resultApproved = (await _controller.Details(id)) as ViewResult;
            var requestAfterApprove = resultApproved.Model as MissionRequest;
            Assert.NotNull(resultApproved);
            Assert.NotNull(requestAfterApprove);
            Assert.AreEqual(MissionRequestStatus.Approved, requestAfterApprove.Status);
        }

        [Test]
        public async Task DeclineMissionRequest()
        {
            // Действие

            var result = (ViewResult)(await _controller.Index(null));
            var requests = result.Model as IEnumerable<MissionRequest>;
            var missionRequest = requests.Last();

            if (missionRequest == null)
            {
                return;
            }

            string id = missionRequest.Id;
            var addResult = await _controller.Decline(id,"Test decline");

            // Утверждение
            Assert.IsTrue(addResult is RedirectToRouteResult);
            var resultApproved = (await _controller.Details(id)) as ViewResult;
            var requestAfterApprove = resultApproved.Model as MissionRequest;
            Assert.NotNull(resultApproved);
            Assert.NotNull(requestAfterApprove);
            Assert.AreEqual(MissionRequestStatus.Declined, requestAfterApprove.Status);
        }
        
        [Test]
        public async Task Get_Ratings_Check_Mission_Request_Declined()
        {
            var result = await _ratingService.GetRatings("User1Id", RatingType.Common);
            var userRecord = result.Ratings.First(res => res.UserId == "User1Id");
            var oldRatings = userRecord.Points;
            var oldPlace = userRecord.Place;

            var addResult = await _controller.Decline("Request1", "Test decline");
            
            var newResult = await _ratingService.GetRatings("User1Id", RatingType.Common);
            userRecord = newResult.Ratings.First(res => res.UserId == "User1Id");
            var newRatings = userRecord.Points;
            var newPlace = userRecord.Place;

            Assert.Less(newRatings, oldRatings);
            Assert.GreaterOrEqual(newPlace, oldPlace);

        }

        [Test]
        public async Task Get_Ratings_Check_Mission_Request_Approved()
        {
            var result = await _ratingService.GetRatings("User2Id", RatingType.Common);
            var userRecord = result.Ratings.First(res => res.UserId == "User2Id");
            var oldRatings = userRecord.Points;
            var oldPlace = userRecord.Place;

            var addResult = await _controller.Approve("Request2", 2);

            var newResult = await _ratingService.GetRatings("User2Id", RatingType.Common);
            userRecord = newResult.Ratings.First(res => res.UserId == "User2Id");
            var newRatings = userRecord.Points;
            var newPlace = userRecord.Place;

            Assert.Greater(newRatings, oldRatings);
            Assert.LessOrEqual(newPlace, oldPlace);

        }


        [TearDown]
        public void Dispose()
        {
            _commonPlacesRepository.Dispose();
            _missionSetRepository.Dispose();
            _userRepository.Dispose();
            _repository.Dispose();
            _missonRepository.Dispose();
            _missionService.Dispose();
            _service.Dispose();
            _controller.Dispose();
        }
    }
}