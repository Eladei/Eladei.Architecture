using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.Architecture.Messaging.Kafka.IntegrationEvents;

namespace Eladei.BookInfo.Api.Extensions;

public static class DictionaryExtensions
{
    public static KafkaIntegrationEventMetadata ExtractMetadata(this Dictionary<string, string> headers) =>
        new()
        {
            MessageKey = headers.TryGetValue(IIntegrationEventBus.IntegrationEventHeadersNames.MessageKey, out var messageKeyStr) && Guid.TryParse(messageKeyStr, out var messageKey) ? messageKey : null,
            EventId = headers.TryGetValue(IIntegrationEventBus.IntegrationEventHeadersNames.EventId, out var eventIdStr) && Guid.TryParse(eventIdStr, out var eventId) ? eventId : null,
            CorrelationId = headers.TryGetValue(IIntegrationEventBus.IntegrationEventHeadersNames.CorrelationId, out var correlationIdStr) && Guid.TryParse(correlationIdStr, out var correlationId) ? correlationId : throw new ArgumentException()
        };
}
