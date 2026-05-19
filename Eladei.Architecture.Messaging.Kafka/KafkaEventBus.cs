using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.Architecture.Messaging.Kafka;
using Eladei.Architecture.Messaging.Kafka.Properties;
using Microsoft.Extensions.Logging;
using Rebus.Bus;

/// <summary>
/// Kafka-based event bus
/// </summary>
public sealed class KafkaEventBus : IIntegrationEventBus, IDisposable
{
    private readonly IBus _eventBus;
    private readonly ILogger<KafkaEventBus>? _logger;
    private readonly string _topic;

    /// <summary>
    /// Creates an instance of KafkaEventBus
    /// </summary>
    /// <param name="kafkaBus">Kafka bus instance</param>
    /// <param name="topic">Topic to which events will be published</param>
    /// <param name="logger">Optional logger</param>
    public KafkaEventBus(IBus kafkaBus, string topic, ILogger<KafkaEventBus>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(kafkaBus, nameof(kafkaBus));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(topic, nameof(topic));

        _eventBus = kafkaBus;
        _topic = topic;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task PublishEventAsync(IIntegrationEvent integrationEvent)
    {
        try
        {
            await _eventBus.Advanced.Topics.Publish(_topic, integrationEvent);

            if (_logger is not null)
            {
                var msg = string.Format(
                    Resources.IntegrationEventWasPublished,
                    integrationEvent.GetType().Name,
                    integrationEvent.EventId,
                    _topic);

                _logger.LogInformation(msg);
            }
        }
        catch (Exception ex)
        {
            var msg = string.Format(
                Resources.IntegrationEventPublishingError,
                integrationEvent.GetType().Name,
                integrationEvent.EventId,
                _topic);

            throw new EventPublishingException(msg, ex);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _eventBus.Dispose();
    }
}