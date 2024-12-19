namespace Scaler.Core.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEmailSender
    {

        /// <summary>
        /// SendEmailAsync
        /// </summary>
        /// <param name="senderName"></param>
        /// <param name="senderEmail"></param>
        /// <param name="recipientName"></param>
        /// <param name="recipientEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        Task SendEmailAsync(
            string senderName,
            string senderEmail,
            string recipientName,
            string recipientEmail,
            string subject,
            string body,
            bool isHtml = true);
    }
}
