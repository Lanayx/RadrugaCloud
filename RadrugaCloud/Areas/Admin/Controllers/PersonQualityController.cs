namespace RadrugaCloud.Areas.Admin.Controllers
{
    using System.Diagnostics;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Core.DomainModels;
    using Services.DomainServices;

    /// <summary>
    /// The person quality controller.
    /// </summary>
    [Authorize]
    public class PersonQualityController : RadrugaCloud.Controllers.PersonQualityController
    {
        #region Fields

        /// <summary>
        /// The _person quality service.
        /// </summary>
        private readonly PersonQualityService _personQualityService;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonQualityController" /> class.
        /// </summary>
        /// <param name="personQualityService">The person quality service.</param>
        public PersonQualityController(PersonQualityService personQualityService)
            : base(personQualityService)
        {
            _personQualityService = personQualityService;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        ///     Creates the specified person quality.
        /// </summary>
        /// <param name="personQuality">
        ///     The person quality.
        /// </param>
        /// <returns>
        ///     Task{ActionResult}.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PersonQuality personQuality)
        {
            FixModelAndModelState(personQuality);
            if (personQuality != null && ModelState.IsValid)
            {
                await _personQualityService.AddNewPersonQuality(personQuality);
                return RedirectToAction("Index");
            }

            return View(personQuality);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var personQuality = await _personQualityService.GetPersonQuality(id);
            if (personQuality == null)
            {
                return HttpNotFound();
            }

            return View(personQuality);
        }

        // POST: Missions/Delete/5

        /// <summary>
        /// The delete confirmed.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            await _personQualityService.DeletePersonQuality(id);
            return RedirectToAction("Index");
        }

        // GET: Missions/Edit/5

        /// <summary>
        /// The edit.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var personQuality = await _personQualityService.GetPersonQuality(id);
            if (personQuality == null)
            {
                return HttpNotFound();
            }

            return View(personQuality);
        }

        // POST: Missions/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.

        /// <summary>
        /// The edit.
        /// </summary>
        /// <param name="personQuality">
        /// The person quality.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PersonQuality personQuality)
        {
            if (personQuality != null && ModelState.IsValid)
            {
                await _personQualityService.UpdatePersonQuality(personQuality);
                return RedirectToAction("Index");
            }

            return View(personQuality);
        }

        #endregion

        // GET: Missions/Delete/5
        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _personQualityService.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///     Fixes the state of the model and model.
        /// </summary>
        /// <param name="personQuality">
        ///     The person quality.
        /// </param>
        private void FixModelAndModelState(PersonQuality personQuality)
        {
            Trace.TraceInformation("{0}", personQuality);
            ModelState.Remove("Id");
        }

        #endregion
    }
}