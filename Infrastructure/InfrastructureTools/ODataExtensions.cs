namespace Infrastructure.InfrastructureTools
{
    using System.Linq;
    using System.Web.Http.OData.Query;
    using Linq2Rest;
    using Linq2Rest.Parser;
    using Linq2Rest.Parser.Readers;

    using Core.CommonModels.Query;
    using Core.Enums;

    /// <summary>
    /// Class ODataExtensions
    /// </summary>
    public static class ODataExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// To the query options.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="odataQueryOptions">The o data query options.</param>
        /// <returns>QueryOptions{``0}.</returns>
        public static QueryOptions<T> ToQueryOptions<T>(this ODataQueryOptions<T> odataQueryOptions)
        {
            if (odataQueryOptions == null)
                return null;
            var nameResolver = new MemberNameResolver();
            var selectFactory = new SelectExpressionFactory<T>(nameResolver, new RuntimeTypeProvider(nameResolver));
            var filterFactiory = new FilterExpressionFactory(nameResolver, Enumerable.Empty<IValueExpressionFactory>());
            var orderFactory = new SortExpressionFactory(nameResolver);

            return new QueryOptions<T>
                       {
                           Top = odataQueryOptions.Top?.Value,
                           Skip = odataQueryOptions.Skip?.Value,
                           Select =
                               !string.IsNullOrEmpty(odataQueryOptions.SelectExpand?.RawSelect)
                                   ? selectFactory.Create(odataQueryOptions.SelectExpand.RawSelect)
                                   : null,
                           Expand =
                               !string.IsNullOrEmpty(odataQueryOptions.SelectExpand?.RawExpand)
                                   ? odataQueryOptions.SelectExpand.RawExpand.Split(',').ToList()
                                   : null,
                           OrderBy =
                               !string.IsNullOrEmpty(odataQueryOptions.OrderBy?.RawValue)
                                   ? orderFactory.Create<T>(odataQueryOptions.OrderBy.RawValue)
                                         .Select(
                                             description =>
                                             new SortDescription(
                                                 description.KeySelector,
                                                 (SortDirection)description.Direction))
                                         .ToList()
                                   : null,
                           Filter =
                               !string.IsNullOrEmpty(odataQueryOptions.Filter?.RawValue)
                                   ? filterFactiory.Create<T>(odataQueryOptions.Filter.RawValue)
                                   : null
                       };
        }

        #endregion
    }
}