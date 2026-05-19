using Eladei.Architecture.Messaging.IntegrationEvents;
using Rebus.Kafka.SchemaRegistry;
using Rebus.Messages;
using Rebus.Pipeline;

namespace Eladei.Architecture.Messaging.Kafka.Interceptors;

/// <summary>
/// Adds a "kafka-key" header with the integration event identifier (IIntegrationEvent.EntityId).
/// </summary>
public sealed class AddKafkaKeyHeaderByEventIdStepInterceptor : IOutgoingStep
{
    public async Task Process(OutgoingStepContext context, Func<Task> next)
    {
        var message = context.Load<Message>();

        if (message?.Body is IIntegrationEvent body)
        {
            message.Headers[KafkaHeaders.KafkaKey] = $"{body.EntityId}";
        }

        await next();
    }
}