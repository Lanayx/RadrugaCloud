namespace Core.Tools
{
    /// <summary>
    /// Class CommonPlaceHelper
    /// </summary>
    public static class CommonPlaceHelper
    {
        /// <summary>
        /// Decreases the full name.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <returns>System.String.</returns>
        public static string DecreaseFullName(this string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return string.Empty;
            }

            var shortName = fullName.Trim().Replace(" ", string.Empty).ToLower();
            return shortName;
        }
    }
}