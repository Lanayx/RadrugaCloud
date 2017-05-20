namespace Infrastructure.InfrastructureTools
{
    using System.Linq;

    using Core.CommonModels.Query;
    using Core.Tools;
    using Core.Tools.CopyHelper;

    /// <summary>
    ///     Class MemoryQueryExtensions
    /// </summary>
    public static class MemoryQueryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Simples apply.
        /// </summary>
        /// <typeparam name="T">
        /// Requested type
        /// </typeparam>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// IQueryable{``0}.
        /// </returns>
        public static IQueryable<T> SimpleApply<T>(this QueryOptions<T> options, IQueryable<T> source) where T : new()
        {
            var result = source;
            if (options.Filter != null)
            {
                result = result.Where(options.Filter);
            }

            if (options.OrderBy != null)
            {
                result = QueryableHelper.ApplySort(result, options.OrderBy);
            }

            if (options.Skip.HasValue)
            {
                result = result.Skip(options.Skip.Value);
            }

            if (options.Top.HasValue)
            {
                result = result.Take(options.Top.Value);
            }

            //TODO
            //if (options.Select != null)
            //{
            //    result = result.Select(options.Select).Select(res => (T)res.CopyTo(new T(),true));
            //}

            return result;
        }

        #endregion
    }
}