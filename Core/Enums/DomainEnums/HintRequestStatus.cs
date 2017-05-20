namespace Core.Enums
{
    /// <summary>
    /// Hint hint request status
    /// </summary>
    public enum HintRequestStatus
    {
        /// <summary>
        /// The success
        /// </summary>        
        Success = 1,
       
        /// <summary>
        /// The user don't have that mission in active status
        /// </summary>        
        UserDontHaveThatMissionInActiveStatus = 2,

        /// <summary>
        /// The hint not found
        /// </summary>        
        HintNotFound = 3,

        /// <summary>
        /// The user don't have coins
        /// </summary>        
        UserDontHaveCoins = 4,
        
        /// <summary>
        /// Common place not exist
        /// </summary>        
        CommonPlaceNotExist = 5
    }
}
