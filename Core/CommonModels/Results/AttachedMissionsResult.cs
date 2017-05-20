namespace Core.CommonModels.Results
{
    using System.Collections.Generic;

    using Core.Enums;
    using Core.NonDomainModels;

    /// <summary>
    /// Class AttachedMissionsResult
    /// </summary>
    public class AttachedMissionsResult : OperationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public AttachedMissionsResult(OperationResultStatus status)
            : base(status)
        {
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
        public AttachedMissionsResult(OperationResultStatus status, string description)
            : base(status, description)
        {
        }

        /// <summary>
        /// Gets or sets the mission sets.
        /// </summary>
        /// <value>The mission sets.</value>
        public IEnumerable<UserMissionSet> MissionSets { get; set; }
    }
}