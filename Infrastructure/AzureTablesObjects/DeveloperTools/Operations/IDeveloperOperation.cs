namespace Infrastructure.AzureTablesObjects.DeveloperTools.Operations
{
    using System.Threading.Tasks;

    using Core.CommonModels.Results;

    /// <summary>
    /// Interface IDeveloperOperation
    /// </summary>
    internal interface IDeveloperOperation
    {
        /// <summary>
        /// Excecutes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>OperationResult.</returns>
        Task<OperationResult> Excecute(params object[] args);
    }
}