namespace Scaler.ServiceBusMessaging;

public interface IServiceBusTopicSubscription
{
    Task PrepareFiltersAndHandleMessages();
    Task CloseSubscriptionAsync();
    ValueTask DisposeAsync();
}
