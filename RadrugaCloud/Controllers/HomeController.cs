namespace RadrugaCloud.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Web.Mvc;

    /// <summary>
    /// The home controller.
    /// </summary>
    public class HomeController : Controller
    {
        #region Public Methods and Operators

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error()
        {
            throw new Exception("MMee");
        }

        public ActionResult TraceError()
        {
            Trace.TraceError("this it test error trace");
            return new EmptyResult();
        }

        #endregion
    }
}