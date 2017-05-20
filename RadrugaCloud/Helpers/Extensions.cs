namespace RadrugaCloud.Helpers
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;
    using System.Web.Http;

    internal static class Extensions
    {
        #region Methods

        /// <summary>
        ///     Validates the object.
        /// </summary>
        /// <param name="validatedObject">The validated object.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        internal static bool ValidateObject(this object validatedObject)
        {
            var context = new ValidationContext(validatedObject, null, null);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(validatedObject, context, results, true);
        }

        #endregion

        internal static string GetCurrentUserId(this ApiController apiController)
        {
            var identity = (ClaimsIdentity)apiController.User.Identity;
            return identity.FindFirst(ClaimTypes.Sid).Value;
        }
    }
}