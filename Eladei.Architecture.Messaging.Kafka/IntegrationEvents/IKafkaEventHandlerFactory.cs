using Eladei.Architecture.Messaging.IntegrationEvents;
using Rebus.Handlers;

namespace Eladei.Architecture.Messaging.Kafka.IntegrationEvents;

/// <summary>
/// Factory for integration event handlers
/// </summary>
public interface IKafkaEventHandlerFactory
{
    /// <summary>
    /// Creates an integration event handler
    /// </summary>
    /// <typeparam name="H">The type of the integration event handler</typeparam>
    /// <typeparam name="E">The type of the integration event</typeparam>
    /// <param name="cancellationToken">Cancellation token for event processing</param>
    /// <returns>The integration event handler</returns>
    H CreateHandler<H, E>(CancellationToken cancellationToken)
        where H : IHandleMessages<E>
        where E : IIntegrationEvent;
}