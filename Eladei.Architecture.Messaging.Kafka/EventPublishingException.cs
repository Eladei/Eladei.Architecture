namespace Eladei.Architecture.Messaging.Kafka;

/// <summary>
/// Ошибка публикации сообщения
/// </summary>
public class EventPublishingException : Exception {
    public EventPublishingException(string message, Exception innerException) : base(message, innerException) { }
}