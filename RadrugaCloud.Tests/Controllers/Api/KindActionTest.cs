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

    using Infrastructure.InfrastructureTools.Memory;
    using Infrastructure.Repositories.Memory;

    using NUnit.Framework;
    using RadrugaCloud.Controllers.Api;

    using Services.BL;
    using Services.DomainServices;

    using KindActionRepository = Infrastructure.Repositories.Memory.KindActionRepository;
    using UserRepository = Infrastructure.Repositories.Memory.UserRepository;

    /// <summary>
    ///     The kindAction controller test.
    /// </summary>
    [TestFixture]
    public sealed class KindActionControllerTest:IDisposable
    {
        private KindActionController _controller;
        private IKindActionRepository _repository;
        private IUserRepository _userRepository;
        private KindActionService _service;
        private ImageService _imageService;
        private RatingService _ratingService;

        private RatingRepository _ratingRepository;

        private AppCountersService _appCountersService;

        /// <summary>
        ///     The init.
        /// </summary>
        [SetUp]
        public void Init()
        {
            _repository = new KindActionRepository();
            _userRepository = new UserRepository();
            _ratingRepository = new RatingRepository();
            _ratingService = new RatingService(_userRepository, _ratingRepository, true);
            _service = new KindActionService(_repository, _userRepository, _ratingService);
            _imageService = new ImageService(new ImageProvider(), new UserRepository());
            _appCountersService = new AppCountersService(new AppCountersRepository());
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Sid, "User1Id") }));
            _controller = new KindActionController(_service, _imageService, _appCountersService) { User = principal };
        }

        /// <summary>
        ///  Create kindAction.
        /// </summary>
        [Test]
        public async Task Create()
        {

            // Действие
            var resultsNew = await _controller.PostWithoutImage("I made a house for the birds myself");

            // Утверждение
            Assert.AreEqual(OperationResultStatus.Success, resultsNew.Status);
            var kindActionAdded = (await _controller.Get(null)).FirstOrDefault();

            Assert.NotNull(kindActionAdded);
            Assert.NotNull(kindActionAdded.UserId);
            Assert.AreNotEqual(kindActionAdded.DateAdded, DateTime.MinValue);
            Assert.AreEqual(kindActionAdded.Description, "I made a house for the birds myself");
        }

        /// <summary>
        ///  Create kindAction.
        /// </summary>
        [Test]
        public async Task Like_Success()
        {

            // Действие
            var resultsNew = await _controller.Like(new Models.Api.KindActionIds {
                KindActionId = "Id_KindAction3",
                UserId = "User2Id"
            });

            // Утверждение
            Assert.AreEqual(OperationResultStatus.Success, resultsNew.Status);
            var kindActionLiked = await _repository.GetKindAction("Id_KindAction3", "User2Id");

            Assert.NotNull(kindActionLiked);
            Assert.Contains("User1Id", kindActionLiked.Likes);
        }

        /// <summary>
        ///  Create kindAction.
        /// </summary>
        [Test]
        public async Task Dislike_Fail_Because_Duplicate()
        {

            // Действие
            var resultsNew = await _controller.Dislike(new Models.Api.KindActionIds
            {
                KindActionId = "Id_KindAction2",
                UserId = "User2Id"
            });

            // Утверждение
            Assert.AreEqual(OperationResultStatus.Error, resultsNew.Status);
        }



        [TearDown]
        public void Dispose()
        {
            _userRepository.Dispose();
            _repository.Dispose();
            _service.Dispose();
            _controller.Dispose();
        }
    }
}