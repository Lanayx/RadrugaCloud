namespace RadrugaCloud.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Services.DomainServices;

    using Core.CommonModels.Query;
    using Core.DomainModels;
    using Core.Enums;
    using Models;

    /// <summary>
    /// Class MissionDraftController
    /// </summary>
    public class MissionDraftController : Controller
    {
        #region Fields

        /// <summary>
        /// The _mission draft service
        /// </summary>
        private readonly MissionDraftService _missionDraftService;

        /// <summary>
        /// The _person quality service
        /// </summary>
        private readonly PersonQualityService _personQualityService;
        
        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MissionDraftController"/> class.
        /// </summary>
        /// <param name="missionDraftService">The mission draft service.</param>
        /// <param name="personQualityService">The person quality service.</param>
        public MissionDraftController(MissionDraftService missionDraftService, PersonQualityService personQualityService)
        {
            _missionDraftService = missionDraftService;
            _personQualityService = personQualityService;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>
        /// ActionResult.
        /// </returns>
        public ActionResult Create()
        {
            return View(new MissionDraftUi());
        }

        /// <summary>
        /// Creates the specified mission draft.
        /// </summary>
        /// <param name="missionDraft">The mission draft.</param>
        /// <returns>
        /// Task{ActionResult}.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MissionDraftUi missionDraft)
        {
            FixModelAndModelState(missionDraft);
            if (missionDraft != null && ModelState.IsValid)
            {
                await _missionDraftService.AddNewMissionDraft(missionDraft.ConvertToDomain());
                return RedirectToAction("Index");
            }

            return View(missionDraft);
        }

        // GET: Missions/Details/5

        /// <summary>
        /// The details.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var missionDraft = await _missionDraftService.GetMissionDraft(id);
            if (missionDraft == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonQualities = await _personQualityService.GetPersonQualities();
            return View(missionDraft);
        }

        /// <summary>
        /// Indexes the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>
        /// Task{ActionResult}.
        /// </returns>
        public async Task<ActionResult> Index(int? page)
        {
            var pageNumber = page ?? 0;
            const int MissionsOnPage = 15;
            var selectOptions = new QueryOptions<MissionDraft>
                                    {
                                        Skip = pageNumber * MissionsOnPage,
                                        Top = MissionsOnPage + 1,
                                        Select =
                                            draft =>
                                            new
                                                {
                                                    draft.Id,
                                                    draft.Name,
                                                    draft.Description,
                                                    draft.PhotoUrl,
                                                    draft.AgeFrom,
                                                    draft.AgeTo,
                                                    draft.Difficulty,
                                                    draft.Author,
                                                    draft.AddDate
                                                }
                                    };
            Expression<Func<MissionDraft, DateTime>> z = x => x.AddDate;
            selectOptions.OrderBy = new List<SortDescription> { new SortDescription(z, SortDirection.Descending) };
            

            var drafts = await _missionDraftService.GetMissionDrafts(selectOptions);
            IEnumerable<MissionDraft> model;
            if (drafts.Count <= MissionsOnPage)
            {
                ViewBag.ShowNext = false;
                model = drafts;
            }
            else
            {
                ViewBag.ShowNext = true;
                model = drafts.Take(MissionsOnPage);
            }

            ViewBag.ShowPrevious = pageNumber != 0;
            ViewBag.CurrentPage = pageNumber;
            return View(model);
        }

        /// <summary>
        /// Persons the types editor.
        /// </summary>
        /// <returns>Task{ActionResult}.</returns>
        public async Task<ActionResult> PersonQualitiesEditor()
        {
            var personQualities = await _personQualityService.GetPersonQualities();

            return PartialView("Controls/PersonQualitiesEditor", personQualities);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged
        /// resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _missionDraftService.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Fixes the state of the model and model.
        /// </summary>
        /// <param name="missionDraft">The mission draft.</param>
        private void FixModelAndModelState(MissionDraftUi missionDraft)
        {
            if (missionDraft == null)
            {
                return;
            }

            ModelState.Remove("Id");
            missionDraft.AddDate = DateTime.UtcNow;
        }

        #endregion
    }
}