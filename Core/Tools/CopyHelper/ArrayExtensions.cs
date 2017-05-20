namespace Core.Tools.CopyHelper
{
    using System;

    /// <summary>
    /// The array extensions.
    /// </summary>
    public static class ArrayExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The for each.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.LongLength == 0)
            {
                return;
            }

            var walker = new ArrayTraverse(array);
            do
            {
                action(array, walker.Position);
            }
            while (walker.Step());
        }

        #endregion
    }
}