namespace Core.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    ///     Class ExpressionHelper
    /// </summary>
    public static class ExpressionHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the select properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selectExpression">The select expression.</param>
        /// <returns>System.String[][].</returns>
        public static List<string> GetAnonymousProperties<T>(this Expression<Func<T, object>> selectExpression)
        {
            if (selectExpression == null)
            {
                return new List<string>();
            }

            var body = selectExpression.Body;
            var bodyAsMemberExpression = body as MemberExpression;
            var bodyAsNewExpression = body as NewExpression;
            if (bodyAsMemberExpression != null)
            {
                return new List<string> { bodyAsMemberExpression.Member.Name };
            }

            if (bodyAsNewExpression == null)
            {
                return null;
            }

            var returnList = new List<string>();
            var members = bodyAsNewExpression.Members;
            foreach (var memberInfo in members)
            {
                returnList.Add(memberInfo.Name);
            }

            return returnList;
        }

        /// <summary>
        /// Works as replace for contains operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="member">The member.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> In<T, R>(this Expression<Func<T, R>> member, IList<R> values)
        {
            var prop = (MemberExpression)member.Body;
            if (!values.Any())
                return _ => false;
            var body = values.Select(v => Expression.Equal(prop, Expression.Constant(v))).Aggregate(Expression.OrElse);
            return Expression.Lambda<Func<T, bool>>(body, member.Parameters[0]);
        }

        // Helps with type inference
        public static Expression<Func<A, B>> Expr<A, B>(Expression<Func<A, B>> exp)
        {
            return exp;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <typeparam name="TSource">The type of the T source.</typeparam>
        /// <typeparam name="TProperty">The type of the T property.</typeparam>
        /// <param name="propertyLambda">The property lambda.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentException">If property lambda is invalid</exception>
        public static string GetPropertyName<TSource, TProperty>(
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
            }

            if (propInfo.ReflectedType != null
                && (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType)))
            {
                throw new ArgumentException(
                    $"Expresion '{propertyLambda}' refers to a property that is not from type {type}.");
            }

            return propInfo.Name;
        }

        /// <summary>
        ///     Gets the rest query.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public static string GetRestQuery(this Expression expression)
        {
            var writer = new ExpressionWriter();
            return writer.Write(expression);
        }

        #endregion
    }
}