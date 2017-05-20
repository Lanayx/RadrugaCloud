namespace Infrastructure.AzureTablesObjects.DeveloperTools
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.Enums;

    using Infrastructure.AzureTablesObjects.DeveloperTools.Operations;

    /// <summary>
    ///     Class DeveloperStorageManager
    /// </summary>
    public class DeveloperStorageManager
    {
        #region Fields

        private readonly Dictionary<DeveloperOperation, Type> _operationTypes =
            new Dictionary<DeveloperOperation, Type>
                {
                    { DeveloperOperation.DeleteAllTables, typeof(DeleteAllTables) },
                    { DeveloperOperation.FixExternalImages, typeof(FixExternalImages) },
                    { DeveloperOperation.FillTablesWithData, typeof(FillTablesWithTestData) },
                    { DeveloperOperation.CleanTempImagesStorage, typeof(CleanTempImagesStorage) },
                    { DeveloperOperation.UpdateUserLocations, typeof(UpdateUserLocations) },
                    { DeveloperOperation.UpdateKindActionsCount, typeof(UpdateKindActionsCount) },
                    { DeveloperOperation.FixLightQuality, typeof(FixLightQuality) },
                };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Executes the operation.
        /// </summary>
        /// <param name="operationName">Name of the operation.</param>
        /// <param name="args">The args.</param>
        /// <returns>OperationResult.</returns>
        public async Task<OperationResult> ExecuteOperation(DeveloperOperation operationName, params object[] args)
        {
            if (!_operationTypes.ContainsKey(operationName))
            {
                return new OperationResult(OperationResultStatus.Error, "Operation is not supported");
            }

            var operationProvider = (IDeveloperOperation)Activator.CreateInstance(_operationTypes[operationName]);
            return await operationProvider.Excecute(args);
        }

        #endregion
    }
}