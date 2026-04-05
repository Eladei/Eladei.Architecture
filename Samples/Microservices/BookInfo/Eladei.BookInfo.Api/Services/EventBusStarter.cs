using Eladei.Architecture.Messaging.IntegrationEvents;

namespace Eladei.BookInfo.Api.Services;

/// <summary>
/// Служба дли инициализации шины событий интеграции
/// </summary>
public class EventBusStarter : IHostedService {
    private readonly IIntegrationEventBus _eventBus;

    public EventBusStarter(IIntegrationEventBus eventBus) {
        _eventBus = eventBus;
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancelToken) => Task.CompletedTask;
}