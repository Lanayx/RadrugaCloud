namespace RadrugaCloud.Tests.Controllers.Api
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Core.Enums;
    using Core.Interfaces.Repositories;

    using Infrastructure.Repositories.Memory;
    using NUnit.Framework;
    using RadrugaCloud.Controllers.Api;
    using RadrugaCloud.Models.Api;

    using Services.DomainServices;

    /// <summary>
    ///     The appErrorInfo controller test.
    /// </summary>
    [TestFixture]
    public sealed class AppErrorInfoControllerTest:IDisposable
    {
        /// <summary>
        ///     The _controller.
        /// </summary>
        private AppErrorInfoController _controller;

        /// <summary>
        ///     The _repository.
        /// </summary>
        private IAppErrorInfoRepository _repository;

        /// <summary>
        ///     The _service.
        /// </summary>
        private AppErrorInfoService _service;

        /// <summary>
        ///     The init.
        /// </summary>
        [SetUp]
        public void Init()
        {
            _repository = new AppErrorInfoRepository();
            _service = new AppErrorInfoService(_repository);
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Sid, "User1Id") }));
            _controller = new AppErrorInfoController(_service) { User = principal };
        }

        /// <summary>
        ///  Create appErrorInfo.
        /// </summary>
        [Test]
        public async Task Create()
        {

            // Действие
            var appErrorInfo =  new AppErrorInfo
                        {
                            UserId = "User1Id",
                            CurrentViewName = "Mission",
                            DeviceModel = "Passion",
                            DevicePlatform = "Android",
                            DeviceVersion = "4.3",
                            AppVersion = "1.1",
                            ErrorData = "Connection failed again and again",
                            ErrorTime = DateTime.UtcNow
                        };

            var resultsNew = await _controller.Post(appErrorInfo);

            // Утверждение
            Assert.AreEqual(OperationResultStatus.Success, resultsNew.Status);
            var appErrorInfoAdded = await _repository.GetAppErrorInfo(resultsNew.Id);
            Assert.AreEqual(appErrorInfoAdded.ErrorData, "Connection failed again and again");

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