namespace Eladei.Architecture.Messaging.IntegrationEvents;

/// <summary>
/// Integration event bus
/// </summary>
public interface IIntegrationEventBus
{
    /// <summary>
    /// Publishes an integration event
    /// </summary>
    /// <param name="integrationEvent">The integration event</param>
    Task PublishEventAsync(IIntegrationEvent integrationEvent);
}

/// <summary>
/// Integration event bus that processes events in parallel (without ordering guarantees).
/// </summary>
public interface IParallelIntegrationEventBus : IIntegrationEventBus { }