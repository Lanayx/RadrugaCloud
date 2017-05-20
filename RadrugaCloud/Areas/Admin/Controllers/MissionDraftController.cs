namespace RadrugaCloud.Areas.Admin.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Core.Enums;

    using RadrugaCloud.Helpers;
    using RadrugaCloud.Models;

    using Services.DomainServices;

    /// <summary>
    ///     The mission draft controller.
    /// </summary>
    [Authorize]
    public class MissionDraftController : RadrugaCloud.Controllers.MissionDraftController
    {
        #region Fields

        /// <summary>
        ///     The _mission draft service.
        /// </summary>
        private readonly MissionDraftService _missionDraftService;

        /// <summary>
        ///     The _mission service
        /// </summary>
        private readonly MissionService _missionService;

        /// <summary>
        ///     The _person quality service
        /// </summary>
        private readonly PersonQualityService _personQualityService;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionDraftController" /> class.
        /// </summary>
        /// <param name="missionDraftService">The mission draft service.</param>
        /// <param name="personQualityService">The person quality service.</param>
        /// <param name="missionService">The mission service.</param>
        public MissionDraftController(
            MissionDraftService missionDraftService, 
            PersonQualityService personQualityService, 
            MissionService missionService)
            : base(missionDraftService, personQualityService)
        {
            _missionDraftService = missionDraftService;
            _personQualityService = personQualityService;
            _missionService = missionService;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Creates the mission from draft.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{ActionResult}.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateMissionFromDraft(string id)
        {
            var missionDraft = await _missionDraftService.GetMissionDraft(id);
            if (missionDraft == null)
            {
                return HttpNotFound();
            }

            var result = await _missionService.AddNewMission(missionDraft);
            if (result.Status == OperationResultStatus.Success)
            {
                await _missionDraftService.DeleteMissionDraft(id);
                return RedirectToAction("Index");
            }
            ViewBag.PersonQualities = await _personQualityService.GetPersonQualities();
            return View("Details", missionDraft);
        }

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

            var missionDraft = await _missionDraftService.GetMissionDraft(id);
            if (missionDraft == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonQualities = await _personQualityService.GetPersonQualities();
            return View(missionDraft);
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
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            await _missionDraftService.DeleteMissionDraft(id);
            return RedirectToAction("Index");
        }

        // GET: Missions/Edit/5

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

            var missionDraft = (await _missionDraftService.GetMissionDraft(id)).ConvertToUi();
            if (missionDraft == null)
            {
                return HttpNotFound();
            }

            var types = await _personQualityService.GetPersonQualities();
            ViewBag.PersonQualities = types;

            return View(missionDraft);
        }

        // POST: Missions/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.

        /// <summary>
        ///     The edit.
        /// </summary>
        /// <param name="missionDraft">
        ///     The mission draft.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(MissionDraftUi missionDraft)
        {
            if (missionDraft != null && ModelState.IsValid)
            {
                await _missionDraftService.UpdateMissionDraft(missionDraft.ConvertToDomain());
                return RedirectToAction("Index");
            }

            var personQualities = await _personQualityService.GetPersonQualities();
            ViewBag.PersonQualities = personQualities;
            return View(missionDraft);
        }

        #endregion

        // GET: Missions/Delete/5
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
                _missionDraftService.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}