using Eladei.Architecture.Messaging.IntegrationEvents;
using Rebus.Handlers;

namespace Eladei.Architecture.Messaging.Kafka.IntegrationEvents;

/// <summary>
/// Фабрика обработчиков событий интеграции
/// </summary>
public interface IKafkaEventHandlerFactory
{
    /// <summary>
    /// Создает обработчик события интеграции
    /// </summary>
    /// <typeparam name="H">Тип обработчика события интеграции</typeparam>
    /// <typeparam name="E">Тип события интеграции</typeparam>
    /// <param name="cancellationToken">Токен отмены обработки события</param>
    /// <returns>Обработчик события интеграции</returns>
    H CreateHandler<H, E>(CancellationToken cancellationToken)
        where H : IHandleMessages<E>
        where E : IIntegrationEvent;
}