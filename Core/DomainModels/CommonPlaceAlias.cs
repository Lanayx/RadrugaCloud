namespace Core.DomainModels
{
    using Core.Tools;

    /// <summary>
    ///     Class CommonPlaceAliace
    /// </summary>
    public class CommonPlaceAlias
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonPlaceAlias"/> class.
        /// </summary>
        public CommonPlaceAlias()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommonPlaceAlias" /> class.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        public CommonPlaceAlias(string fullName)
        {
            FullName = fullName;
            ShortName = fullName.DecreaseFullName();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the full name.
        /// </summary>
        /// <value>The full name.</value>
        public string FullName { get; set; }

        /// <summary>
        ///     Gets or sets the short name.
        /// </summary>
        /// <value>The short name.</value>
        public string ShortName { get; set; }

        #endregion
    }
}