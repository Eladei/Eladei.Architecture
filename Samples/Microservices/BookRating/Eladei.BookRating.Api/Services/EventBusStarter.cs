using Eladei.Architecture.Messaging.IntegrationEvents;

namespace Eladei.BookRating.Api.Services;

/// <summary>
/// Служба дли инициализации шины событий интеграции
/// </summary>
/// <remarks>Шина инициализируется при первом внедрении в конструктор объекта 
/// на основе настроек из CompositionRoot.</remarks>
public class EventBusStarter : IHostedService {
    private readonly IIntegrationEventBus _eventBus;

    public EventBusStarter(IIntegrationEventBus eventBus) {
        _eventBus = eventBus;
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancelToken) => Task.CompletedTask;
}