namespace RadrugaCloud.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Core.CommonModels.Query;
    using Core.DomainModels;
    using Core.Enums;
    using Services.DomainServices;
    using Models;
    using Helpers;

    /// <summary>
    /// Class QuestionController
    /// </summary>
    [Authorize]
    public class QuestionController : Controller
    {
        private readonly QuestionService _questionService;
        private readonly PersonQualityService _personQualityService;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionController" /> class.
        /// </summary>
        /// <param name="questionService">The question service.</param>
        /// <param name="personQualityService">The person quality service.</param>
        public QuestionController(QuestionService questionService, PersonQualityService personQualityService)
        {
            _questionService = questionService;
            _personQualityService = personQualityService;
        }
        
        /// <summary>
        /// Indexes the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>Task{ActionResult}.</returns>
        public async Task<ActionResult> Index(int? page)
        {
            var pageNumber = page ?? 0;
            const int QuestionsOnPage = 15;
            var selectOptions = new QueryOptions<Question>
            {
                Skip = pageNumber * QuestionsOnPage,
                Top = QuestionsOnPage + 1,
                Select = question =>
                new
                {
                    question.Id,
                    question.Name,
                    question.Text
                }
            };

            Expression<Func<Question, string>> z = x => x.Name;
            selectOptions.OrderBy = new List<SortDescription> { new SortDescription(z, SortDirection.Ascending) };

            var questions = await _questionService.GetQuestions(selectOptions);
            IEnumerable<Question> model;
            if (questions.Count <= QuestionsOnPage)
            {
                ViewBag.ShowNext = false;
                model = questions;
            }
            else
            {
                ViewBag.ShowNext = true;
                model = questions.Take(QuestionsOnPage);
            }

            ViewBag.ShowPrevious = pageNumber != 0;
            ViewBag.CurrentPage = pageNumber;
            return View(model);
        }

        /// <summary>
        /// Detailses the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{ActionResult}.</returns>
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Question question = await _questionService.GetQuestion(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.PersonQualities = await _personQualityService.GetPersonQualities();
            return View(question);
        }

        // GET: Admin/Questions/Create

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns>Task{ActionResult}.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(QuestionUi question)
        {
            FixModelAndModelState(question);
            if (question != null && ModelState.IsValid)
            {
                await _questionService.AddNewQuestion(question.ConvertToDomain());
                return RedirectToAction("Index");
            }

            return View(question);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{ActionResult}.</returns>
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var question = (await _questionService.GetQuestion(id)).ConvertToUi();
            if (question == null)
            {
                return HttpNotFound();
            }

            var types = await _personQualityService.GetPersonQualities();
            ViewBag.PersonQualities = types;

            return View(question);
        }
        
        /// <summary>
        /// Edits the specified question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns>Task{ActionResult}.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(QuestionUi question)
        {
            var personQualities = await _personQualityService.GetPersonQualities();
            if (ModelState.IsValid)
            {
                await _questionService.UpdateQuestion(question.ConvertToDomain());
                return RedirectToAction("Index");
            }

            ViewBag.PersonQualities = personQualities;
            return View(question);
        }
        
        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{ActionResult}.</returns>
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Question question = await _questionService.GetQuestion(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonQualities = await _personQualityService.GetPersonQualities();
            return View(question);
        }

        /// <summary>
        /// Deletes the confirmed.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{ActionResult}.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            await _questionService.DeleteQuestion(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Persons the options editor.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult OptionsEditor()
        {
            return PartialView("Controls/OptionsEditor");
        }

        /// <summary>
        /// Persons the options editor.
        /// </summary>
        /// <returns>Task{ActionResult}.</returns>
        public async Task<ActionResult> PersonQualitiesWithScoreEditor()
        {
            var personQualities = await _personQualityService.GetPersonQualities();
            ViewData["PersonQualities"] = personQualities;

            return PartialView("Controls/PersonQualitiesWithScoreEditor");
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _questionService.Dispose();
            }

            base.Dispose(disposing);
        }

        private void FixModelAndModelState(QuestionUi questionUi)
        {
            if (questionUi == null)
            {
                return;
            }

            ModelState.Remove("Id");
        }
    }
}
