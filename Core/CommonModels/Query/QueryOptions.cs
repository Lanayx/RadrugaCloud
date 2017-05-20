namespace Core.CommonModels.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    ///     The query options.
    /// </summary>
    /// <typeparam name="T">
    ///     Requested type
    /// </typeparam>
    public class QueryOptions<T>
    {
        /// <summary>
        ///     Gets or sets the expand.
        /// </summary>
        public List<string> Expand { get; set; }

        /// <summary>
        ///     Gets or sets the filter.
        /// </summary>
        public Expression<Func<T, bool>> Filter { get; set; }

        /// <summary>
        ///     Gets or sets the order by.
        /// </summary>
        public List<SortDescription> OrderBy { get; set; }

        /// <summary>
        ///     Gets or sets the select expression.
        /// </summary>
        public Expression<Func<T, object>> Select { get; set; }

        /// <summary>
        ///     Gets or sets the skip.
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        ///     Gets or sets the top.
        /// </summary>
        public int? Top { get; set; }
    }
}