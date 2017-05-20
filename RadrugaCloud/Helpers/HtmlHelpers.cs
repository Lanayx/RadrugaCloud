namespace RadrugaCloud.Helpers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    /// <summary>
    ///     Class HtmlHelpers
    /// </summary>
    public static class HtmlHelpers
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the display attribute from.
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns>System.String.</returns>
        public static string GetDisplayAttributeFrom(this Enum enumValue, Type enumType)
        {
            return enumType.GetMember(enumValue.ToString()).First().GetCustomAttribute<DisplayAttribute>().Name;
        }

        public static IHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> html, Expression<Func<TModel, TEnum>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            var enumType = Nullable.GetUnderlyingType(metadata.ModelType) ?? metadata.ModelType;           
            var enumValues = Enum.GetValues(enumType).Cast<Enum>();

            var items = enumValues.Select( t => new SelectListItem
                                                {
                                                    Text = t.GetDisplayAttributeFrom(enumType),
                                                    Value = t.ToString(),
                                                    Selected = t.Equals(metadata.Model)
                                                });

            return html.DropDownListFor(expression, items, string.Empty, null);
        }

        #endregion
    }
}