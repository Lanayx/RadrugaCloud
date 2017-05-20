namespace RadrugaCloud.Tests.Controllers.Api
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Core.Constants;
    using Core.DomainModels;
    using Core.Enums;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;
    using Core.Tools.CopyHelper;

    using Infrastructure.Repositories.Memory;

    using NUnit.Framework;

    using RadrugaCloud.Controllers.Api;

    using Services.BL;
    using Services.DomainServices;

    /// <summary>
    ///     The question controller test.
    /// </summary>
    [TestFixture]
    public sealed class QuestionControllerTest : IDisposable
    {
        /// <summary>
        ///     The init.
        /// </summary>
        [SetUp]
        public void Init()
        {
            _repository = new QuestionRepository();
            _userRepository = new UserRepository();
            _missionSetRepository = new MissionSetRepository();
            _missionRepository = new MissionRepository();
            _service = new QuestionService(
                _repository,
                _userRepository,
                _missionRepository,
                new AssignmentService(_missionSetRepository, _missionRepository));

            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Sid, "User1Id") }));
            _controller = new QuestionController(_service, new AppCountersService(new AppCountersRepository()))
                              {
                                  User
                                      =
                                      principal
                              };
        }

        [TearDown]
        public void Dispose()
        {
            _repository.Dispose();
            _missionSetRepository.Dispose();
            _missionRepository.Dispose();
            _userRepository.Dispose();
            _service.Dispose();
            _controller.Dispose();
        }

        private QuestionController _controller;

        private IQuestionRepository _repository;

        private QuestionService _service;

        private IUserRepository _userRepository;

        private IMissionRepository _missionRepository;

        private MissionSetRepository _missionSetRepository;

        /// <summary>
        ///     The get current user.
        /// </summary>
        [Test]
        public async Task Answer_Question_Add_Quality()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPersonalQualitiesCount = user.PersonQualitiesWithScores.Count;
            // Действие
            var result =
                await
                _controller.PostQuestionsResults(
                    new[] { new PersonQualityIdWithScore { PersonQualityId = "SomeId", Score = 2 } });

            // Утверждение
            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.Greater(user.PersonQualitiesWithScores.Count, oldPersonalQualitiesCount);
        }

        /// <summary>
        ///     The get current user.
        /// </summary>
        [Test]
        public async Task Answer_Question_Check_SetLine()
        {
            var firstQuestion = (await _service.GetQuestions()).First();
            var user = await _userRepository.GetUser("User1Id");

            // Действие
            var result =
                await
                _controller.PostQuestionsResults(firstQuestion.Options.First().PersonQualitiesWithScores.ToArray());

            // Утверждение
            Assert.AreEqual(result.Status, OperationResultStatus.Success);

            Assert.Greater(user.MissionSetIds.Count(), 3);
            var outpostSetOrder = user.MissionSetIds.First(ms => ms.MissionSetId == "OutpostSet").Order;
            var firstSetOrder =
                user.MissionSetIds.First(ms => ms.MissionSetId == GameConstants.MissionSet.FirstSetId).Order;
            var radarSetOrder = user.MissionSetIds.First(ms => ms.MissionSetId == "RadarSet").Order;
            Assert.Greater(outpostSetOrder, firstSetOrder);
            Assert.Greater(outpostSetOrder, radarSetOrder);
            Assert.Greater(radarSetOrder, firstSetOrder);
        }

        /// <summary>
        ///     The get current user.
        /// </summary>
        [Test]
        public async Task Answer_Question_Update_Quality()
        {
            var user = await _userRepository.GetUser("User1Id");
            var oldPQCount = user.PersonQualitiesWithScores.Count();
            var oldScoreSum = user.PersonQualitiesWithScores.Sum(pq => pq.Score);
            // Действие
            var result =
                await
                _controller.PostQuestionsResults(
                    new[]
                        {
                            new PersonQualityIdWithScore
                                {
                                    PersonQualityId =
                                        GameConstants.PersonQuality.ActivityQualityId,
                                    Score = 2
                                },
                            new PersonQualityIdWithScore
                                {
                                    PersonQualityId =
                                        GameConstants.PersonQuality.CommunicationQualityId,
                                    Score = -5
                                }
                        });

            // Утверждение
            Assert.AreEqual(result.Status, OperationResultStatus.Success);
            Assert.AreEqual(user.PersonQualitiesWithScores.Count(), oldPQCount);

            var newSum = user.PersonQualitiesWithScores.Sum(ptws => ptws.Score);
            Assert.AreNotEqual(newSum, oldScoreSum);
        }
    }
}