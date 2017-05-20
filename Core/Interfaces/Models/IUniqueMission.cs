namespace Core.Interfaces.Models
{
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.DomainModels;

    /// <summary>
    /// Interface IUniqueMission
    /// </summary>
    public interface IUniqueMission
    {
        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Task{MissionCompletionResult}.</returns>
        Task<MissionCompletionResult> ProcessRequest(MissionRequest request);
    }
}
