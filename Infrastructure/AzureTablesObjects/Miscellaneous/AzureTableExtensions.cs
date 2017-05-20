namespace Infrastructure.AzureTablesObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Core.CommonModels.Query;
    using Core.Tools;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    ///     The azure extensions.
    /// </summary>
    public static class AzureTableExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <param name="target">The object instance to copy to.</param>
        public static void CopyToTableEntity<T>(this T source, T target) where T : ITableEntity
        {
            var tableEntityProperties = typeof(ITableEntity).GetProperties().Select(p => p.Name).ToArray();
            foreach (PropertyInfo sourceProperty in source.GetType().GetProperties())
            {
                if (tableEntityProperties.Contains(sourceProperty.Name))
                {
                    continue;
                }

                foreach (PropertyInfo targetProperty in target.GetType().GetProperties())
                {
                    if (targetProperty.Name != sourceProperty.Name || targetProperty.PropertyType != sourceProperty.PropertyType)
                    {
                        continue;
                    }

                    MethodInfo setMethod = targetProperty.GetSetMethod();

                    if (setMethod != null)
                    {
                        setMethod.Invoke(target, new[] { sourceProperty.GetGetMethod().Invoke(source, null) });
                    }
                    else
                    {
                        //Do nothing
                    }
                }
            }
        }

        /// <summary>
        /// Generates the table query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TTable">The type of the T table.</typeparam>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="initialFilter">The initial filter.</param>
        /// <returns>TableQuery{``0}.</returns>
        public static TableQuery<TTable> GenerateTableQuery<T, TTable>(
            this QueryOptions<T> queryOptions,
            string initialFilter = "")
        {
            var tableQuery = new TableQuery<TTable>();
            if (queryOptions == null)
            {
                return tableQuery;
            }

            var filter = initialFilter;
            if (queryOptions.Filter != null)
            {
                var tableFilter = queryOptions.Filter.GetRestQuery();
                filter = !string.IsNullOrEmpty(initialFilter)
                             ? TableQuery.CombineFilters(initialFilter, TableOperators.And, tableFilter)
                             : tableFilter;
            }

            if (!string.IsNullOrEmpty(filter))
            {
                tableQuery = tableQuery.Where(filter);
            }

            if (queryOptions.OrderBy != null)
            {
                return tableQuery;
            }

            var selectProperties = queryOptions.Select.GetAnonymousProperties();
            if (selectProperties.Any())
            {
                tableQuery = tableQuery.Select(selectProperties);
            }

            // TODO Skip, Take
            return tableQuery;
        }

        /// <summary>
        /// Generates the starts with filter condition.
        /// </summary>
        /// <param name="startsWithField">The table field for filter.</param>
        /// <param name="startsWithPattern">The starts with pattern for filter.</param>
        /// <returns>
        /// string
        /// </returns>
        public static string GenerateStartsWithFilterCondition(string startsWithField,string startsWithPattern)
        {
            var length = startsWithPattern.Length - 1;
            var lastChar = startsWithPattern[length];

            var nextLastChar = (char)(lastChar + 1);

            var startsWithEndPattern = startsWithPattern.Substring(0, length) + nextLastChar;

            var startsWithCondition = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(startsWithField,
                    QueryComparisons.GreaterThanOrEqual,
                    startsWithPattern),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(startsWithField,
                    QueryComparisons.LessThan,
                    startsWithEndPattern)
                );
            
            return startsWithCondition;
        }

        /// <summary>
        ///     Executes the query async.
        /// </summary>
        /// <typeparam name="T">
        ///     Requsted type
        /// </typeparam>
        /// <param name="table">
        ///     The table.
        /// </param>
        /// <param name="query">
        ///     The query.
        /// </param>
        /// <param name="cancellationToken">
        ///     The cancellation token.
        /// </param>
        /// <param name="onProgress">
        ///     The on progress.
        /// </param>
        /// <returns>
        ///     Task{IList{``0}}.
        /// </returns>
        public static async Task<List<T>> ExecuteQueryAsync<T>(
            this CloudTable table, 
            TableQuery<T> query, 
            CancellationToken cancellationToken = default(CancellationToken), 
            Action<List<T>> onProgress = null) where T : ITableEntity, new()
        {
            var items = new List<T>();

            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<T> seg = await table.ExecuteQueryAsync(query, token, cancellationToken);
                token = seg.ContinuationToken;
                items.AddRange(seg);
                if (onProgress != null)
                {
                    onProgress(items);
                }
            }
            while (token != null && !cancellationToken.IsCancellationRequested);

            return items;
        }

        public static async Task ExecuteQueryAsyncNoResult<T>(
            this CloudTable table,
            TableQuery<T> query,
            CancellationToken cancellationToken = default(CancellationToken),
            Func<IEnumerable<T>, Task> onProgress = null) where T : ITableEntity, new()
        {
            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<T> seg = await table.ExecuteQueryAsync(query, token, cancellationToken);
                token = seg.ContinuationToken;
                if (onProgress != null)
                {
                    await onProgress(seg);
                }
            }
            while (token != null && !cancellationToken.IsCancellationRequested);
            
        }

        /// <summary>
        /// Filters the collection post factum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initialCollection">The initial collection.</param>
        /// <param name="options">The options.</param>
        /// <returns>IEnumerable{``0}.</returns>
        public static List<T> FilterCollectionPostFactum<T>(
            this List<T> initialCollection,
            QueryOptions<T> options)
        {
            if (options == null || !initialCollection.AnyValues())
            {
                return initialCollection;
            }

            if (options.OrderBy != null && options.OrderBy.Any())
            {
                initialCollection = EnumerableHelper.Sort(initialCollection, options.OrderBy).ToList();
            }

            if (options.Skip.HasValue)
            {
                initialCollection = initialCollection.Skip(options.Skip.Value).ToList();
            }

            if (options.Top.HasValue)
            {
                initialCollection = initialCollection.Take(options.Top.Value).ToList();
            }

            return initialCollection;
        }

        /// <summary>
        /// Gets the filter by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>System.String.</returns>
        public static string GetFilterById(this string id)
        {
            return TableQuery.GenerateFilterCondition(
                AzureTableConstants.PartitionKey,
                QueryComparisons.Equal,
                id);
        }

        /// <summary>
        /// Filters the updatable links.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="linksToAdd">The links to add.</param>
        /// <param name="linksToBeDeleted">The links to be deleted.</param>
        /// <returns>IEnumerable{``0}.</returns>
        public static List<T> FilterUpdatableLinks<T>(List<T> linksToAdd, List<T> linksToBeDeleted) where T : TableEntity
        {
            var deletedLinks = linksToBeDeleted;
            var sameLinks = linksToAdd.Where(l => deletedLinks.Any(dl => dl.RowKey == l.RowKey)).Select(l => l.RowKey).ToIList();
            var linksForUpdate = new List<T>();
            foreach (var sameLink in sameLinks)
            {
                var inBaseLink = linksToBeDeleted.First(l => l.RowKey == sameLink);
                var updatedLink = linksToAdd.First(l => l.RowKey == sameLink);
                updatedLink.CopyToTableEntity(inBaseLink);
                linksForUpdate.Add(inBaseLink);
            }

            linksToAdd.RemoveAll(l => sameLinks.Contains(l.RowKey));
            linksToBeDeleted.RemoveAll(l => sameLinks.Contains(l.RowKey));
            return linksForUpdate;
        }

        /// <summary>
        ///     Gets the partition key.
        /// </summary>
        /// <param name="rowKey">The row key.</param>
        /// <returns>System.String.</returns>
        public static string GetPartitionKey(this string rowKey)
        {
            if (string.IsNullOrEmpty(rowKey) || rowKey.Length < 2)
            {
                return rowKey;
            }

            return rowKey.Substring(0, 2);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Executes the query async.
        /// </summary>
        /// <typeparam name="T">
        ///     Requsted type
        /// </typeparam>
        /// <param name="table">
        ///     The table.
        /// </param>
        /// <param name="query">
        ///     The query.
        /// </param>
        /// <param name="token">
        ///     The token.
        /// </param>
        /// <param name="cancellationToken">
        ///     The cancellation token.
        /// </param>
        /// <returns>
        ///     Task{TableQuerySegment{``0}}.
        /// </returns>
        private static Task<TableQuerySegment<T>> ExecuteQueryAsync<T>(
            this CloudTable table, 
            TableQuery<T> query, 
            TableContinuationToken token, 
            CancellationToken cancellationToken = default(CancellationToken)) where T : ITableEntity, new()
        {
            ICancellableAsyncResult ar = table.BeginExecuteQuerySegmented(query, token, null, null);
            cancellationToken.Register(ar.Cancel);

            return Task.Factory.FromAsync<TableQuerySegment<T>>(ar, table.EndExecuteQuerySegmented<T>);
        }

        #endregion
    }
}