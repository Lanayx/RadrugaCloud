namespace RadrugaCloud.Controllers.Api
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.OData.Query;

    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Core.NonDomainModels;

    using Services.DomainServices;
    using Infrastructure.InfrastructureTools;
    using Helpers;

    /// <summary>
    /// Class QuestionController
    /// </summary>
    [Authorize]
    public class QuestionController : ApiController
    {
        private readonly QuestionService _questionService;

        private readonly AppCountersService _appCountersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionController" /> class.
        /// </summary>
        /// <param name="questionService">The question service.</param>
        /// <param name="appCountersService">The application counters service.</param>
        public QuestionController(QuestionService questionService, AppCountersService appCountersService)
        {
            _questionService = questionService;
            _appCountersService = appCountersService;
        }

        /// <summary>
        /// Gets the specified odata options.
        /// </summary>
        /// <returns>Task{IEnumerable{Question}}.</returns>
        public Task<List<Question>> Get(ODataQueryOptions<Question> odataOptions)
        {
            return _questionService.GetQuestions(odataOptions.ToQueryOptions());
        }

        /// <summary>
        /// Posts the questions results.
        /// </summary>
        /// <param name="questionsResults">The questions results.</param>
        /// <returns></returns>
        public async Task<ColorResult> PostQuestionsResults(PersonQualityIdWithScore[] questionsResults)
        {
            var id = this.GetCurrentUserId();
            var result = await _questionService.AnswerQuestions(id, questionsResults);
            if (result.Status == OperationResultStatus.Success) 
                await _appCountersService.QuestionsAnswered();
            return result;
        }
    }
}