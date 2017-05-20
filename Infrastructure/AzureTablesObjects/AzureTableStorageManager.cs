namespace Infrastructure.AzureTablesObjects
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.Enums;
    using Core.Tools;

    using Infrastructure.InfrastructureTools;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    ///     The azure table storage manager.
    /// </summary>
    internal class AzureTableStorageManager
    {
        #region Fields

        private readonly string _tableName;

        private CloudTableClient _cloudTableClient;

        private CloudTable _cloudTableReference;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AzureTableStorageManager" /> class.
        /// </summary>
        /// <param name="tableName">
        ///     Name of the table.
        /// </param>
        public AzureTableStorageManager(AzureTableName tableName)
        {
            _tableName = tableName.ToString();
        }

        #endregion

        #region Properties

        private CloudTableClient CloudTableClient
        {
            get
            {
                if (_cloudTableClient != null)
                {
                    return _cloudTableClient;
                }

                var connectionString =
                    ConfigurationManager.ConnectionStrings["TableStorageAzureConnectionString"].ConnectionString;
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                tableClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan.FromMilliseconds(200), 3 );
                _cloudTableClient = tableClient;
                return _cloudTableClient;
            }
        }

        private CloudTable CloudTableReference
        {
            get
            {
                if (_cloudTableReference != null)
                {
                    return _cloudTableReference;
                }

                _cloudTableReference = CloudTableClient.GetTableReference(_tableName);

                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["Warming"]))
                    _cloudTableReference.CreateIfNotExists();

                return _cloudTableReference;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the entity.
        /// </summary>
        /// <typeparam name="T">
        ///     Type of entity to add
        /// </typeparam>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <returns>
        ///     AddResult.
        /// </returns>
        public async Task<IdResult> AddEntityAsync<T>(T entity) where T : ITableEntity
        {
            // Create the TableOperation that inserts the azure entity.
            TableOperation insertOperation = TableOperation.Insert(entity);

            // Execute the insert operation.
            var insertResult = await CloudTableReference.ExecuteAsync(insertOperation);
            if (insertResult.HttpStatusCode.EnsureSuccessStatusCode())
            {
                return new IdResult(entity.PartitionKey);
            }

            var message = string.Format(
                "Insert operation performed with error. StatusCode: {0}", 
                insertResult.HttpStatusCode);
            Trace.TraceError(message);

            return new IdResult(OperationResultStatus.Error, message);
        }

        /// <summary>
        ///     Adds the entity batch async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities">The entities.</param>
        /// <returns>Task{AddResult}.</returns>
        public async Task<IdResult> AddEntityBatchAsync<T>(List<T> entities) where T : ITableEntity
        {
            // Create the batch operation.
            var batchOperation = new TableBatchOperation();
            foreach (var entity in entities)
            {
                batchOperation.Insert(entity);
            }

            // Execute the insert operation.
            return await ProcessAddBatch(batchOperation, entities[0].PartitionKey);
        }

        /// <summary>
        ///     Deletes the entities batch async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities">The entities.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> DeleteEntitiesBatchAsync<T>(IEnumerable<T> entities) where T : TableEntity
        {
            var batchOperation = new TableBatchOperation();
            foreach (var entity in entities)
            {
                batchOperation.Delete(entity);
            }

            return await ProcessUpdateDeleteBatch(batchOperation);
        }

        /// <summary>
        ///     Deletes the entity async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> DeleteEntityAsync<T>(T entity) where T : TableEntity
        {
            return await DeleteEntitiesBatchAsync(new List<T> { entity });
        }

        /// <summary>
        ///     Gets the entities.
        /// </summary>
        /// <param name="query">
        ///     The query.
        /// </param>
        /// <typeparam name="T">
        ///     Type of entity to get
        /// </typeparam>
        /// <returns>
        ///     IQueryable{``0}.
        /// </returns>
        public async Task<List<T>> GetEntitiesAsync<T>(TableQuery<T> query) where T : TableEntity, new()
        {
            var entities = await CloudTableReference.ExecuteQueryAsync(query);

            return entities;
        }

        /// <summary>
        ///     Gets the entities for delete.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityId">The entity id.</param>
        /// <param name="mainEntityRowKeyToExclude">The main entity row key to exclude.</param>
        /// <param name="rowKeyPrefixesToExclude">The row key prefixes to exclude.</param>
        /// <returns>Task{IList{``0}}.</returns>
        public async Task<List<T>> GetEntitiesForCompleteDelete<T>(
            string entityId, 
            string mainEntityRowKeyToExclude = "", 
            List<string> rowKeyPrefixesToExclude = null) where T : TableEntity, new()
        {
            string filter;
            if (!string.IsNullOrEmpty(mainEntityRowKeyToExclude))
            {
                string filterA = TableQuery.GenerateFilterCondition(
                    AzureTableConstants.PartitionKey, 
                    QueryComparisons.Equal, 
                    entityId);
                string filterB = TableQuery.GenerateFilterCondition(
                    AzureTableConstants.RowKey, 
                    QueryComparisons.NotEqual, 
                    mainEntityRowKeyToExclude);
                filter = TableQuery.CombineFilters(filterA, TableOperators.And, filterB);
            }
            else
            {
                filter = TableQuery.GenerateFilterCondition(
                    AzureTableConstants.PartitionKey, 
                    QueryComparisons.Equal, 
                    entityId);
            }

            var entities = await GetEntitiesAsync(new TableQuery<T>().Where(filter));
            
            if (rowKeyPrefixesToExclude != null && rowKeyPrefixesToExclude.Any())
            {
                entities = entities.Where(e => rowKeyPrefixesToExclude.Any(r => e.RowKey.StartsWith(r))).ToList();
            }

            return entities;
        }

        /// <summary>
        /// Gets the entity by id.
        /// </summary>
        /// <typeparam name="T">Type of entity to get</typeparam>
        /// <param name="entityId">The entity id.</param>
        /// <param name="rowKey">The row key.</param>
        /// <returns>Entyty.</returns>
        public async Task<T> GetEntityByIdAndRowKeyAsync<T>(string entityId, string rowKey) where T : TableEntity
        {
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(entityId, rowKey);

            // Execute the retrieve operation.
            var retrievedResult = await CloudTableReference.ExecuteAsync(retrieveOperation);
            if (!retrievedResult.HttpStatusCode.EnsureSuccessStatusCode())
            {
                var message = $"Retrieve operation performed with error. StatusCode: {retrievedResult.HttpStatusCode}";
                Trace.TraceError(message);
                return null;
            }

            return (T)retrievedResult.Result;
        }


        public async Task<OperationResult> UpsertEntityAsync<T>(T entity, bool replace = true) where T : ITableEntity
        {
            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = replace ? 
                TableOperation.InsertOrReplace(entity) :
                TableOperation.InsertOrMerge(entity);

            // Execute the insert operation.
            var insertResult = await CloudTableReference.ExecuteAsync(insertOperation);
            if (insertResult.HttpStatusCode.EnsureSuccessStatusCode())
            {
                return new OperationResult(OperationResultStatus.Success);
            }

            var message = $"Upsert operation performed with error. StatusCode: {insertResult.HttpStatusCode}";
            return new IdResult(OperationResultStatus.Error, message);
        }

        /// <summary>
        ///     Updates the entity async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="entitiesToAdd">The entities to replace.</param>
        /// <param name="entitiesToDelete">The entities to delete.</param>
        /// <param name="replace">if set to <c>true</c> [force].</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> UpdateEntityAsync<T>(
            T entity, 
            IList<T> entitiesToAdd = null,
            IList<T> entitiesToDelete = null,
            bool replace = true) where T : TableEntity
        {
            return await UpdateEntityBatchAsync(new List<T> { entity }, entitiesToAdd, entitiesToDelete, replace);
        }

        /// <summary>
        ///     Updates the entity batch async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities">The entities.</param>
        /// <param name="entitiesToAdd">The entities to add.</param>
        /// <param name="entitiesToDelete">The entities to delete.</param>
        /// <param name="replace">if set to <c>true</c> [force].</param>
        /// <returns>Task{OperationResult}.</returns>
        public async Task<OperationResult> UpdateEntityBatchAsync<T>(
            IList<T> entities,
            IList<T> entitiesToAdd = null,
            IList<T> entitiesToDelete = null, 
            bool replace = true) where T : TableEntity
        {
            var batchOperation = new TableBatchOperation();

            foreach (var entity in entities)
            {
                if (replace)
                {
                    batchOperation.Replace(entity);
                }
                else
                {
                    batchOperation.Merge(entity);
                }
            }

            return await CompleteUpdateDeleteBatchWithRestOperationsAndStart(batchOperation, entitiesToAdd, entitiesToDelete);
        }


        public async Task<OperationResult> UpdateEntitiesAsync<T>(TableQuery<T> query, Func<IEnumerable<T>, Task> action) where T : TableEntity, new()
        {
            await CloudTableReference.ExecuteQueryAsyncNoResult(query, onProgress: action);
            return new OperationResult(OperationResultStatus.Success);
        }

        #endregion

        #region Methods

        private async Task<OperationResult> CompleteUpdateDeleteBatchWithRestOperationsAndStart<T>(
            TableBatchOperation batchOperation,
            IList<T> entitiesToAdd,
            IList<T> entitiesToDelete) where T : TableEntity
        {
            if (entitiesToAdd != null)
            {
                foreach (var entityToAdd in entitiesToAdd)
                {
                    batchOperation.Insert(entityToAdd);
                } 
            }

            if (entitiesToDelete != null)
            {
                foreach (var entityToDelete in entitiesToDelete)
                {
                    batchOperation.Delete(entityToDelete);
                } 
            }

            // Execute the operation.
            return await ProcessUpdateDeleteBatch(batchOperation);
        }

        private async Task<List<string>> ExecuteBatchAndGetFailedResults(TableBatchOperation batchOperation)
        {
            var failedOperations = new List<string>();
            if (batchOperation.Count <= 0)
            {
                return failedOperations;
            }

            var batchResult = await CloudTableReference.ExecuteBatchAsync(batchOperation);
            
            foreach (var tableResult in batchResult)
            {
                if (!tableResult.HttpStatusCode.EnsureSuccessStatusCode())
                {
                    failedOperations.Add(
                        $"{batchResult.IndexOf(tableResult)}-{(HttpStatusCode)tableResult.HttpStatusCode}");
                }
            }

            return failedOperations;
        }

        private async Task<IdResult> ProcessAddBatch(TableBatchOperation batchOperation, string successResult)
        {
            var failedOperations = await ExecuteBatchAndGetFailedResults(batchOperation);

            if (!failedOperations.Any())
            {
                return new IdResult(successResult);
            }

            var message = $"Some batch operations failed({failedOperations.JoinToString()}).";
            Trace.TraceError(message);

            return new IdResult(OperationResultStatus.Error, message);
        }

        private async Task<OperationResult> ProcessUpdateDeleteBatch(TableBatchOperation batchOperation)
        {
            var failedOperations = await ExecuteBatchAndGetFailedResults(batchOperation);

            if (!failedOperations.Any())
            {
                return new OperationResult(OperationResultStatus.Success);
            }

            var message = $"Some batch operations failed({failedOperations.JoinToString()}).";
            Trace.TraceError(message);

            return new OperationResult(OperationResultStatus.Error, message);
        }

        #endregion
    }
}