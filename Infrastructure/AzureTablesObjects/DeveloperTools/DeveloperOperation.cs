namespace Infrastructure.AzureTablesObjects.DeveloperTools
{
    /// <summary>
    /// Enum DeveloperOperation
    /// </summary>
    public enum DeveloperOperation
    {
        /// <summary>
        /// The unspecified
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// The delete all tables
        /// </summary>
        DeleteAllTables = 1, 

        /// <summary>
        /// The fill tables with data
        /// </summary>
        FillTablesWithData = 2,

        /// <summary>
        /// The fix external images
        /// </summary>
        FixExternalImages = 3,

        /// <summary>
        /// The clean temp images storage
        /// </summary>
        CleanTempImagesStorage = 4,

        /// <summary>
        /// Update user locations before update
        /// </summary>
        UpdateUserLocations = 5,

        /// <summary>
        /// Update user kind actions count before update
        /// </summary>
        UpdateKindActionsCount = 6,

        /// <summary>
        /// Fixes user light quality
        /// </summary>
        FixLightQuality = 7
    }
}