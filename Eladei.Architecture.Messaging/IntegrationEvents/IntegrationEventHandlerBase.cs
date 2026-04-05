using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.Properties;
using Microsoft.Extensions.Logging;

namespace Eladei.Architecture.Messaging.IntegrationEvents;

/// <summary>
/// Базовый класс обработчика события интеграции
/// </summary>
public abstract class IntegrationEventHandlerBase<E> where E : IIntegrationEvent {
    protected readonly CancellationToken _cancellationToken;
    protected readonly ICorrelationContext _correlationContext;
    protected readonly ILogger? _logger;

    /// <summary>
    /// Создает объект класса IntegrationEventHandlerBase
    /// </summary>
    /// <param name="cancellationToken">Токена отмены</param>
    /// <param name="logger">Логгер</param>
    public IntegrationEventHandlerBase(
        CancellationToken cancellationToken, 
        ICorrelationContext correlationContext, 
        ILogger? logger = null) {
        _cancellationToken = cancellationToken;

        _correlationContext = correlationContext
            ?? throw new ArgumentNullException(nameof(correlationContext));

        _logger = logger;
    }

    /// <summary>
    /// Обрабатывает событие интеграции
    /// </summary>
    /// <param name="integrationEvent">Событие интеграции</param>
    public virtual async Task Handle(E integrationEvent) {
        using (_correlationContext.SetCorrelationId(integrationEvent.CorrelationId)) {
            LogHandlingStarted(integrationEvent);

            using var innerTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken);

            try {
                await HandleAsync(integrationEvent, innerTokenSource.Token);

                LogHandlingSuccessfullFinished(integrationEvent);
            }
            catch (Exception ex) {
                if (IgnoreException(ex)) {
                    LogHandlingErrorIgnorance(integrationEvent, ex);

                    return;
                }

                switch (ex) {
                    case OperationCanceledException:
                        LogHandlingCancelled(integrationEvent, (OperationCanceledException)ex);
                        break;
                    default:
                        LogCriticalError(integrationEvent, ex);
                        break;
                }

                throw;
            }
        }
    }

    #region Методы логирования

    /// <summary>
    /// Логировать начало обработки события интеграции
    /// </summary>
    /// <param name="integrationEvent">Событие интеграции</param>
    protected virtual void LogHandlingStarted(E integrationEvent) {
        var msg = string.Format(
            Resources.IntegrationEventHandlingStarted,
            integrationEvent.GetType().Name,
            integrationEvent.EventId);

        _logger?.LogInformation(msg);
    }

    /// <summary>
    /// Логировать успешное завершение обработки события интеграции
    /// </summary>
    /// <param name="integrationEvent">Событие интеграции</param>
    protected virtual void LogHandlingSuccessfullFinished(E integrationEvent) {
        var msg = string.Format(
            Resources.IntegrationEventHandlingSuccessfullyFinished,
            integrationEvent.GetType().Name,
            integrationEvent.EventId);

        _logger?.LogInformation(msg);
    }

    /// <summary>
    /// Логировать отмену обработки события интеграции
    /// </summary>
    /// <param name="integrationEvent">Событие интеграции</param>
    /// <param name="ex">Данные по отмене операции</param>
    protected virtual void LogHandlingCancelled(E integrationEvent, OperationCanceledException ex) {
        var msg = string.Format(
            Resources.IntegrationEventHandlingCancelled,
            integrationEvent.GetType().Name,
            integrationEvent.EventId);

        _logger?.LogInformation(ex, msg);
    }

    /// <summary>
    /// Логировать игнорирование ошибки обработки события интеграции
    /// </summary>
    /// <param name="integrationEvent">Событие интеграции</param>
    /// <param name="ex">Ошибка, которая будет проигнорирована</param>
    protected virtual void LogHandlingErrorIgnorance(E integrationEvent, Exception ex) {
        var msg = string.Format(
            Resources.IntegrationEventHandlingErrorWillBeIgrored,
            integrationEvent.GetType().Name,
            integrationEvent.EventId);

        _logger?.LogInformation(ex, msg);
    }

    /// <summary>
    /// Логировать ошибку обработки события интеграции
    /// </summary>
    /// <typeparam name="F">Тип ошибки</typeparam>
    /// <param name="integrationEvent">Событие интеграции</param>
    /// <param name="ex">Ошибка</param>
    protected virtual void LogCriticalError<F>(E integrationEvent, F ex) where F : Exception {
        var errorMsg = string.Format(
            Resources.IntegrationEventHandlingError, 
            integrationEvent.GetType().Name, 
            integrationEvent.EventId);

        _logger?.LogCritical(ex, errorMsg);
    }

    #endregion

    /// <summary>
    /// Определение необходимости игнорирования ошибки
    /// </summary>
    /// <param name="ex">Ошибка, которая может быть проигнорирована</param>
    protected virtual bool IgnoreException(Exception ex) {
        return false;
    }

    /// <summary>
    /// Обработать событие интеграции
    /// </summary>
    /// <param name="integrationEvent">Событие интеграции</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат выполнения операции</returns>
    protected abstract Task HandleAsync(E integrationEvent, CancellationToken cancellationToken);
}