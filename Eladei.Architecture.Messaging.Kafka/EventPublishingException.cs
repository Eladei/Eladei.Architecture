namespace Eladei.Architecture.Messaging.Kafka;

/// <summary>
/// Exception thrown when a message publishing fails
/// </summary>
public class EventPublishingException : Exception
{
    public EventPublishingException(string message, Exception innerException)
        : base(message, innerException) { }
}