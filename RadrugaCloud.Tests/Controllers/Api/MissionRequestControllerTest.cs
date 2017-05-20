namespace RadrugaCloud.Tests.Controllers.Api
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Http.OData;
    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Query;
    using NUnit.Framework;

    using Core.DomainModels;
    using Core.Interfaces.Repositories;
    using Infrastructure.InfrastructureTools;
    using Infrastructure.Repositories.AzureTables;
    using Infrastructure.Repositories.Memory;
    using Services.BL;
    using Services.DomainServices;

    using AppCountersRepository = Infrastructure.Repositories.AzureTables.AppCountersRepository;
    using CommonPlacesRepository = Infrastructure.Repositories.Memory.CommonPlacesRepository;
    using MissionRepository = Infrastructure.Repositories.Memory.MissionRepository;
    using MissionRequestRepository = Infrastructure.Repositories.Memory.MissionRequestRepository;
    using MissionSetRepository = Infrastructure.Repositories.AzureTables.MissionSetRepository;
    using UserRepository = Infrastructure.Repositories.Memory.UserRepository;
    using HintRequestRepository = Infrastructure.Repositories.Memory.HintRequestRepository;

    /// <summary>
    /// The mission request controller test.
    /// </summary>
    [TestFixture]
    public sealed class MissionRequestControllerTest : IDisposable
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
            _userRepository = new UserRepository();
            _ratingRepository = new RatingRepository();
            _ratingService = new RatingService(_userRepository, _ratingRepository, true);
            _missionService = new MissionService(_missonRepository, _userRepository, new MissionSetRepository(), new MissionRequestRepository(), new HintRequestRepository(), _ratingService, _commonPlacesRepository);
            _service = new MissionRequestService(_repository, _missonRepository, _userRepository, _commonPlacesRepository, _ratingService, new NotificationService(null),
                new AppCountersService(new AppCountersRepository()));
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Sid, "User1Id") }));
        }


        private MissionService _missionService;

        private IMissionRepository _missonRepository;

        private CommonPlacesRepository _commonPlacesRepository;

        private IMissionRequestRepository _repository;
        
        private MissionRequestService _service;

        private UserRepository _userRepository;

        private RatingService _ratingService;

        private RatingRepository _ratingRepository;

        /// <summary>
        /// Gets the name of the mission request id by.
        /// </summary>
        [Test]
        public async Task GetMissionRequestIdByName()
        {
            // arrange
            const string ControllerName = "MissionRequest";
            var requestString = string.Concat(
                ConfigurationManager.AppSettings["ApiUrl"],
                ControllerName,
                "?$filter=MissionId eq 'Mission1'&$select=Id&$orderby=LastUpdateDate desc");
            var request = new HttpRequestMessage(HttpMethod.Get, requestString);
           
            ODataModelBuilder modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<MissionRequest>(ControllerName);
            var opts =
                new ODataQueryOptions<MissionRequest>(
                    new ODataQueryContext(modelBuilder.GetEdmModel(), typeof(MissionRequest)), request);

            // Действие
            var results = (await _service.GetRequests(opts.ToQueryOptions()));

            // Утверждение
            Assert.NotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Request1", results.First().Id);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [TearDown]
        public void Dispose()
        {
            _commonPlacesRepository.Dispose();
            _userRepository.Dispose();
            _repository.Dispose();
            _missonRepository.Dispose();
            _missionService.Dispose();
            _service.Dispose();
        }
    }
}