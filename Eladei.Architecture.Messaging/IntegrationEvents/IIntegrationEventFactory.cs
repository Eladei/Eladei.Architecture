using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.Architecture.Messaging.IntegrationEvents;

/// <summary>
/// Фабрика для формирования событий интеграции
/// </summary>
public interface IIntegrationEventFactory {
    /// <summary>
    /// Создать событие интеграции на основе события предметной области
    /// </summary>
    /// <remarks>Если событие интеграции не должно быть отправлено, то возвращает null</remarks>
    /// <param name="domainEvent">Событие предметной области</param>
    /// <param name="correlationId">Id для сквозного отслеживания</param>
    /// <returns>Событие интеграции</returns>
    IIntegrationEvent? Create(IDomainEvent domainEvent, Guid correlationId);
}