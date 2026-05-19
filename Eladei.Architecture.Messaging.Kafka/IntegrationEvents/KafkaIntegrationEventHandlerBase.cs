using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Eladei.Architecture.Messaging.Kafka.IntegrationEvents;

/// <summary>
/// Base class for Kafka integration event handlers
/// </summary>
/// <typeparam name="E">The type of the integration event</typeparam>
public abstract class KafkaIntegrationEventHandlerBase<E>
    : IntegrationEventHandlerBase<E>, IHandleMessages<E>
    where E : IIntegrationEvent
{
    /// <summary>
    /// Creates an instance of the KafkaIntegrationEventHandlerBase class
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="correlationContext">Correlation context</param>
    /// <param name="logger">Optional logger</param>
    public KafkaIntegrationEventHandlerBase(
        CancellationToken cancellationToken,
        ICorrelationContext correlationContext,
        ILogger? logger = null)
        : base(cancellationToken, correlationContext, logger)
    {
    }
}