namespace Scaler.Notification.Models
{
    public class EmailRequest
    {
        public required string RecipientName { get; set; }
        public required string RecipientEmail { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public bool IsHtml { get; set; } = true;
    }
}
