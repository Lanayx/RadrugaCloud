namespace Core.CommonModels.Query
{
    using System.Linq.Expressions;

    using Core.Enums;

    /// <summary>
    /// Class SortDescription
    /// </summary>
    public class SortDescription
    {
        #region Fields

        /// <summary>
        ///     The _direction
        /// </summary>
        private readonly SortDirection _direction;

        /// <summary>
        ///     The _key selector
        /// </summary>
        private readonly Expression _keySelector;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SortDescription"/> class.
        /// </summary>
        /// <param name="keySelector">
        /// The function to select the sort key.
        /// </param>
        /// <param name="direction">
        /// The sort direction.
        /// </param>
        public SortDescription(Expression keySelector, SortDirection direction)
        {
            _keySelector = keySelector;
            _direction = direction;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the sort direction.
        /// </summary>
        /// <value>The direction.</value>
        public SortDirection Direction
        {
            get
            {
                return _direction;
            }
        }

        /// <summary>
        ///     Gets the key to sort by.
        /// </summary>
        /// <value>The key selector.</value>
        public Expression KeySelector
        {
            get
            {
                return _keySelector;
            }
        }

        #endregion
    }
}