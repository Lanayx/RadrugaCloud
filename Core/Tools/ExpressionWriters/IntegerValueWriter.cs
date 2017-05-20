// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerValueWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IntegerValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Core.Tools.ExpressionWriters
{
    internal abstract class IntegerValueWriter<T> : ValueWriterBase<T>
    {
        #region Public Methods and Operators

        public override string Write(object value)
        {
            return value.ToString();
        }

        #endregion
    }
}