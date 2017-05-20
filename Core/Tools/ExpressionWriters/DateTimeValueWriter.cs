// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeValueWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DateTimeValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Core.Tools.ExpressionWriters
{
    using System;
    using System.Xml;

    internal class DateTimeValueWriter : ValueWriterBase<DateTime>
    {
        #region Public Methods and Operators

        public override string Write(object value)
        {
            var dateTimeValue = (DateTime)value;

#if !NETFX_CORE
            return string.Format("datetime'{0}'", XmlConvert.ToString(dateTimeValue, XmlDateTimeSerializationMode.Utc));
#else
			return string.Format("datetime'{0}'", XmlConvert.ToString(dateTimeValue.ToUniversalTime()));
#endif
        }

        #endregion
    }
}