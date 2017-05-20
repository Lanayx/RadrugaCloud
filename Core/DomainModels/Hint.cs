namespace Core.DomainModels
{
    using Core.Enums;

    /// <summary>
    /// A class for hints for missions
    /// </summary>
    public class Hint
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>       
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the text of hint.
        /// </summary>      
        public string Text { get; set; }
       
        /// <summary>
        /// Gets or sets the score.
        /// </summary>     
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the type of hint.
        /// </summary>      
        public HintType Type { get; set; }

        #endregion
    }
}
