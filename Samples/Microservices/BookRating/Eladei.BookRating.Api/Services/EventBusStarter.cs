using Eladei.Architecture.Messaging.IntegrationEvents;

namespace Eladei.BookRating.Api.Services;

/// <summary>
/// Service for initializing the integration event bus
/// </summary>
/// <remarks>The bus is initialized upon the first injection into the constructor of an object
/// based on settings from CompositionRoot.</remarks>
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