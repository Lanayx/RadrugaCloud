namespace Core.Enums
{
    /// <summary>
    /// The mission completion status.
    /// </summary>
    public enum MissionDisplayStatus : byte
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
        /// Available for execution
        /// </summary>
        Available = 3,

        /// <summary>
        /// Not available for execution yet
        /// </summary>
        NotAvailable = 4
    }
}