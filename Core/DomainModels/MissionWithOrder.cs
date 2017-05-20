namespace Core.DomainModels
{
    /// <summary>
    /// Class MissionWithOrder
    /// </summary>
    public class MissionWithOrder
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public byte Order { get; set; }


        /// <summary>
        /// Gets or sets the mission.
        /// </summary>
        /// <value>The mission.</value>
        public Mission Mission { get; set; }
        #endregion
    }
}