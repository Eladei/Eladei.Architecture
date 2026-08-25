using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.Architecture.Messaging.Kafka.Properties;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Kafka.SchemaRegistry;

namespace Eladei.Architecture.Messaging.Kafka;

/// <summary>
/// Kafka-based event bus
/// </summary>
public class KafkaEventBus : IIntegrationEventBus, IDisposable
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
    public virtual async Task PublishEventAsync(IIntegrationEvent integrationEvent, Dictionary<string, string>? headers = null)
    {
        var publishHeaders = headers is null
            ? []
            : new Dictionary<string, string>(headers);

        publishHeaders.TryGetValue(IIntegrationEventBus.IntegrationEventHeadersNames.EventId, out var eventId);

        if (publishHeaders.TryGetValue(IIntegrationEventBus.IntegrationEventHeadersNames.MessageKey, out var messageKey))
        {
            publishHeaders[KafkaHeaders.KafkaKey] = messageKey;
        }

        try
        {
            await _eventBus.Advanced.Topics.Publish(_topic, integrationEvent, publishHeaders);

            if (_logger is not null)
            {
                var msg = string.Format(
                    Resources.IntegrationEventWasPublished,
                    integrationEvent.GetType().Name,
                    eventId,
                    _topic);

                _logger.LogInformation(msg);
            }
        }
        catch (Exception ex)
        {
            var msg = string.Format(
                Resources.IntegrationEventPublishingError,
                integrationEvent.GetType().Name,
                eventId,
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
