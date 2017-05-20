namespace Core.CommonModels.Results
{
    using Core.Enums;

    /// <summary>
    /// The kindAction result.
    /// </summary>
    public class KindActionResult : OperationResult
    {
        // For errors
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IdResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public KindActionResult(OperationResultStatus status, string description)
            : base(status, description)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdResult"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public KindActionResult(int points, int coins, byte kindScale)
            : base(OperationResultStatus.Success)
        {
            Points = points;
            Coins = coins;
            KindScale = kindScale;
        }

        #endregion

        #region Public Properties

       
        public int Points { get; private set; }
        public int Coins { get; private set; }
        public byte KindScale { get; private set; }

        #endregion
    }
}