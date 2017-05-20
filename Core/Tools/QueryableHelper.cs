namespace Core.Tools
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Core.CommonModels.Query;
    using Core.Enums;

    /// <summary>
    ///     Class QueryableHelper
    /// </summary>
    public static class QueryableHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// Applies the sort.
        /// </summary>
        /// <typeparam name="T">Requested parameter</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="sortDescriptions">The sort descriptions.</param>
        /// <returns>IQueryable{``0}.</returns>
        public static IQueryable<T> ApplySort<T>(IQueryable<T> source, List<SortDescription> sortDescriptions)
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
                        var orderedEnumerable = source as IOrderedQueryable<T>;

                        source = sortDescription.Direction == SortDirection.Ascending
                                     ? orderedEnumerable.ThenBy(sortDescription.KeySelector)
                                     : orderedEnumerable.ThenByDescending(sortDescription.KeySelector);
                    }
                }
            }

            return source;
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
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, Expression keySelector)
        {
            var propertyType = keySelector.GetType().GetGenericArguments()[0].GetGenericArguments()[1];
            var orderbyMethod =
                typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(x => x.Name == "OrderBy" && x.GetParameters().Length == 2);

            if (orderbyMethod == null)
            {
                return (IOrderedQueryable<T>)source;
            }

            orderbyMethod = orderbyMethod.MakeGenericMethod(typeof(T), propertyType);

            return (IOrderedQueryable<T>)orderbyMethod.Invoke(null, new object[] { source, keySelector });
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
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, Expression keySelector)
        {
            var propertyType = keySelector.GetType().GetGenericArguments()[0].GetGenericArguments()[1];
            var orderbyMethod =
                typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 2);

            if (orderbyMethod == null)
            {
                return (IOrderedQueryable<T>)source;
            }

            orderbyMethod = orderbyMethod.MakeGenericMethod(typeof(T), propertyType);

            return (IOrderedQueryable<T>)orderbyMethod.Invoke(null, new object[] { source, keySelector });
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
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, Expression keySelector)
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

            return (IOrderedQueryable<T>)orderbyMethod.Invoke(null, new object[] { source, keySelector });
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
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, Expression keySelector)
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

            return (IOrderedQueryable<T>)orderbyMethod.Invoke(null, new object[] { source, keySelector });
        }

        #endregion
    }
}