namespace Core.Enums
{
    /// <summary>
    /// The mission request status.
    /// </summary>
    public enum MissionRequestStatus : byte
    {
        /// <summary>
        /// The not checked.
        /// </summary>
        NotChecked = 0,

        /// <summary>
        /// The approved.
        /// </summary>
        Approved = 1,

        /// <summary>
        /// The declined.
        /// </summary>
        Declined = 2,

        /// <summary>
        /// The autoapproval.
        /// </summary>
        AutoApproval = 3,
    }
}