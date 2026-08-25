using Eladei.Architecture.Messaging.IntegrationEvents;

namespace Eladei.BookInfo.Api.Services;

public class EventBusStarter : IHostedService
{
    private readonly IIntegrationEventBus _eventBus;

    public EventBusStarter(IIntegrationEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancelToken) => Task.CompletedTask;
}
