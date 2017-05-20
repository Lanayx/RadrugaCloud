namespace Core.CommonModels.Results
{
    using Core.Enums;

    /// <summary>
    /// The url result.
    /// </summary>
    public class UrlResult : OperationResult
    {
        // For errors
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public UrlResult(OperationResultStatus status, string description)
            : base(status, description)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlResult"/> class.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        public UrlResult(string url)
            : base(OperationResultStatus.Success)
        {
            Url = url;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the url
        /// </summary>
        public string Url { get; private set; }

        #endregion
    }
}