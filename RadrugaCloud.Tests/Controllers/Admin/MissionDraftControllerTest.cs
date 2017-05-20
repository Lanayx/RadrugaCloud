namespace RadrugaCloud.Tests.Controllers.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Areas.Admin.Controllers;

    using Core.CommonModels.Query;
    using Core.DomainModels;
    using Core.Interfaces.Repositories;

    using Infrastructure.Repositories.AzureTables;
    using Infrastructure.Repositories.Memory;

    using Models;
    using NUnit.Framework;
    using Services.DomainServices;

    using MissionDraftRepository = Infrastructure.Repositories.Memory.MissionDraftRepository;
    using MissionRepository = Infrastructure.Repositories.Memory.MissionRepository;
    using MissionRequestRepository = Infrastructure.Repositories.Memory.MissionRequestRepository;
    using MissionSetRepository = Infrastructure.Repositories.AzureTables.MissionSetRepository;
    using PersonQualityRepository = Infrastructure.Repositories.Memory.PersonQualityRepository;
    using UserRepository = Infrastructure.Repositories.Memory.UserRepository;
    using HintRequestRepository = Infrastructure.Repositories.Memory.HintRequestRepository;
    using CommonPlaceRepository = Infrastructure.Repositories.Memory.CommonPlacesRepository;

    [TestFixture]
    internal class MissionDraftControllerTest : IDisposable
    {
        private IMissionDraftRepository _repository;
        private MissionDraftController _controller;
        private MissionDraftService _service;

        private PersonQualityRepository _personQualityRepository;
        private PersonQualityService _personQualityService;
        private UserRepository _userRepository;
        private MissionRepository _missionRepository;
        private MissionService _missionService;

        private IRatingRepository _ratingRepository;
        private RatingService _ratingService;

        [SetUp]
        public void Init()
        {
            _repository = new MissionDraftRepository();
            _personQualityRepository = new PersonQualityRepository();
            _userRepository = new UserRepository();
            _ratingRepository = new RatingRepository();
            _missionRepository = new MissionRepository();
            _service = new MissionDraftService(_repository);
            _personQualityService = new PersonQualityService(_personQualityRepository);
            _ratingService = new RatingService(_userRepository, _ratingRepository, true);
            _missionService = new MissionService(_missionRepository, _userRepository, new MissionSetRepository(), new MissionRequestRepository(), new HintRequestRepository(), _ratingService, new CommonPlaceRepository());
            _controller = new MissionDraftController(_service, _personQualityService, _missionService);
        }

        [Test]
        public async Task Delete()
        {
            // Действие
            var result = (await _controller.Index(null)) as ViewResult;
            var drafts = result.Model as IEnumerable<MissionDraft>;
            int draftsCount = drafts.Count();
            var draft = drafts.First();

            var deleteResult = (await _controller.DeleteConfirmed(draft.Id)) as RedirectToRouteResult;

            result = (await _controller.Index(null)) as ViewResult;
            var draftsAfterDelete = result.Model as IEnumerable<MissionDraft>;

            // Утверждение
            Assert.IsNotNull(deleteResult);
            Assert.IsNotNull(draftsAfterDelete);
            Assert.IsNotEmpty(draftsAfterDelete);
            Assert.AreEqual(draftsCount - 1, draftsAfterDelete.Count());
        }

        [Test]
        public async Task Delete_view()
        {
            // Действие
            var result = (await _controller.Index(null)) as ViewResult;
            var drafts = result.Model as IEnumerable<MissionDraft>;
            var draft = drafts.First();

            result = (await _controller.Delete(draft.Id)) as ViewResult;
            var editDraft = result.Model as MissionDraft;

            // Утверждение
            Assert.IsNotNull(editDraft);
            Assert.AreEqual(editDraft.Id, draft.Id);
        }

        [Test]
        public async Task Edit()
        {
            // Действие
            var result = (await _controller.Index(null)) as ViewResult;
            var drafts = result.Model as IEnumerable<MissionDraft>;
            var oldDraft = drafts.Last();
            var draft = new MissionDraftUi
            {
                Id = oldDraft.Id,
                Name = "Updated draft",
                Description = oldDraft.Description,
                Difficulty = oldDraft.Difficulty
            };
            var editResult = (await _controller.Edit(draft)) as RedirectToRouteResult;

            result = (await _controller.Details(oldDraft.Id)) as ViewResult;
            var draftAfterEdit = result.Model as MissionDraft;

            // Утверждение
            Assert.IsNotNull(editResult);
            Assert.IsNotNull(drafts);
            Assert.IsNotEmpty(drafts);
            Assert.AreEqual("Updated draft", draftAfterEdit.Name);
        }

        [Test]
        public async Task Edit_view()
        {
            // Действие
            var result = (await _controller.Index(null)) as ViewResult;
            var drafts = result.Model as IEnumerable<MissionDraft>;
            var draft = drafts.First();

            result = (await _controller.Edit(draft.Id)) as ViewResult;
            var editDraft = result.Model as MissionDraftUi;

            // Утверждение
            Assert.IsNotNull(editDraft);
            Assert.AreEqual(editDraft.Id, draft.Id);
        }

        [Test]
        public async Task ConvertDraftToMission()
        {
            // Действие
            var result = (await _controller.Index(null)) as ViewResult;
            var drafts = result.Model as IEnumerable<MissionDraft>;
            var draft = drafts.First();

            var actionResult = (await _controller.CreateMissionFromDraft(draft.Id)) as RedirectToRouteResult;
            Assert.NotNull(actionResult);

            result = (await _controller.Index(null)) as ViewResult;
            var draftsAfterDelete = result.Model as IEnumerable<MissionDraft>;
            var oldDraft = draftsAfterDelete.FirstOrDefault(dr => dr.Id == draft.Id);

            var newMission =
                 await _missionService.GetMissions(
                    new QueryOptions<Mission> { Filter = mission => mission.Name == draft.Name });

            // Утверждение
            Assert.IsNull(oldDraft);
            Assert.IsNotEmpty(newMission);
            Assert.NotNull(newMission.First());
        }

        [TearDown]
        public void Dispose()
        {
            _missionRepository.Dispose();
            _userRepository.Dispose();
            _repository.Dispose();
            _personQualityRepository.Dispose();
            _service.Dispose();
            _personQualityService.Dispose();
            _controller.Dispose();
        }
    }
}