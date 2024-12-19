namespace Scaler.ServiceBusMessaging;

public interface IProcessData
{
    Task Process(EmailPayload myPayload);
}
