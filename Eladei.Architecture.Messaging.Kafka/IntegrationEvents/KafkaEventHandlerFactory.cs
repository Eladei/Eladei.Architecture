using Eladei.Architecture.Messaging.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Handlers;

namespace Eladei.Architecture.Messaging.Kafka.IntegrationEvents;

/// <summary>
/// Factory for integration event handlers
/// </summary>
public class KafkaEventHandlerFactory : IKafkaEventHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Creates an instance of the KafkaEventHandlerFactory class
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    public KafkaEventHandlerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public H CreateHandler<H, E>(CancellationToken cancellationToken)
        where H : IHandleMessages<E>
        where E : IIntegrationEvent
    {
        return ActivatorUtilities.CreateInstance<H>(_serviceProvider, cancellationToken);
    }
}