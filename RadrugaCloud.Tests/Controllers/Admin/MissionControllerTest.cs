namespace RadrugaCloud.Tests.Controllers.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Areas.Admin.Controllers;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.Tools;

    using Infrastructure.Repositories.AzureTables;
    using Infrastructure.Repositories.Memory;

    using Models;
    using NUnit.Framework;
    using Services.DomainServices;

    using CommonPlacesRepository = Infrastructure.Repositories.Memory.CommonPlacesRepository;
    using MissionRepository = Infrastructure.Repositories.Memory.MissionRepository;
    using MissionRequestRepository = Infrastructure.Repositories.AzureTables.MissionRequestRepository;
    using MissionSetRepository = Infrastructure.Repositories.AzureTables.MissionSetRepository;
    using PersonQualityRepository = Infrastructure.Repositories.Memory.PersonQualityRepository;
    using UserRepository = Infrastructure.Repositories.Memory.UserRepository;
    using HintRequestRepository = Infrastructure.Repositories.Memory.HintRequestRepository;

    [TestFixture]
    internal class MissionControllerTest : IDisposable
    {
        private IMissionRepository _repository;
        private MissionController _controller;
        private MissionService _service;

        private ICommonPlacesRepository _commonPlacesRepository;
        private CommonPlacesService _commonPlacesService;

        private PersonQualityRepository _personQualityRepository;
        private PersonQualityService _personQualityService;
        private UserRepository _userRepository;

        private RatingService _ratingService;
        private IRatingRepository _ratingRepository;

        [SetUp]
        public void Init()
        {
            _repository = new MissionRepository();
            _personQualityRepository = new PersonQualityRepository();
            _userRepository = new UserRepository();
            _ratingRepository = new RatingRepository();
            _commonPlacesRepository = new CommonPlacesRepository();
            _commonPlacesService = new CommonPlacesService(_commonPlacesRepository);
            _ratingService = new RatingService(_userRepository, _ratingRepository, true);
            _service = new MissionService(_repository, _userRepository, new MissionSetRepository(), new MissionRequestRepository(), new HintRequestRepository(), _ratingService, _commonPlacesRepository);
            _personQualityService = new PersonQualityService(_personQualityRepository);
            _controller = new MissionController(_personQualityService, _service, _commonPlacesService);
        }

        [Test]
        public async Task CreateRightAnswerMission()
        {
            // Действие
            var missions = await _service.GetMissions();
            int count = missions.Count;

            var newMission = new MissionUi
            {
                Name = "RightAnswer mission",
                Description = "RightAnswer",
                PhotoUrl = "https://pp.vk.me/c543109/v543109262/11668/-YwppEKJVx0.jpg",
                Difficulty = 1,
                ExecutionType = ExecutionType.RightAnswer,
                CorrectAnswers = "один;два",
                AgeFrom = 10,
                AgeTo = 80
            };

            await _controller.Create(newMission);
            missions = await _service.GetMissions();

            // Утверждение
            Assert.IsNotEmpty(missions);
            Assert.AreEqual("RightAnswer mission", missions.Last().Name);
            Assert.AreEqual(count + 1, missions.Count);
        }

        [Test]
        public async Task CreateCommonPlaceMission_Success()
        {
            // Действие
            var missions = await _service.GetMissions();
            int count = missions.Count;

            var newMission = new MissionUi
            {
                Name = "CommonPlace mission",
                Description = "CommonPlace",
                PhotoUrl = "https://pp.vk.me/c543109/v543109262/11668/-YwppEKJVx0.jpg",
                Difficulty = 1,
                ExecutionType = ExecutionType.CommonPlace,
                CommonPlaceAlias = "CM 5",
                AgeFrom = 10,
                AgeTo = 80
            };

            await _controller.Create(newMission);
            missions = await _service.GetMissions();

            // Утверждение
            Assert.IsNotEmpty(missions);
            Assert.AreEqual("CommonPlace mission", missions.Last().Name);
            Assert.AreEqual(count + 1, missions.Count);
        }

        [Test]
        public async Task CreateCommonPlaceMission_Fail()
        {
            // Действие
            var missions = await _service.GetMissions();
            int count = missions.Count;

            var newMission = new MissionUi
            {
                Name = "CommonPlace mission",
                Description = "CommonPlace",
                PhotoUrl = "https://pp.vk.me/c543109/v543109262/11668/-YwppEKJVx0.jpg",
                Difficulty = 1,
                ExecutionType = ExecutionType.CommonPlace,
                CommonPlaceAlias = "CM 4",
                AgeFrom = 10,
                AgeTo = 80
            };

            await _controller.Create(newMission);
            missions = await _service.GetMissions();

            // Утверждение
            Assert.IsNotEmpty(missions);
            Assert.AreEqual(count, missions.Count);
        }

        [Test]
        public async Task Index()
        {
            // Действие
            var result = (await _controller.Index(null)) as ViewResult;

            // Утверждение
            Assert.IsNotNull(result);
            var missions = result.Model as IEnumerable<Mission>;
            Assert.IsNotNull(missions);
            Assert.IsNotEmpty(missions);
        }

        /// <summary>
        /// Indexes the paging.
        /// </summary>
        [Test]
        public async Task IndexPaging()
        {
            // Действие
            var result = (await _controller.Index(4)) as ViewResult;

            // Утверждение
            Assert.IsNotNull(result);
            var missions = result.Model as IEnumerable<Mission>;
            Assert.IsNotNull(missions);
            Assert.IsEmpty(missions);
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [Test]
        public async Task Delete()
        {
            // Действие
            var result = (await _controller.Index(null)) as ViewResult;
            Assert.IsNotNull(result);

            var missions = result.Model as IEnumerable<Mission>;
            Assert.IsNotNull(missions);
            var mission = missions.First();
            var missionId = mission.Id;

            var deleteResult = (await _controller.DeleteConfirmed(mission.Id)) as RedirectToRouteResult;

            result = (await _controller.Index(null)) as ViewResult;
            Assert.IsNotNull(result);

            var missionsAfterDelete = (IEnumerable<Mission>)result.Model;
            Assert.IsNotNull(missionsAfterDelete);
            
            // Утверждение
            Assert.IsNotNull(deleteResult);
            var afterDelete = missionsAfterDelete.ToIList();
            Assert.IsNotEmpty(afterDelete);
            Assert.IsFalse(afterDelete.Select(m => m.Id).Contains(missionId));
        }

        /// <summary>
        /// Deletes the view.
        /// </summary>
        [Test]
        public async Task DeleteView()
        {
            // Действие
            var result = (ViewResult)(await _controller.Index(null));
            var missions = (IEnumerable<Mission>)result.Model;
            var mission = missions.First();

            result = (ViewResult)(await _controller.Delete(mission.Id));
            var editMission = result.Model as Mission;

            // Утверждение
            Assert.IsNotNull(editMission);
            Assert.AreEqual(editMission.Id, mission.Id);
        }

        [Test]
        public async Task Edit()
        {
            // Действие
            var result = (ViewResult)(await _controller.Index(null));
            var missions = (IEnumerable<Mission>)result.Model;
            var oldMission = missions.Last();
            var mission = new MissionUi
            {
                Id = oldMission.Id,
                Name = "Updated mission",
                Description = oldMission.Description,
                Difficulty = oldMission.Difficulty
            };
            var editResult = (RedirectToRouteResult)(await _controller.Edit(mission));

            result = (ViewResult)(await _controller.Details(oldMission.Id));
            var missionAfterEdit = (Mission)result.Model;

            // Утверждение
            Assert.IsNotNull(editResult);
            Assert.IsNotNull(missions);
            
            Assert.AreEqual("Updated mission", missionAfterEdit.Name);
        }

        /// <summary>
        /// Edits the view.
        /// </summary>
        [Test]
        public async Task EditView()
        {
            // Действие
            var result = (ViewResult)(await _controller.Index(null));
            var missions = (IEnumerable<Mission>)result.Model;
            var mission = missions.First();

            result = (ViewResult)(await _controller.Edit(mission.Id));
            var editMission = result.Model as MissionUi;

            // Утверждение
            Assert.IsNotNull(editMission);
            Assert.AreEqual(editMission.Id, mission.Id);
        }

        [TearDown]
        public void Dispose()
        {
            _commonPlacesRepository.Dispose();
            _userRepository.Dispose();
            _repository.Dispose();
            _personQualityRepository.Dispose();
            _service.Dispose();
            _personQualityService.Dispose();
            _controller.Dispose();
        }
    }
}