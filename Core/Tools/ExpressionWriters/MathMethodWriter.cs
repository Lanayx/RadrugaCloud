// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MathMethodWriter.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MathMethodWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Core.Tools.ExpressionWriters
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;

    /*[ContractClass(typeof(MathMethodWriterContracts))]*/
    internal abstract class MathMethodWriter : IMethodCallWriter
    {
        #region Properties

        protected abstract string MethodName { get; }

        #endregion

        #region Public Methods and Operators

        public abstract bool CanHandle(MethodCallExpression expression);

        public string Handle(MethodCallExpression expression, Func<Expression, string> expressionWriter)
        {
            Contract.Assume(expression.Arguments.Count > 0);

            var mathArgument = expression.Arguments[0];

            Contract.Assume(mathArgument != null);

            return string.Format("{0}({1})", MethodName, expressionWriter(mathArgument));
        }

        #endregion
    }

    /*[ContractClassFor(typeof(MathMethodWriter))]
    internal abstract class MathMethodWriterContracts : MathMethodWriter
    {
        #region Properties

        protected override string MethodName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
                throw new NotImplementedException();
            }
        }

        #endregion
    }*/
}