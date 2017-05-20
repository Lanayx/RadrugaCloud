namespace RadrugaCloud.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Mvc;

    using Core.CommonModels.Query;
    using Core.DomainModels;

    using Services.DomainServices;

    /// <summary>
    /// The person quality controller.
    /// </summary>
    public class PersonQualityController : Controller
    {
        #region Fields

        /// <summary>
        /// The _person quality service.
        /// </summary>
        private readonly PersonQualityService _personQualityService;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonQualityController"/> class.
        /// </summary>
        /// <param name="personQualityService">
        /// The person quality service.
        /// </param>
        public PersonQualityController(PersonQualityService personQualityService)
        {
            _personQualityService = personQualityService;
        }

        #endregion

        #region Public Methods and Operators
      
        // GET: Missions/Details/5

        /// <summary>
        ///     The details.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<ActionResult> Details(string id)
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

        /// <summary>
        ///     Indexes the specified page.
        /// </summary>
        /// <param name="page">
        ///     The page.
        /// </param>
        /// <returns>
        ///     Task{ActionResult}.
        /// </returns>
        public async Task<ActionResult> Index([FromUri] int? page)
        {
            var pageNumber = page ?? 0;
            const int ItemsOnPage = 15;
            var selectOptions = new QueryOptions<PersonQuality>
            {
                /*Filter = p => p.Author == "Defor",*/
                Skip = pageNumber * ItemsOnPage,
                Top = ItemsOnPage + 1,
                Select = draft =>
                new
                {
                    draft.Id,
                    draft.Name,
                    draft.Description
                }
            };
            var types = await _personQualityService.GetPersonQualities(selectOptions);
            IEnumerable<PersonQuality> model;
            if (types.Count <= ItemsOnPage)
            {
                ViewBag.ShowNext = false;
                model = types;
            }
            else
            {
                ViewBag.ShowNext = true;
                model = types.Take(ItemsOnPage);
            }

            ViewBag.ShowPrevious = pageNumber != 0;
            ViewBag.CurrentPage = pageNumber;
            return View(model);
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

        #endregion
    }
}