namespace Infrastructure.InfrastructureTools.Sendgrid
{
    using Core.CommonModels.Results;
    using Core.Interfaces.Providers;
    using System;
    using System.Net;
    using System.Net.Mail;
    using SendGrid;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Core.Enums;

    public class MailProvider : IMailProvider
    {
        public async Task<OperationResult> SendMail(string email, string title, string text)
        {
            try
            {
                SendGridMessage myMessage = new SendGridMessage();
                myMessage.From = new MailAddress("support@radruga.com");
                myMessage.AddTo(email);
                myMessage.Subject = title;
                myMessage.Html = text;

                // Create credentials, specifying your user name and password.
                var credentials = new NetworkCredential("azure_9f0cfd721ec804914e63b38cb34008ed@azure.com", "RadEmail2016");

                // Create an Web transport for sending email.
                var transportWeb = new Web(credentials);

                // Send the email.
                await transportWeb.DeliverAsync(myMessage);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return new OperationResult(OperationResultStatus.Error,"Email sending failed");
            }
            return new OperationResult(OperationResultStatus.Success);

        }
    }
}
