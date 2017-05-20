namespace Infrastructure.InfrastructureTools.Stub
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.CommonModels.Results;
    using Core.Enums;
    using Core.Interfaces.Providers;

    public class NotificationProvider: INotificationProvider
    {
        public Task<Core.CommonModels.Results.OperationResult> SendNotification(IEnumerable<string> tags, string title, string message)
        {
            return Task.Factory.StartNew(() => new OperationResult(OperationResultStatus.Success));
        }
    }
}
