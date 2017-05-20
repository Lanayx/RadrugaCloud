namespace Core.CommonModels.Results
{
    using Core.Enums;

    /// <summary>
    /// The add result.
    /// </summary>
    public class ColorResult : OperationResult
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
        public ColorResult(OperationResultStatus status, string description)
            : base(status, description)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorResult" /> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public ColorResult(string color)
            : base(OperationResultStatus.Success)
        {
            Color = color;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the id.
        /// </summary>
        public string Color { get; private set; }

        #endregion
    }
}