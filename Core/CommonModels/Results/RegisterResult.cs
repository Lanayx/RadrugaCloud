namespace Core.CommonModels.Results
{
    using Core.Enums;

    /// <summary>
    /// Class RegisterResult
    /// </summary>
    public class RegisterResult : IdResult
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterResult" /> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="description">The description.</param>
        public RegisterResult(OperationResultStatus status, string description)
            : base(status, description)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterResult" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public RegisterResult(string id)
            : base(id)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the O auth token.
        /// </summary>
        /// <value>The O auth token.</value>
        public string OAuthToken { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Froms the id result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>RegisterResult.</returns>
        public static RegisterResult FromIdResult(IdResult result)
        {
            var regResult = new RegisterResult(result.Id) { Status = result.Status, Description = result.Description };
            return regResult;
        }

        #endregion
    }
}