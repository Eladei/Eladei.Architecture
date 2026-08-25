namespace Eladei.Architecture.Messaging.Kafka;

/// <summary>
/// Exception thrown when a message publishing fails
/// </summary>
public class EventPublishingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <seealso cref="EventPublishingException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference</param>
    public EventPublishingException(string message, Exception innerException)
        : base(message, innerException) { }
}
