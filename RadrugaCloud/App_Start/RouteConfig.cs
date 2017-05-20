namespace RadrugaCloud
{
    using System.Web.Mvc;
    using System.Web.Routing;

    using Controllers;

    /// <summary>
    /// The route config.
    /// </summary>
    public class RouteConfig
    {
        #region Public Methods and Operators

        /// <summary>
        /// The register routes.
        /// </summary>
        /// <param name="routes">
        /// The routes.
        /// </param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var route = routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "MissionDraft", action = "Index", id = UrlParameter.Optional },
                new[] { typeof(MissionDraftController).Namespace });

            route.DataTokens["UseNamespaceFallback"] = false;
        }

        #endregion
    }
}