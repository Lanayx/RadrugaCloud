namespace Core.Enums
{
    /// <summary>
    /// The operation result status.
    /// </summary>
    public enum OperationResultStatus : byte
    {
        /// <summary>
        /// The success.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The error.
        /// </summary>
        Error = 1,

        /// <summary>
        /// The warning.
        /// </summary>
        Warning = 2
    }
}