namespace Core.Interfaces.Providers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.CommonModels.Results;

    public interface INotificationProvider
    {
        /// <summary>
        /// Sends the notification.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        Task<OperationResult> SendNotification(IEnumerable<string> tags, string title, string message);
    }
}
