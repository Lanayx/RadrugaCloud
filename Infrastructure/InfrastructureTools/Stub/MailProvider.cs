namespace Infrastructure.InfrastructureTools.Stub
{
    using System.Threading.Tasks;
    using Core.CommonModels.Results;
    using Core.Enums;
    using Core.Interfaces.Providers;

    public class MailProvider:IMailProvider
    {
        public Task<Core.CommonModels.Results.OperationResult> SendMail(string email, string title, string text)
        {
            return Task.Factory.StartNew(() => new OperationResult(OperationResultStatus.Success));
        }
    }
}
