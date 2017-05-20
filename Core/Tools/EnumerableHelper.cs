namespace Core.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Core.CommonModels.Query;
    using Core.Enums;

    /// <summary>
    /// Class QueryableHelper
    /// </summary>
    public static class EnumerableHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// Applies the sort.
        /// </summary>
        /// <typeparam name="T">Requested parameter</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="sortDescriptions">The sort descriptions.</param>
        /// <returns>IEnumerable{``0}.</returns>
        public static IEnumerable<T> Sort<T>(IEnumerable<T> source, List<SortDescription> sortDescriptions)
        {
            if (sortDescriptions.Any())
            {
                var isFirst = true;
                foreach (var sortDescription in sortDescriptions.Where(x => x != null))
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        source = sortDescription.Direction == SortDirection.Ascending
                                     ? source.OrderBy(sortDescription.KeySelector)
                                     : source.OrderByDescending(sortDescription.KeySelector);
                    }
                    else
                    {
                        var orderedEnumerable = source as IOrderedEnumerable<T>;

                        source = sortDescription.Direction == SortDirection.Ascending
                                     ? orderedEnumerable.ThenBy(sortDescription.KeySelector)
                                     : orderedEnumerable.ThenByDescending(sortDescription.KeySelector);
                    }
                }
            }

            return source;
        }

        /// <summary>
        /// Converts IEnumerable type to IList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input.</param>
        /// <returns>IList{``0}.</returns>
        public static IList<T> ToIList<T>(this IEnumerable<T> input)
        {
            if (input == null)
                return null;
            return input as IList<T> ?? input.ToList();
        }

        /// <summary>
        /// Checks for null and any
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public static bool AnyValues<T>(this IEnumerable<T> input)
        {
            return input != null && input.Any();
        }

        /// <summary>
        /// Anies the values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input.</param>
        /// <param name="predicate"></param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public static bool AnyValues<T>(this IEnumerable<T> input, Func<T, bool> predicate)
        {
            return input != null && input.Any(predicate);
        }

        /// <summary>
        /// Orders by.
        /// </summary>
        /// <typeparam name="T">
        /// Requested parameter
        /// </typeparam>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="keySelector">
        /// The key selector.
        /// </param>
        /// <returns>
        /// IOrderedQueryable{``0}.
        /// </returns>
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> source, Expression keySelector)
        {
            var propertyType = keySelector.GetType().GetGenericArguments()[0].GetGenericArguments()[1];
            var orderbyMethod =
                typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(x => x.Name == "OrderBy" && x.GetParameters().Length == 2);

            if (orderbyMethod == null)
            {
                return source;
            }

            orderbyMethod = orderbyMethod.MakeGenericMethod(typeof(T), propertyType);

            return (IEnumerable<T>)orderbyMethod.Invoke(null, new object[] { source.AsQueryable(), keySelector });
        }

        /// <summary>
        /// Orders by descending.
        /// </summary>
        /// <typeparam name="T">
        /// Requested parameter
        /// </typeparam>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="keySelector">
        /// The key selector.
        /// </param>
        /// <returns>
        /// IOrderedQueryable{``0}.
        /// </returns>
        public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> source, Expression keySelector)
        {
            var propertyType = keySelector.GetType().GetGenericArguments()[0].GetGenericArguments()[1];
            var orderbyMethod =
                typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 2);

            if (orderbyMethod == null)
            {
                return source;
            }

            orderbyMethod = orderbyMethod.MakeGenericMethod(typeof(T), propertyType);

            return (IEnumerable<T>)orderbyMethod.Invoke(null, new object[] { source.AsQueryable(), keySelector });
        }

        /// <summary>
        /// Thens by.
        /// </summary>
        /// <typeparam name="T">
        /// Requested parameter
        /// </typeparam>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="keySelector">
        /// The key selector.
        /// </param>
        /// <returns>
        /// IOrderedQueryable{``0}.
        /// </returns>
        public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> source, Expression keySelector)
        {
            var propertyType = keySelector.GetType().GetGenericArguments()[0].GetGenericArguments()[1];
            var orderbyMethod =
                typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(x => x.Name == "ThenBy" && x.GetParameters().Length == 2);

            if (orderbyMethod == null)
            {
                return source;
            }

            orderbyMethod = orderbyMethod.MakeGenericMethod(typeof(T), propertyType);

            return (IOrderedEnumerable<T>)orderbyMethod.Invoke(null, new object[] { source, keySelector });
        }

        /// <summary>
        /// Thens by descending.
        /// </summary>
        /// <typeparam name="T">
        /// Requested parameter
        /// </typeparam>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="keySelector">
        /// The key selector.
        /// </param>
        /// <returns>
        /// IOrderedQueryable{``0}.
        /// </returns>
        public static IOrderedEnumerable<T> ThenByDescending<T>(this IOrderedEnumerable<T> source, Expression keySelector)
        {
            var propertyType = keySelector.GetType().GetGenericArguments()[0].GetGenericArguments()[1];
            var orderbyMethod =
                typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(x => x.Name == "ThenByDescending" && x.GetParameters().Length == 2);

            if (orderbyMethod == null)
            {
                return source;
            }

            orderbyMethod = orderbyMethod.MakeGenericMethod(typeof(T), propertyType);

            return (IOrderedEnumerable<T>)orderbyMethod.Invoke(null, new object[] { source, keySelector });
        }

        #endregion
    }
}