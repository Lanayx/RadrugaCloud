namespace RadrugaCloud.Tests.Controllers.Site
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Core.DomainModels;
    using Core.Interfaces.Repositories;
    using Infrastructure.Repositories.Memory;
    using Models;
    using NUnit.Framework;
    using RadrugaCloud.Controllers;
    using Services.DomainServices;

    [TestFixture]
    internal sealed class MissionDraftControllerTest:IDisposable
    {
        private IMissionDraftRepository _repository;
        private MissionDraftController _controller;
        private MissionDraftService _service;

        private PersonQualityRepository _personQualityRepository;
        private PersonQualityService _personQualityService;

        [SetUp]
        public void Init()
        {
            _repository = new MissionDraftRepository();
            _personQualityRepository = new PersonQualityRepository();
            _service = new MissionDraftService(_repository);
            _personQualityService = new PersonQualityService(_personQualityRepository);
            _controller = new MissionDraftController(_service,_personQualityService);
        }

        public async Task Create()
        {
            // Действие
            var result = (await _controller.Index(null)) as ViewResult;
            var drafts = result.Model as IEnumerable<MissionDraft>;
            int count = drafts.Count();
            var newMissionDraft = new MissionDraftUi
            {
                Name = "New draft",
                Description = "Description",
                PhotoUrl = "http://vk.com",
                Difficulty = 1
            };
            result = (await _controller.Create(newMissionDraft)) as ViewResult;
            // Утверждение
            Assert.IsNotNull(drafts);
            Assert.IsNotEmpty(drafts);
            Assert.AreEqual(drafts.Last().Name, "New draft");
            Assert.AreEqual(count + 1, drafts.Count());
        }

        [Test]
        public async Task Index()
        {
            // Действие
            var result = (await _controller.Index(null)) as ViewResult;

            // Утверждение
            Assert.IsNotNull(result);
            var drafts = result.Model as IEnumerable<MissionDraft>;
            Assert.IsNotNull(drafts);
            Assert.IsNotEmpty(drafts);
        }

        [Test]
        public async Task Index_paging()
        {
            // Действие
            var result = (await _controller.Index(1)) as ViewResult;

            // Утверждение
            Assert.IsNotNull(result);
            var drafts = result.Model as IEnumerable<MissionDraft>;
            Assert.IsNotNull(drafts);
            Assert.IsEmpty(drafts);
        }

        [TearDown]
        public void Dispose()
        {

            _repository.Dispose();
            _personQualityRepository.Dispose();
            _service.Dispose();
            _personQualityService.Dispose();
            _controller.Dispose();
        }
    }
}