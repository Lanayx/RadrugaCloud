namespace Infrastructure.InfrastructureTools
{
    /// <summary>
    /// Class StatusCodeExtentions
    /// </summary>
    public static class StatusCodeExtentions
    {
        /// <summary>
        /// Ensures the success status code.
        /// </summary>
        /// <param name="responseStatusCode">The response status code.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public static bool EnsureSuccessStatusCode(this int responseStatusCode)
        {
            return responseStatusCode >= 200 && responseStatusCode < 300;
        }
    }
}