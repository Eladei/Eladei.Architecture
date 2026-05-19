using Eladei.Architecture.Ddd.DomainEvents;

namespace Eladei.Architecture.Messaging.IntegrationEvents;

/// <summary>
/// Factory for creating integration events
/// </summary>
public interface IIntegrationEventFactory
{
    /// <summary>
    /// Creates an integration event based on a domain event
    /// </summary>
    /// <remarks>If the integration event should not be published, returns null</remarks>
    /// <param name="domainEvent">The domain event</param>
    /// <param name="correlationId">The correlation ID for end-to-end tracing</param>
    /// <returns>The integration event</returns>
    IIntegrationEvent? Create(IDomainEvent domainEvent, Guid correlationId);
}