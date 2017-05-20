namespace RadrugaCloud.Tests.Controllers.Missions
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Core.Constants;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;

    using Infrastructure.InfrastructureTools.Memory;
    using Infrastructure.InfrastructureTools.Stub;
    using Infrastructure.Repositories.AzureTables;
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
    ///     The user controller test.
    /// </summary>
    [TestFixture]
    public sealed class UniqueMisisonsTest : IDisposable
    {
        /// <summary>
        ///     The _controller.
        /// </summary>
        private MissionController _controller;

        /// <summary>
        ///     The _repository.
        /// </summary>
        private IUserRepository _userRepository;

        /// <summary>
        ///     The _service.
        /// </summary>
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

        private RatingRepository _ratingRepository;

        private CommonPlacesRepository _commonPlacesRepository;

        /// <summary>
        ///     The init.
        /// </summary>
        [SetUp]
        public void Init()
        {
            _userRepository = new UserRepository();
            _missionRepository = new MissionRepository();
            _commonPlacesRepository = new CommonPlacesRepository();
            _missionSetRepository = new MissionSetRepository();
            _missionRequestRepository = new MissionRequestRepository();
            _ratingRepository = new RatingRepository();
            var imageProvider = new ImageProvider();
            _imageService = new ImageService(imageProvider, _userRepository);
            _ratingService = new RatingService(_userRepository, _ratingRepository, true);
            _appCountersRepository = new AppCountersRepository();
            _appCountersService = new AppCountersService(_appCountersRepository);
            _service = new UserService(_userRepository, _missionRepository, _ratingRepository, _appCountersService);
            _missionService = new MissionService(_missionRepository, _userRepository, _missionSetRepository, new MissionRequestRepository(), new HintRequestRepository(), _ratingService, _commonPlacesRepository);
            _missionRequestService = new MissionRequestService(_missionRequestRepository, _missionRepository, _userRepository, _commonPlacesRepository, _ratingService,
                new NotificationService(new NotificationProvider()), _appCountersService);

            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Sid, "User1Id") }));
            _controller = new MissionController(_missionService, _missionRequestService, _imageService) { User = principal };
        }



        [Test]
        public async Task Show_Yourself_Success()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldAvatarUrl = user.AvatarUrl;
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("d061155a-9504-498d-a6e7-bcc20c295cde", new MissionProof { ImageUrls = new List<string> { "imageurl" } });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Success);
            Assert.AreEqual(oldAvatarUrl, user.AvatarUrl);// avatar is updated separately
            Assert.Greater(user.Points, oldPoints);

        }


        [Test]
        public async Task Command_Point_Success()
        {
            var user = await _userRepository.GetUser("User1Id");
            var list = user.MissionSetIds.ToList();
            list.Add(new MissionSetIdWithOrder { MissionSetId =  GameConstants.MissionSet.FirstSetId, Order = 3 });
            list.Add(new MissionSetIdWithOrder { MissionSetId = GameConstants.MissionSet.LastSetId, Order = 4 });
            user.MissionSetIds = list;

            var list2 = user.ActiveMissionSetIds.ToList();
            list2.Add(GameConstants.MissionSet.FirstSetId);
            user.ActiveMissionSetIds = list2;


            var oldHomeCoordinate = user.HomeCoordinate;
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("5823973e-ac62-4efe-9c5d-1755df874d99", new MissionProof { Coordinates = new List<GeoCoordinate> { new GeoCoordinate(51.9512481, 29.1605902) } });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual(MissionCompletionStatus.Success, result.MissionCompletionStatus);
            Assert.AreNotEqual(oldHomeCoordinate, user.HomeCoordinate);
            Assert.AreNotEqual(GeoCoordinate.Unknown, user.HomeCoordinate);
            Assert.Greater(user.Points, oldPoints);
            Assert.IsFalse(string.IsNullOrEmpty(user.UniqueCityId));
        }

        [Test]
        public async Task Command_Point_Invalid_Home()
        {
            var user = await _userRepository.GetUser("User1Id");
            var list = user.MissionSetIds.ToList();
            list.Add(new MissionSetIdWithOrder { MissionSetId = GameConstants.MissionSet.FirstSetId, Order = 3 });
            list.Add(new MissionSetIdWithOrder { MissionSetId = GameConstants.MissionSet.LastSetId, Order = 4 });
            user.MissionSetIds = list;

            var list2 = user.ActiveMissionSetIds.ToList();
            list2.Add(GameConstants.MissionSet.FirstSetId);
            user.ActiveMissionSetIds = list2;

            var result = await _controller.CompleteMission("5823973e-ac62-4efe-9c5d-1755df874d99", new MissionProof { Coordinates = new List<GeoCoordinate> { new GeoCoordinate(70, 90) } });

            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual(MissionCompletionStatus.IntermediateFail, result.MissionCompletionStatus);
        }

        [Test]
        public async Task Healthy_Spirit1_Success()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("8ab5fa9b-15a7-49ec-8d5f-2a47d4affd52", new MissionProof { CreatedText = "120" });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Success);
            Assert.AreEqual(result.StarsCount, 3);
            Assert.Greater(user.Points, oldPoints);

        }

        [Test]
        public async Task Healthy_Spirit1_Fail()
        {
            var user = await _userRepository.GetUser("User1Id");
            /*var oldEastCoordinate = user.BaseEastCoordinate;*/
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("8ab5fa9b-15a7-49ec-8d5f-2a47d4affd52", new MissionProof { CreatedText = "181" });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Fail);
            Assert.Less(user.Points, oldPoints);

        }

        [Test]
        public async Task Your_Base_Success()
        {
            var user = await _userRepository.GetUser("User1Id");

            user.HomeCoordinate = new GeoCoordinate(43.4650677153926, -80.5223608016968);

            var oldEast = user.BaseEastCoordinate;
            var oldPoints = user.Points;

            var result =
                await
                _controller.CompleteMission(
                    "71130d2e-e513-4b29-ad12-7a368f4d927a",
                    new MissionProof
                        {
                            Coordinates = //NESW
                                new List<GeoCoordinate>
                                    {
                                        new GeoCoordinate(43.4664381984673, -80.5222320556641),
                                        new GeoCoordinate(43.4650365676888, -80.5205154418945),
                                        new GeoCoordinate(43.4636193301921, -80.5220818519592),
                                        new GeoCoordinate(43.4650988630802, -80.5248284339905)
                                    }
                        });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Success);
            Assert.Greater(user.Points, oldPoints);
            Assert.AreNotEqual(oldEast, user.BaseEastCoordinate);
            Assert.AreNotEqual(GeoCoordinate.Unknown, user.BaseEastCoordinate);            
        }

        [Test]
        public async Task Your_Base_Fail()
        {
            var user = await _userRepository.GetUser("User1Id");

            var oldEast = user.BaseEastCoordinate;
            var oldPoints = user.Points;

            var result =
                await
                _controller.CompleteMission(
                    "71130d2e-e513-4b29-ad12-7a368f4d927a",
                    new MissionProof
                        {
                            Coordinates =
                                new List<GeoCoordinate>
                                    {
                                        new GeoCoordinate(53.901763, 27.566661),
                                        new GeoCoordinate(60.900031, 27.569204),
                                        new GeoCoordinate(53.900031, 27.563443),
                                        new GeoCoordinate(53.898407, 27.566640) //NEWS
                                    }
                        });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Fail);
            Assert.Less(user.Points, oldPoints);
            Assert.AreEqual(oldEast, user.BaseEastCoordinate);
        }

        [Test]
        public async Task Help_Choice_Success()
        {
            var user = await _userRepository.GetUser("User1Id");

            var oldPoints = user.Points;

            var result =
                await
                _controller.CompleteMission(
                    "19e18c3f-2daa-4e18-bf32-2a1f77fdc73f",
                    new MissionProof
                    {
                        CreatedText = "И другу, и Маме"
                    });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Success);
            Assert.Greater(user.Points, oldPoints);

        }

        [Test]
        public async Task Help_Choice_Fail()
        {
            var user = await _userRepository.GetUser("User1Id");

            var oldPoints = user.Points;

            var result =
               await
               _controller.CompleteMission(
                   "19e18c3f-2daa-4e18-bf32-2a1f77fdc73f",
                   new MissionProof
                   {
                       CreatedText = "Себе"
                   });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Fail);
            Assert.Less(user.Points, oldPoints);
        }


        [Test]
        public async Task Radar_Success()
        {
            var user = await _userRepository.GetUser("User1Id");
            var list = user.MissionSetIds.ToList();
            list.Add(new MissionSetIdWithOrder { MissionSetId = "RadarSet", Order = 3 });
            list.Add(new MissionSetIdWithOrder { MissionSetId = GameConstants.MissionSet.LastSetId, Order = 4 });
            user.MissionSetIds = list;

            var list2 = user.ActiveMissionSetIds.ToList();
            list2.Add("RadarSet");
            user.ActiveMissionSetIds = list2;

            var oldPoints = user.Points;
            var radarCoordinate = new GeoCoordinate(53.900493, 27.567513);

            var result =
                await
                _controller.CompleteMission(
                    "b23f79fe-08d3-4d24-a293-51f67e2131be",
                    new MissionProof
                    {
                        Coordinates = new List<GeoCoordinate> { radarCoordinate },
                        TimeElapsed = 540
                    });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Success);
            Assert.AreEqual(radarCoordinate, user.RadarCoordinate);
            Assert.Greater(user.Points, oldPoints);

        }

        [Test]
        public async Task Radar_Fail()
        {
            var user = await _userRepository.GetUser("User1Id");
            var list = user.MissionSetIds.ToList();
            list.Add(new MissionSetIdWithOrder { MissionSetId = "RadarSet", Order = 3 });
            list.Add(new MissionSetIdWithOrder { MissionSetId = GameConstants.MissionSet.LastSetId, Order = 4 });
            user.MissionSetIds = list;

            var list2 = user.ActiveMissionSetIds.ToList();
            list2.Add("RadarSet");
            user.ActiveMissionSetIds = list2;


            var oldPoints = user.Points;
            var radarCoordinate = new GeoCoordinate(54.900492, 27.567513);

            var result =
                await
                _controller.CompleteMission(
                    "b23f79fe-08d3-4d24-a293-51f67e2131be",
                    new MissionProof
                    {
                        Coordinates = new List<GeoCoordinate> { radarCoordinate },
                        TimeElapsed = 540
                    });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.IntermediateFail);
            Assert.AreNotEqual(radarCoordinate, user.RadarCoordinate);
            Assert.AreEqual(user.Points, oldPoints);

        }

        [Test]
        public async Task Censored_Success()
        {
            var user = await _userRepository.GetUser("User1Id");

            var oldPoints = user.Points;

            var result = await _controller.DeclineMission("524b9f98-88b2-460a-ae3f-e15df4cef2ae");

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Success);
            Assert.Greater(user.Points, oldPoints);

        }

        [Test]
        public async Task Censored_Fail()
        {
            var user = await _userRepository.GetUser("User1Id");

            var oldPoints = user.Points;

            var result =
               await
               _controller.CompleteMission(
                   "524b9f98-88b2-460a-ae3f-e15df4cef2ae",
                   new MissionProof
                   {
                       CreatedText = "XXX"
                   });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Fail);
            Assert.Less(user.Points, oldPoints);
        }

        [Test]
        public async Task Friends_Base_Success()
        {
            var user = await _userRepository.GetUser("User1Id");

            var oldPoints = user.Points;

            var result =
                await
                _controller.CompleteMission(
                    "2c187275-97c0-4dcf-a1f0-89d7018a03e2",
                    new MissionProof
                    {
                        Coordinates = new List<GeoCoordinate> { new GeoCoordinate(55.55542, 28.77782) }
                    });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(MissionCompletionStatus.Success, result.MissionCompletionStatus);
            Assert.Greater(user.Points, oldPoints);

        }

        [Test]
        public async Task Friends_Base_Fail()
        {
            var user = await _userRepository.GetUser("User1Id");

            var oldPoints = user.Points;

            var result =
               await
               _controller.CompleteMission(
                    "2c187275-97c0-4dcf-a1f0-89d7018a03e2",
                   new MissionProof
                   {
                       Coordinates = new List<GeoCoordinate> { new GeoCoordinate(53.9, 27.56667) }
                   });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(OperationResultStatus.Success, result.Status);
            Assert.AreEqual(MissionCompletionStatus.Fail, result.MissionCompletionStatus);
            Assert.Less(user.Points, oldPoints);
        }


        [Test]
        public async Task Perpetum_Mobile_Success()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPoints = user.Points;

            var result = await _controller.CompleteMission("3be2d03c-f100-42be-bc9f-54166e108d43", 
                new MissionProof { NumberOfTries = 3 });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Success);
            Assert.Greater(user.Points, oldPoints);

        }

        [Test]
        public async Task Outpost_Success()
        {
            var user = await _userRepository.GetUser("User1Id");
            var list = user.MissionSetIds.ToList();
            list.Add(new MissionSetIdWithOrder { MissionSetId = "OutpostSet", Order = 3 });
            list.Add(new MissionSetIdWithOrder { MissionSetId = GameConstants.MissionSet.LastSetId, Order = 4 });
            user.MissionSetIds = list;

            var list2 = user.ActiveMissionSetIds.ToList();
            list2.Add("OutpostSet");
            user.ActiveMissionSetIds = list2;


            var oldPoints = user.Points;
            var outpostCoordinate = new GeoCoordinate(53.901801, 27.569953);

            var result =
                await
                _controller.CompleteMission(
                    "8c67c3de-5458-4c6a-a09e-be2d06cdcb2e",
                    new MissionProof
                    {
                        Coordinates = new List<GeoCoordinate> { outpostCoordinate }
                    });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.Success);
            Assert.AreEqual(outpostCoordinate, user.OutpostCoordinate);
            Assert.Greater(user.Points, oldPoints);

        }

        [Test]
        public async Task Outpost_Fail()
        {
            var user = await _userRepository.GetUser("User1Id");
            var list = user.MissionSetIds.ToList();
            list.Add(new MissionSetIdWithOrder{MissionSetId = "OutpostSet", Order = 3});
            list.Add(new MissionSetIdWithOrder { MissionSetId = GameConstants.MissionSet.LastSetId, Order = 4 });
            user.MissionSetIds = list;

            var list2 = user.ActiveMissionSetIds.ToList();
            list2.Add("OutpostSet");
            user.ActiveMissionSetIds = list2;


            var oldPoints = user.Points;
            var outpostCoordinate = new GeoCoordinate(54.901801, 27.569953);

            var result =
                await
                _controller.CompleteMission(
                    "8c67c3de-5458-4c6a-a09e-be2d06cdcb2e",
                    new MissionProof
                    {
                        Coordinates = new List<GeoCoordinate> { outpostCoordinate }
                    });

            user = await _userRepository.GetUser("User1Id");

            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(result.MissionCompletionStatus, MissionCompletionStatus.IntermediateFail);
            Assert.AreNotEqual(outpostCoordinate, user.OutpostCoordinate);
            Assert.AreEqual(user.Points, oldPoints);

        }


        [TearDown]
        public void Dispose()
        {
            _appCountersRepository.Dispose();
            _commonPlacesRepository.Dispose();
            _missionRepository.Dispose();
            _missionRequestRepository.Dispose();
            _missionSetRepository.Dispose();
            _userRepository.Dispose();
            _service.Dispose();
            _controller.Dispose();
        }
    }
}