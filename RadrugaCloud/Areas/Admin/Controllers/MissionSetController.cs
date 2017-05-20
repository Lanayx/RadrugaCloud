namespace RadrugaCloud.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Mvc;

    using Core.CommonModels.Query;
    using Core.CommonModels.Results;
    using Core.DomainModels;
    using Core.Enums;
    using Models;
    using Helpers;

    using Services.DomainServices;

    /// <summary>
    ///  The mission set controller.
    /// </summary>
    [System.Web.Mvc.Authorize]
    public class MissionSetController : Controller
    {
        #region Constants

        private const string MissionsTempKey = "CurrentMissions";

        #endregion

        #region Fields

        private readonly MissionService _missionService;

        private readonly MissionSetService _missionSetService;

        private readonly PersonQualityService _personQualityService;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MissionSetController" /> class.
        /// </summary>
        /// <param name="missionSetService">The mission set service.</param>
        /// <param name="missionService">The mission service.</param>
        /// <param name="personQualityService">The person quality service.</param>
        public MissionSetController(MissionSetService missionSetService, MissionService missionService, PersonQualityService personQualityService)
        {
            _missionSetService = missionSetService;
            _missionService = missionService;
            _personQualityService = personQualityService;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates this instance.
        /// </summary>
        /// <returns>
        ///     ActionResult.
        /// </returns>
        public async Task<ActionResult> Create()
        {
            var model = new MissionSetUI();
            ViewBag.Missions = await GetBaseMissions();
            SetCurrentMissionsToTempData(model);
            return View(model);
        }

        // GET: Missions/Edit/5

        /// <summary>
        ///     Creates the specified mission.
        /// </summary>
        /// <param name="missionSet">The mission.</param>
        /// <returns>Task{ActionResult}.</returns>
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MissionSetUI missionSet)
        {
            return await AddUpdateMissionSet(missionSet, true);
        }

        // POST: Missions/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.

        /// <summary>
        ///     The delete.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var missionSet = await _missionSetService.GetMissionSet(id);
            if (missionSet == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonQualities = await _personQualityService.GetPersonQualities();
            return View(missionSet);
        }

        // POST: Missions/Delete/5

        /// <summary>
        ///     The delete confirmed.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            await _missionSetService.DeleteMissionSet(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        ///     The details.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var missionSet = await _missionSetService.GetMissionSet(id);
            if (missionSet == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonQualities = await _personQualityService.GetPersonQualities();
            return View(missionSet);
        }

        /// <summary>
        ///     The edit.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var missionSet = (await _missionSetService.GetMissionSet(id)).ConvertToUi();
            if (missionSet == null)
            {
                return HttpNotFound();
            }

            SetCurrentMissionsToTempData(missionSet);
            ViewBag.Missions = await GetBaseMissions();

            return View(missionSet);
        }

        /// <summary>
        ///     The edit.
        /// </summary>
        /// <param name="missionSet">
        ///     The mission draft.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(MissionSetUI missionSet)
        {
            return await AddUpdateMissionSet(missionSet, false);
        }

        /// <summary>
        ///     Indexes the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>
        ///     Task{ActionResult}.
        /// </returns>
        public async Task<ActionResult> Index([FromUri] int? page)
        {
            var pageNumber = page ?? 0;
            const int SetsOnPage = 15;
            var selectOptions = new QueryOptions<MissionSet>
                                    {
                                        /*Filter = p => p.Author == "Defor",*/
                                        Skip = pageNumber * SetsOnPage,
                                        Top = SetsOnPage + 1,
                                        Select =
                                            draft =>
                                            new 
                                            { 
                                                draft.Id, 
                                                draft.Name, 
                                                draft.Missions 
                                            }
                                    };

            Expression<Func<MissionSet, string>> z = x => x.Name;
            selectOptions.OrderBy = new List<SortDescription> { new SortDescription(z, SortDirection.Ascending) };

            var sets = await _missionSetService.GetMissionSets(selectOptions);
            IEnumerable<MissionSet> model;
            if (sets.Count <= SetsOnPage)
            {
                ViewBag.ShowNext = false;
                model = sets;
            }
            else
            {
                ViewBag.ShowNext = true;
                model = sets.Take(SetsOnPage);
            }

            ViewBag.ShowPrevious = pageNumber != 0;
            ViewBag.CurrentPage = pageNumber;
            return View(model);
        }

        /// <summary>
        ///     Bases the missions editor.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public async Task<ActionResult> SeederMissions()
        {
            var missions = await GetBaseMissions();

            return PartialView("Controls/InSetMissionEditor", missions);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The dispose.
        /// </summary>
        /// <param name="disposing">
        ///     The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _missionService.Dispose();
                _missionSetService.Dispose();
            }

            base.Dispose(disposing);
        }

        private async Task<ActionResult> AddUpdateMissionSet(MissionSetUI missionSet, bool createOperation)
        {
            if (ModelState.IsValid)
            {
                OperationResult result = createOperation
                                             ? await _missionSetService.AddNewMissionSet(missionSet.ConvertToDomain())
                                             : await _missionSetService.UpdateMissionSet(missionSet.ConvertToDomain());
                if (result.Status != OperationResultStatus.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Description);
                    return View(missionSet);
                }

                return RedirectToAction("Index");
            }

            return View(missionSet);
        }

        private async Task<Dictionary<string, string>> GetBaseMissions()
        {
            var selectOptions = new QueryOptions<Mission> { Select = m => new { m.Id, m.Name } };
            Expression<Func<Mission, string>> z = x => x.Name;
            selectOptions.OrderBy = new List<SortDescription> { new SortDescription(z, SortDirection.Ascending) };

            var missions = await _missionService.GetMissions(selectOptions);

            var currentMissions = GetCurrentMissionsFromTempData();

            var result = missions.Where(m => string.IsNullOrEmpty(m.MissionSetId) || currentMissions.Contains(m.Id))
                    .ToDictionary(m => m.Id, m => m.Name);
            return result;

        }

        private List<string> GetCurrentMissionsFromTempData()
        {
            var ids = TempData[MissionsTempKey] as List<string>;
            if (ids != null)
            {
                TempData.Keep(MissionsTempKey);
            }

            return ids ?? new List<string>();
        }

        private void SetCurrentMissionsToTempData(MissionSetUI model)
        {
            TempData[MissionsTempKey] = model.Missions;
        }

        #endregion
    }
}