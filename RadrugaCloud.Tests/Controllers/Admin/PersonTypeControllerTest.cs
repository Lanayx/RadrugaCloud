namespace RadrugaCloud.Tests.Controllers.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Areas.Admin.Controllers;

    using Core.DomainModels;
    using Core.Interfaces.Repositories;
    using Infrastructure.Repositories.Memory;
    using Models;
    using NUnit.Framework;
    using Services.DomainServices;

    [TestFixture]
    internal class PersonQualityControllerTest:IDisposable
    {
        private IPersonQualityRepository _repository;
        private PersonQualityController _controller;
        private PersonQualityService _service;

        [SetUp]
        public void Init()
        {
            _repository = new PersonQualityRepository();
            _service = new PersonQualityService(_repository);
            _controller = new PersonQualityController(_service);
        }

        [Test]
        public async Task Delete()
        {
            // Действие
            var result = (await _controller.Index(null)) as ViewResult;
            var personQualities = result.Model as IEnumerable<PersonQuality>;
            int personQualitiesCount = personQualities.Count();
            PersonQuality personQuality = personQualities.First();

            var deleteResult = (await _controller.DeleteConfirmed(personQuality.Id)) as RedirectToRouteResult;

            result = (await _controller.Index(null)) as ViewResult;
            var personQualitiesAfterDelete = result.Model as IEnumerable<PersonQuality>;

            // Утверждение
            Assert.IsNotNull(personQualitiesAfterDelete);
            Assert.IsNotEmpty(personQualitiesAfterDelete);
            Assert.AreEqual(personQualitiesCount - 1, personQualitiesAfterDelete.Count());
        }

        [Test]
        public async Task Delete_view()
        {
            // Действие
            var result = (await _controller.Index(null)) as ViewResult;
            var personQualities = result.Model as IEnumerable<PersonQuality>;
            PersonQuality personQuality = personQualities.First();

            result = (await _controller.Delete(personQuality.Id)) as ViewResult;
            var editDraft = result.Model as PersonQuality;

            // Утверждение
            Assert.IsNotNull(editDraft);
            Assert.AreEqual(editDraft.Id, personQuality.Id);
        }

        [Test]
        public async Task Edit()
        {
            // Действие
            var result = (await _controller.Index(null)) as ViewResult;
            var personQualities = result.Model as IEnumerable<PersonQuality>;
            PersonQuality oldDraft = personQualities.First();
            var personQuality = new PersonQuality
            {
                Id = oldDraft.Id,
                Name = "Updated personQuality",
                Description = oldDraft.Description
            };
            var editResult = (await _controller.Edit(personQuality)) as RedirectToRouteResult;

            result = (await _controller.Details(oldDraft.Id)) as ViewResult;
            var personQualityAfterEdit = result.Model as PersonQuality;

            // Утверждение
            Assert.IsNotNull(editResult);
            Assert.IsNotNull(personQualities);
            Assert.IsNotEmpty(personQualities);
            Assert.AreEqual("Updated personQuality", personQualityAfterEdit.Name);
        }

        [Test]
        public async Task Edit_view()
        {
            // Действие
            var result = (await _controller.Index(null)) as ViewResult;
            var personQualities = result.Model as IEnumerable<PersonQuality>;
            PersonQuality personQuality = personQualities.First();

            result = (await _controller.Edit(personQuality.Id)) as ViewResult;
            var editDraft = result.Model as PersonQuality;

            // Утверждение
            Assert.IsNotNull(editDraft);
            Assert.AreEqual(editDraft.Id, personQuality.Id);
        }

    

        [TearDown]
        public void Dispose()
        {
            _repository.Dispose();
            _service.Dispose();
            _controller.Dispose();
        }
    }
}