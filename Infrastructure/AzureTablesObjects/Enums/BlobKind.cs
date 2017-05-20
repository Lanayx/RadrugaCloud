namespace Infrastructure.AzureTablesObjects
{
    /// <summary>
    /// Enum BlobKind
    /// </summary>
    public enum BlobKind
    {
        /// <summary>
        /// The empty
        /// </summary>
        Empty = 0,

        /// <summary>
        /// The azure
        /// </summary>
        Azure = 1,

        /// <summary>
        /// The emulator
        /// </summary>
        Emulator = 2,

        /// <summary>
        /// The external
        /// </summary>
        External = 3
    }
}