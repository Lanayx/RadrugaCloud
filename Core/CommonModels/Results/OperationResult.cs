namespace Core.CommonModels.Results
{
    using Core.Enums;

    /// <summary>
    /// The operation result.
    /// </summary>
    public class OperationResult
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public OperationResult(OperationResultStatus status)
        {
            Status = status;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public OperationResult(OperationResultStatus status, string description)
        {
            Status = status;
            Description = description;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the not found.
        /// </summary>
        public static OperationResult NotFound
        {
            get
            {
                return new OperationResult(OperationResultStatus.Error, "Not Found");
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public OperationResultStatus Status { get; set; }

        #endregion
    }
}