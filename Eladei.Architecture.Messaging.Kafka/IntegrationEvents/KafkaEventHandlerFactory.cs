using Eladei.Architecture.Messaging.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Handlers;

namespace Eladei.Architecture.Messaging.Kafka.IntegrationEvents;

/// <summary>
/// Фабрика обработчиков событий интеграции
/// </summary>
public class KafkaEventHandlerFactory : IKafkaEventHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Создает объект класса EventHandlerFactory
    /// </summary>
    /// <param name="serviceProvider">Поставщик сервисов</param>
    public KafkaEventHandlerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public H CreateHandler<H, E>(CancellationToken cancellationToken)
        where H : IHandleMessages<E>
        where E : IIntegrationEvent
    {
        return ActivatorUtilities.CreateInstance<H>(_serviceProvider, cancellationToken);
    }
}