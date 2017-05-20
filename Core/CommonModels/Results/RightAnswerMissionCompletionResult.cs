namespace Core.CommonModels.Results
{
    using System.Collections.Generic;

    using Core.Enums;

    /// <summary>
    /// Class RightAnswerMissionCompletionResult
    /// </summary>
    public class RightAnswerMissionCompletionResult : MissionCompletionResult
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionCompletionResult" /> class.
        /// </summary>
        public RightAnswerMissionCompletionResult()
            : base(OperationResultStatus.Success)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissionCompletionResult" /> class.
        /// </summary>
        /// <param name="status">The status.</param>
        public RightAnswerMissionCompletionResult(OperationResultStatus status)
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
        public RightAnswerMissionCompletionResult(OperationResultStatus status, string description)
            : base(status, description)
        {
        }

        /// <summary>
        /// Gets or sets the answer statuses.
        /// </summary>
        /// <value>The answer statuses.</value>
        public List<string> AnswerStatuses { get; set; }

        /// <summary>
        /// Froms the mission completion result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>RightAnswerMissionCompletionResult.</returns>
        public static RightAnswerMissionCompletionResult FromMissionCompletionResult(MissionCompletionResult result)
        {
            return new RightAnswerMissionCompletionResult
                       {
                           Points = result.Points,
                           Status = result.Status,
                           Description = result.Description,
                           MissionCompletionStatus = result.MissionCompletionStatus,
                           StarsCount = result.StarsCount,
                           TryCount = result.TryCount
                       };
        }
    }
}