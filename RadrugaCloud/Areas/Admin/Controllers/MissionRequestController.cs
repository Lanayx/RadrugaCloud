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

    /// <summary>
    /// Class MissionRequestController
    /// </summary>
    [Authorize]
    public class MissionRequestController : Controller
    {
        private readonly MissionRequestService _missionRequestService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MissionRequestController"/> class.
        /// </summary>
        /// <param name="missionRequestService">The missionRequest service.</param>
        public MissionRequestController(MissionRequestService missionRequestService)
        {
            _missionRequestService = missionRequestService;
        }
        
        // GET: Admin/MissionRequests

        /// <summary>
        /// Indexes the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>Task{ActionResult}.</returns>
        public async Task<ActionResult> Index(int? page)
        {
            var pageNumber = page ?? 0;
            const int MissionRequestsOnPage = 15;
            var selectOptions = new QueryOptions<MissionRequest>
            {
                Skip = pageNumber * MissionRequestsOnPage,
                Top = MissionRequestsOnPage + 1,
/*                Select = missionRequest =>
                new
                {
                    missionRequest.Id,
                    missionRequest.User.NickName,
                    missionRequest.Mission.Name,
                    missionRequest.LastUpdateDate
                },*/
                Expand = new List<string> { "Mission", "User" },
                Filter = missionRequest => missionRequest.Status == MissionRequestStatus.NotChecked
            };

            Expression<Func<MissionRequest, DateTime>> z = x => x.LastUpdateDate;
            selectOptions.OrderBy = new List<SortDescription> { new SortDescription(z, SortDirection.Ascending) };
            
            var missionRequests = await _missionRequestService.GetRequests(selectOptions);
            IEnumerable<MissionRequest> model;
            if (missionRequests.Count <= MissionRequestsOnPage)
            {
                ViewBag.ShowNext = false;
                model = missionRequests;
            }
            else
            {
                ViewBag.ShowNext = true;
                model = missionRequests.Take(MissionRequestsOnPage);
            }

            ViewBag.ShowPrevious = pageNumber != 0;
            ViewBag.CurrentPage = pageNumber;
            return View(model);
        }

        // GET: Admin/MissionRequests/Details/5

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

            MissionRequest missionRequest = await _missionRequestService.GetRequest(id);
            if (missionRequest == null)
            {
                return HttpNotFound();
            }

            return View(missionRequest);
        }

        /// <summary>
        /// Approves the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="starsCount">The stars count.</param>
        /// <returns>
        /// Task{ActionResult}.
        /// </returns>
        [System.Web.Http.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Approve(string id, int starsCount)
        {
            if (starsCount > 0 && starsCount <= 3)
            {
                await _missionRequestService.ApproveRequest(id, starsCount);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Declines the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="reason">The reason.</param>
        /// <returns>Task{ActionResult}.</returns>
        [System.Web.Http.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Decline(string id, string reason)
        {
            await _missionRequestService.DeclineRequest(id, reason);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _missionRequestService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
