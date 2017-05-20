namespace Core.Tools.CopyHelper
{
    using System.Reflection;

    /// <summary>
    /// The deep copy utility.
    /// </summary>
    public static class DeepCopyUtility
    {
        #region Public Methods and Operators

        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <param name="source">The object instance to copy.</param>
        /// <param name="target">The object instance to copy to.</param>
        /// <param name="copyNulls">if set to <c>true</c> [copy nulls].</param>
        /// <returns>
        /// System.Object.
        /// </returns>
        public static object CopyTo(this object source, object target, bool copyNulls = true)
        {
            foreach (PropertyInfo sourceProperty in source.GetType().GetProperties())
            {
                foreach (PropertyInfo targetProperty in target.GetType().GetProperties())
                {
                    if (targetProperty.Name != sourceProperty.Name || targetProperty.PropertyType != sourceProperty.PropertyType)
                    {
                        continue;
                    }

                    // for merge operation we won't copy null values
                    if (!copyNulls && sourceProperty.GetValue(source) == null)
                    {
                        continue;
                    }

                    MethodInfo setMethod = targetProperty.GetSetMethod();

                    if (setMethod != null)
                    {
                        setMethod.Invoke(target, new[] { sourceProperty.GetGetMethod().Invoke(source, null) });
                    }
                }
            }

            return target;
        }

        #endregion
    }
}