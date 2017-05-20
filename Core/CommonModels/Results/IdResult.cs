namespace Core.CommonModels.Results
{
    using Core.Enums;

    /// <summary>
    /// The id result.
    /// </summary>
    public class IdResult : OperationResult
    {
        // For errors
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IdResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public IdResult(OperationResultStatus status, string description)
            : base(status, description)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdResult"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public IdResult(string id)
            : base(OperationResultStatus.Success)
        {
            Id = id;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the id.
        /// </summary>
        public string Id { get; private set; }

        #endregion
    }
}