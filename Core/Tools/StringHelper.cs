namespace Core.Tools
{
    /// <summary>
    /// Class StringHelper
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        ///     To the camel case.
        /// </summary>
        /// <param name="initial">The initial.</param>
        /// <returns>System.String.</returns>
        public static string ToCamelCase(this string initial)
        {
            if (string.IsNullOrEmpty(initial))
            {
                return string.Empty;
            }

            if (initial.Length == 1)
            {
                return initial.ToLower();
            }

            return char.ToLower(initial[0]) + initial.Substring(1);
        }

        /// <summary>
        /// Gets the caller identifier.
        /// </summary>
        /// <param name="classInstance">The class instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public static string GetCallerID(this object classInstance, string methodName)
        {
            var className = classInstance.GetType().FullName;
            return string.Concat(className, ".", methodName);
        }
    }
}