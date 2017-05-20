namespace Core.Enums
{
    /// <summary>
    /// The mission completion status.
    /// </summary>
    public enum MissionCompletionStatus : byte
    {
        /// <summary>
        /// MissionCompleted
        /// </summary>
        Success = 0,

        /// <summary>
        /// MissionFailed
        /// </summary>
        Fail = 1,

        /// <summary>
        /// Waiting for admin approval
        /// </summary>
        Waiting = 2,

        /// <summary>
        /// The intermediate fail
        /// </summary>
        IntermediateFail = 3
    }
}