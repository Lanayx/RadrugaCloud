namespace RadrugaCloud.Tests.Controllers.Site
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

   
        public async Task Create()
        {
            // Действие
            var result = (await _controller.Index(null)) as ViewResult;
            var personQualities = result.Model as IEnumerable<PersonQuality>;
            int count = personQualities.Count();
            var newPersonQuality = new PersonQuality
            {
                Name = "New personQuality",
                Description = "Description"
            };
            result = (await _controller.Create(newPersonQuality)) as ViewResult;
            // Утверждение
            Assert.IsNotNull(personQualities);
            Assert.IsNotEmpty(personQualities);
            Assert.AreEqual(personQualities.Last().Name, "New personQuality");
            Assert.AreEqual(count + 1, personQualities.Count());
        }

        [Test]
        public async Task Index()
        {
            // Действие
            var result = (await _controller.Index(null)) as ViewResult;

            // Утверждение
            Assert.IsNotNull(result);
            var personQualities = result.Model as IEnumerable<PersonQuality>;
            Assert.IsNotNull(personQualities);
            Assert.IsNotEmpty(personQualities);
        }

        [Test]
        public async Task Index_paging()
        {
            // Действие
            var result = (await _controller.Index(1)) as ViewResult;

            // Утверждение
            Assert.IsNotNull(result);
            var personQualities = result.Model as IEnumerable<PersonQuality>;
            Assert.IsNotNull(personQualities);
            Assert.IsEmpty(personQualities);
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