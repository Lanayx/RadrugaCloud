// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringValueWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the StringValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Core.Tools.ExpressionWriters
{
    internal class StringValueWriter : ValueWriterBase<string>
    {
        #region Public Methods and Operators

        public override string Write(object value)
        {
            /*if (!string.IsNullOrEmpty(value.ToString()))
            {
                return string.Format("'{0}'", value);
            }*/

            var stringValue = value.ToString();
            string returnedValue = stringValue.Contains("'") ? stringValue.Replace("'", "''") : stringValue;

            return string.Format("'{0}'", returnedValue);
        }

        #endregion
    }
}