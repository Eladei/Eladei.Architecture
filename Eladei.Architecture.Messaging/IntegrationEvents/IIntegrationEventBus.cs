namespace Eladei.Architecture.Messaging.IntegrationEvents;

/// <summary>
/// Шина событий интеграции
/// </summary>
public interface IIntegrationEventBus
{
    /// <summary>
    /// Опубликовать событие интеграции
    /// </summary>
    /// <param name="integrationEvent">Событие интеграции</param>
    Task PublishEventAsync(IIntegrationEvent integrationEvent);
}

/// <summary>
/// Шина событий интеграции, которые должны обрабатываться параллельно
/// </summary>
public interface IParallelIntegrationEventBus : IIntegrationEventBus { }