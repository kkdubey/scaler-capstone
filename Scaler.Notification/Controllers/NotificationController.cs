using Microsoft.AspNetCore.Mvc;
using Scaler.Notification.Models;
using Scaler.Notification.Services;

namespace Scaler.Notification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IEmailSenderService _emailSenderService;

        public NotificationController(IEmailSenderService emailSenderService)
        {
            _emailSenderService = emailSenderService;
        }


        // write method to send email
        [HttpPost("sendEmail")]
        public IActionResult SendEmail(EmailRequest emailRequest)
        {
            return Ok(_emailSenderService.SendEmailAsync(emailRequest.RecipientName, emailRequest.RecipientEmail, emailRequest.Subject, emailRequest.Body, emailRequest.IsHtml));
        }

    }
}
