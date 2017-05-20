namespace RadrugaCloud.Models.Attributes
{
    using System;
    using System.Reflection;
    using System.Web.Mvc;

    /// <summary>
    /// Class MultipleButtonAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AllowMultipleInputButtonsAttribute : ActionNameSelectorAttribute
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the argument.
        /// </summary>
        /// <value>The argument.</value>
        public string Argument { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines whether the action name is valid in the specified controller context.
        /// </summary>
        /// <param name="controllerContext">
        /// The controller context.
        /// </param>
        /// <param name="actionName">
        /// The name of the action.
        /// </param>
        /// <param name="methodInfo">
        /// Information about the action method.
        /// </param>
        /// <returns>
        /// true if the action name is valid in the specified controller context; otherwise, false.
        /// </returns>
        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            bool isValidName = false;
            string keyValue = string.Format("{0}:{1}", Name, Argument);
            var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);
            if (value != null)
            {
                controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;
                isValidName = true;
            }

            return isValidName;
        }

        #endregion
    }
}