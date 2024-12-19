using Scaler.Core.Services;
using Scaler.ServiceBusMessaging;

namespace Scaler.Server.Services.Email
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailSender(ILogger<EmailSender> logger, ServiceBusSender serviceBusSender)
        : IEmailSender
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="senderName"></param>
        /// <param name="senderEmail"></param>
        /// <param name="recipientName"></param>
        /// <param name="recipientEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(
            string senderName,
            string senderEmail,
            string recipientName,
            string recipientEmail,
            string subject,
            string body,
            bool isHtml = true)
        {
            try
            {
                await serviceBusSender.SendMessage(new EmailPayload
                {
                    SenderEmail = senderEmail,
                    SenderName = senderName,
                    RecipientName = recipientName,
                    RecipientEmail = recipientEmail,
                    Subject = subject,
                    Body = body,
                    IsHtml = isHtml
                }).ConfigureAwait(false);

            }
            catch (Exception)
            {
                logger.LogError("Sending email Failed!");
            }
        }

    }
}
