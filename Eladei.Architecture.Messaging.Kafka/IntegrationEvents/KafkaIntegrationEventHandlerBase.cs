using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Eladei.Architecture.Messaging.Kafka.IntegrationEvents;

/// <summary>
/// Базовый класс обработчика события интеграции для Kafka
/// </summary>
/// <typeparam name="E">Тип события интеграции</typeparam>
public abstract class KafkaIntegrationEventHandlerBase<E> : IntegrationEventHandlerBase<E>, IHandleMessages<E> where E : IIntegrationEvent
{
    /// <summary>
    /// Создает объект класса KafkaStatelessIntegrationEventHandlerBase
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <param name="correlationContext">Контекст корреляции</param>
    /// <param name="logger">Логгер</param>
    public KafkaIntegrationEventHandlerBase(CancellationToken cancellationToken, ICorrelationContext correlationContext, ILogger? logger = null)
        : base(cancellationToken, correlationContext, logger) { }
}