namespace RadrugaCloud.Tests.Controllers.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Core.DomainModels;
    using Core.Interfaces.Repositories;
    using Core.NonDomainModels;

    using Infrastructure.Repositories.Memory;

    using NUnit.Framework;

    using RadrugaCloud.Areas.Admin.Controllers;
    using RadrugaCloud.Areas.Admin.Models;

    using Services.BL;
    using Services.DomainServices;

    using MissionSetRepository = Infrastructure.Repositories.AzureTables.MissionSetRepository;

    [TestFixture]
    internal class QuestionControllerTest : IDisposable
    {
        [SetUp]
        public void Init()
        {
            _repository = new QuestionRepository();
            _personQualityRepository = new PersonQualityRepository();
            _userRepository = new UserRepository();
            _missionRepository = new MissionRepository();
            _missionSetRepository = new MissionSetRepository();
            _service = new QuestionService(
                _repository,
                _userRepository,
                _missionRepository,
                new AssignmentService(_missionSetRepository, _missionRepository));
            _personQualityService = new PersonQualityService(_personQualityRepository);
            _controller = new QuestionController(_service, _personQualityService);
        }

        [TearDown]
        public void Dispose()
        {
            _missionRepository.Dispose();
            _missionSetRepository.Dispose();
            _userRepository.Dispose();
            _repository.Dispose();
            _personQualityRepository.Dispose();
            _service.Dispose();
            _personQualityService.Dispose();
            _controller.Dispose();
        }

        private IQuestionRepository _repository;

        private IMissionSetRepository _missionSetRepository;

        private QuestionController _controller;

        private QuestionService _service;

        private IMissionRepository _missionRepository;

        private PersonQualityRepository _personQualityRepository;

        private PersonQualityService _personQualityService;

        private UserRepository _userRepository;

        [Test]
        public async Task Delete()
        {
            // Действие
            var result = (ViewResult)await _controller.Index(null);
            var questions =(List<Question>) result.Model;
            var questionsCount = questions.Count;
            var question = questions.First();

            var deleteResult = (await _controller.DeleteConfirmed(question.Id)) as RedirectToRouteResult;

            result = (ViewResult)await _controller.Index(null);
            var questionsAfterDelete = (List<Question>)result.Model;

            // Утверждение
            Assert.IsNotNull(deleteResult);
            Assert.IsNotNull(questionsAfterDelete);
            Assert.IsNotEmpty(questionsAfterDelete);
            Assert.AreEqual(questionsCount - 1, questionsAfterDelete.Count);
        }

        [Test]
        public async Task DeleteView()
        {
            // Действие
            var result = (ViewResult)await _controller.Index(null);
            var questions = (List<Question>)result.Model;
            var question = questions.First();

            result = (ViewResult)await _controller.Delete(question.Id);
            var editQuestion = (Question)result.Model;

            // Утверждение
            Assert.IsNotNull(editQuestion);
            Assert.AreEqual(editQuestion.Id, question.Id);
        }

        [Test]
        public async Task Edit()
        {
            // Действие
            var result = (ViewResult)await _controller.Index(null);
            var questions = (List<Question>)result.Model;
            var oldQuestion = questions.First();
            var question = new QuestionUi
                               {
                                   Id = oldQuestion.Id,
                                   Name = "Updated question",
                                   Text = oldQuestion.Text,
                                   QuestionOptions =
                                       new List<QuestionOptionUI>
                                           {
                                               new QuestionOptionUI
                                                   {
                                                       Number = 0,
                                                       Text = "Brand new option",
                                                       PersonQualitiesWithScores =
                                                           new List<PersonQualityIdWithScore>
                                                               {
                                                                   new PersonQualityIdWithScore
                                                                       {
                                                                           PersonQualityId = "PersonQuality1",
                                                                           Score = -2
                                                                       }
                                                               }
                                                   }
                                           }
                               };
            var editResult = await _controller.Edit(question) as RedirectToRouteResult;

            result = (ViewResult)await _controller.Details(oldQuestion.Id);
            var questionAfterEdit = (Question)result.Model;

            // Утверждение
            Assert.IsNotNull(editResult);
            Assert.IsNotNull(questions);
            Assert.IsNotEmpty(questions);
            Assert.AreEqual("Updated question", questionAfterEdit.Name);
            Assert.AreEqual(
                "PersonQuality1",
                questionAfterEdit.Options.Single().PersonQualitiesWithScores.Single().PersonQualityId);
        }

        [Test]
        public async Task EditView()
        {
            // Действие
            var result = (ViewResult) await _controller.Index(null);
            var questions = (List<Question>) result.Model;
            var question = questions.First();

            result = (ViewResult)await _controller.Edit(question.Id);
            var editQuestion = (QuestionUi)result.Model;

            // Утверждение
            Assert.IsNotNull(editQuestion);
            Assert.AreEqual(editQuestion.Id, question.Id);
        }
    }
}