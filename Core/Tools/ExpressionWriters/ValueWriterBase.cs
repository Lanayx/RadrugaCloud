// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueWriterBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ValueWriterBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Core.Tools.ExpressionWriters
{
    using System;

    internal abstract class ValueWriterBase<T> : IValueWriter
    {
        #region Public Methods and Operators

        public bool Handles(Type type)
        {
#if !NETFX_CORE
            return typeof(T) == type;
#else
			return typeof(T).GetTypeInfo().Equals(type.GetTypeInfo());
#endif
        }

        public abstract string Write(object value);

        #endregion
    }
}