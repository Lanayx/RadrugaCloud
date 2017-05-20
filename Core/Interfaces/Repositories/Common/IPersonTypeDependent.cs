namespace Core.Interfaces.Repositories.Common
{
    using System.Threading.Tasks;

    using Core.CommonModels.Results;

    /// <summary>
    ///     Interface IPersonQualityDependent
    /// </summary>
    public interface IPersonQualityDependent
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Removes the type of the links to deleted person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <returns>Task{OperationResult}.</returns>
        Task<OperationResult> RemoveLinksToDeletedPersonQuality(string personQualityId);

        /// <summary>
        ///     Updates the type of the links to person.
        /// </summary>
        /// <param name="personQualityId">The person quality id.</param>
        /// <param name="personQualityName">Name of the person quality.</param>
        /// <returns>Task{OperationResult}.</returns>
        Task<OperationResult> UpdateLinksToPersonQuality(string personQualityId, string personQualityName);

        #endregion
    }
}