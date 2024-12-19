using Scaler.Notification.Services;
using Scaler.ServiceBusMessaging;

namespace Scaler.Notification.Worker
{
    public class ProcessEmail : IProcessData
    {
        private readonly IEmailSenderService _emailSenderService;

        public ProcessEmail(IEmailSenderService emailSenderService)
        {
            _emailSenderService = emailSenderService;
        }

        public async Task Process(EmailPayload myPayload)
        {
            try
            {
                await _emailSenderService.SendEmailAsync(myPayload.SenderName, myPayload.SenderEmail, myPayload.RecipientName, myPayload.RecipientEmail, myPayload.Subject, myPayload.Body, myPayload.IsHtml);

            }
            catch (Exception)
            {

                throw new Exception("Sending Email Failed!");
            }
        }
    }
}
