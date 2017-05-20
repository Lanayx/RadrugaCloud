// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumValueWriter.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EnumValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Core.Tools.ExpressionWriters
{
    using System;

    internal class EnumValueWriter : IValueWriter
    {
        #region Public Methods and Operators

        public bool Handles(Type type)
        {
#if !NETFX_CORE
            return type.IsEnum;
#else
			return type.GetTypeInfo().IsEnum;
#endif
        }

        public string Write(object value)
        {
            var enumType = value.GetType();

            return string.Format("{0}'{1}'", enumType.FullName, value);
        }

        #endregion
    }
}