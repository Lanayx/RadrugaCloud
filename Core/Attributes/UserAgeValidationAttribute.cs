namespace Core.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The user age validation.
    /// </summary>
    public class UserAgeValidationAttribute : ValidationAttribute
    {
        #region Public Methods and Operators

        /// <summary>
        /// The is valid.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool IsValid(object value)
        {
            DateTime dt;
            bool parsed = DateTime.TryParse((string)value, out dt);
            if (!parsed)
            {
                return false;
            }

            if (dt < DateTime.UtcNow.AddYears(-100) || dt > DateTime.UtcNow.AddYears(-2))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}