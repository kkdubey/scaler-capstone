namespace Scaler.ServiceBusMessaging;

public class EmailPayload
{
    public required string SenderName { get; set; }
    public required string SenderEmail { get; set; }
    public required string RecipientName { get; set; }
    public required string RecipientEmail { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public bool IsHtml { get; set; } = true;
}
