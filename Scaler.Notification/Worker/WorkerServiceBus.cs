﻿using Scaler.ServiceBusMessaging;

namespace Scaler.Notification.Worker
{
    public class WorkerServiceBus : IHostedService, IDisposable
    {
        private readonly ILogger<WorkerServiceBus> _logger;
        private readonly IServiceBusConsumer _serviceBusConsumer;
        private readonly IServiceBusTopicSubscription _serviceBusTopicSubscription;

        public WorkerServiceBus(IServiceBusConsumer serviceBusConsumer,
            IServiceBusTopicSubscription serviceBusTopicSubscription,
            ILogger<WorkerServiceBus> logger)
        {
            _serviceBusConsumer = serviceBusConsumer;
            _serviceBusTopicSubscription = serviceBusTopicSubscription;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Starting the service bus queue consumer and the subscription");
            await _serviceBusConsumer.RegisterOnMessageHandlerAndReceiveMessages().ConfigureAwait(false);
            await _serviceBusTopicSubscription.PrepareFiltersAndHandleMessages().ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Stopping the service bus queue consumer and the subscription");
            await _serviceBusConsumer.CloseQueueAsync().ConfigureAwait(false);
            await _serviceBusTopicSubscription.CloseSubscriptionAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async void Dispose(bool disposing)
        {
            if (disposing)
            {
                await _serviceBusConsumer.DisposeAsync().ConfigureAwait(false);
                await _serviceBusTopicSubscription.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
