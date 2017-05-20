namespace Core.CommonModels.Results
{
    using Core.Enums;

    /// <summary>
    ///     The add result.
    /// </summary>
    public class MissionCompletionResult : OperationResult
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionCompletionResult" /> class.
        /// </summary>
        public MissionCompletionResult()
            : base(OperationResultStatus.Success)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionCompletionResult" /> class.
        /// </summary>
        /// <param name="status">The status.</param>
        public MissionCompletionResult(OperationResultStatus status)
            : base(status)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IdResult" /> class.
        /// </summary>
        /// <param name="status">
        ///     The status.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        public MissionCompletionResult(OperationResultStatus status, string description)
            : base(status, description)
        {
        }

        /// <summary>
        ///     Gets the id.
        /// </summary>
        public MissionCompletionStatus MissionCompletionStatus { get; set; }

        /// <summary>
        ///     Gets or sets the points.(on success)
        /// </summary>
        /// <value>
        ///     The points.
        /// </value>
        public int? Points { get; set; }

        /// <summary>
        ///     Gets or sets the stars count (on success)
        /// </summary>
        /// <value>
        ///     The stars count.
        /// </value>
        public int? StarsCount { get; set; }

        /// <summary>
        /// Gets or sets the current try count.
        /// </summary>
        /// <value>
        /// The try count.
        /// </value>
        public int? TryCount { get; set; }

        /// <summary>
        ///     Froms the id result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>MissionCompletionResult.</returns>
        public static MissionCompletionResult FromOperationResult(OperationResult result)
        {
            return new MissionCompletionResult(result.Status, result.Description);
        }
    }
}