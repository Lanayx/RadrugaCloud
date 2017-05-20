namespace Core.Tools.CopyHelper
{
    using System;

    /// <summary>
    ///     Class ArrayTraverse
    /// </summary>
    internal class ArrayTraverse
    {
        #region Fields

        /// <summary>
        ///     The max lengths.
        /// </summary>
        private readonly int[] _maxLengths;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ArrayTraverse" /> class.
        /// </summary>
        /// <param name="array">
        ///     The array.
        /// </param>
        public ArrayTraverse(Array array)
        {
            _maxLengths = new int[array.Rank];
            for (int i = 0; i < array.Rank; ++i)
            {
                _maxLengths[i] = array.GetLength(i) - 1;
            }

            Position = new int[array.Rank];
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public int[] Position { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The step.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Step()
        {
            for (int i = 0; i < Position.Length; ++i)
            {
                if (Position[i] < _maxLengths[i])
                {
                    Position[i]++;
                    for (int j = 0; j < i; j++)
                    {
                        Position[j] = 0;
                    }

                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}