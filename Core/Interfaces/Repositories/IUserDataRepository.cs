namespace Core.Interfaces.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Core.CommonModels.Results;

    public interface IUserDataRepository : IDisposable
    {
        Task<OperationResult> PostMessageToDevelopers(string userId, string message);
    }
}
