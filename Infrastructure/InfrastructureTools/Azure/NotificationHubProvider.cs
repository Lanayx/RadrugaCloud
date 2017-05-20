using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.InfrastructureTools.Azure
{
    using System.Diagnostics;
    using System.Web.Configuration;

    using Core.CommonModels.Results;
    using Core.Enums;
    using Core.Interfaces.Providers;
    using Microsoft.Azure.NotificationHubs;

    /// <summary>
    /// Azure notification hub
    /// </summary>
    /// <seealso cref="Core.Interfaces.Providers.INotificationProvider" />
    public class NotificationHubProvider : INotificationProvider
    {
        /// <summary>
        /// Sends the notification.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public async Task<OperationResult> SendNotification(IEnumerable<string> tags, string title, string message)
        {
            var connectionString =
                WebConfigurationManager.AppSettings["Microsoft.Azure.NotificationHubs.ConnectionString"];
            var hub = NotificationHubClient.CreateClientFromConnectionString(connectionString, "RadrugaNotificationHub",true);
            var result = await hub.SendTemplateNotificationAsync(new Dictionary<string, string> { { "title", title }, { "message", message } }, tags);
            Trace.TraceInformation("Send notification result.  Success {0}, Fail {1}", result.Success,result.Failure);
            return result.Success > 0 || result.Failure == 0 ? 
                new OperationResult(OperationResultStatus.Success) :
                new OperationResult(OperationResultStatus.Error, "Couldn't send a notification");
        }
    }
}
