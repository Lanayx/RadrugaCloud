namespace Core.Tools
{
    using System;
    using System.Reflection;

    using Core.Attributes;

    /// <summary>
    /// Class EnumHelper
    /// </summary>
    public static class EnumHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the value of <see cref="DescriptionAttribute"/>.
        /// </summary>
        /// <param name="en">
        /// Enum value.
        /// </param>
        /// <returns>
        /// Description
        /// </returns>
        public static string GetDescription(this Enum en)
        {
            return GetValueFromExtensionAttribute<DescriptionAttribute>(en);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generic method for getting value for one of extention attribute.
        /// </summary>
        /// <typeparam name="T">
        /// Type nested from <see cref="BaseEnumExtenderAttribute"/>
        /// </typeparam>
        /// <param name="en">
        /// Enum value.
        /// </param>
        /// <returns>
        /// Attributes value.
        /// </returns>
        private static string GetValueFromExtensionAttribute<T>(Enum en) where T : BaseEnumExtenderAttribute
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(T), false);

                if (attrs.Length > 0)
                {
                    return ((T)attrs[0]).Text;
                }
            }

            return en.ToString();
        }

        #endregion
    }
}