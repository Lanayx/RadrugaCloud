namespace Core.Interfaces.Providers
{
    using System.Threading.Tasks;
    using Core.CommonModels.Results;

    public interface IMailProvider
    {
        Task<OperationResult> SendMail(string email, string title, string text);
    }
}
