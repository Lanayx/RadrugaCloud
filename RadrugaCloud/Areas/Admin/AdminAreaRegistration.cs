namespace RadrugaCloud.Areas.Admin
{
    using System.Web.Mvc;

    using RadrugaCloud.Areas.Admin.Controllers;

    /// <summary>
    /// Class AdminAreaRegistration
    /// </summary>
    public class AdminAreaRegistration : AreaRegistration
    {
        #region Public Properties

        /// <summary>
        /// Gets the name of the area to register.
        /// </summary>
        /// <value>The name of the area.</value>
        /// <returns>The name of the area to register.</returns>
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Registers an area in an ASP.NET MVC application using the specified area's context information.
        /// </summary>
        /// <param name="context">Encapsulates the information that is required in order to register the area.</param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default", 
                "Admin/{controller}/{action}/{id}", 
                new { controller = "MissionDraft", action = "Index", id = UrlParameter.Optional }, 
                new[] { typeof(MissionDraftController).Namespace });
        }

        #endregion
    }
}