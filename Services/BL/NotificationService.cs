namespace Services.BL
{
    using System.Threading.Tasks;

    using Core.CommonModels.Results;
    using Core.Interfaces.Providers;

    public class NotificationService
    {
        private readonly INotificationProvider _notificationsProvider;

        public NotificationService(INotificationProvider notificationsProvider)
        {
            _notificationsProvider = notificationsProvider;
        }

        public Task<OperationResult> ApproveMissionNotify(string userId, string missionName)
        {
            return _notificationsProvider.SendNotification(new[] { userId }, "Ура!", "Миссия \"" + missionName + "\" засчитана!");
        }

        public Task<OperationResult> DeclineMissionNotify(string userId, string missionName)
        {
            return _notificationsProvider.SendNotification(new[] { userId }, "Увы...", "Миссия \"" + missionName + "\" не засчитана.");
        }

    }
}
