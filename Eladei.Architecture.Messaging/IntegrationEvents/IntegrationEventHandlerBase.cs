using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.Properties;
using Microsoft.Extensions.Logging;

namespace Eladei.Architecture.Messaging.IntegrationEvents;

/// <summary>
/// Base class for integration event handlers
/// </summary>
public abstract class IntegrationEventHandlerBase<E> where E : IIntegrationEvent
{
    protected readonly CancellationToken _cancellationToken;
    protected readonly ICorrelationContext _correlationContext;
    protected readonly ILogger? _logger;

    /// <summary>
    /// Creates an instance of the IntegrationEventHandlerBase class
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="correlationContext">Correlation context</param>
    /// <param name="logger">Optional logger</param>
    public IntegrationEventHandlerBase(
        CancellationToken cancellationToken,
        ICorrelationContext correlationContext,
        ILogger? logger = null)
    {
        _cancellationToken = cancellationToken;

        _correlationContext = correlationContext
            ?? throw new ArgumentNullException(nameof(correlationContext));

        _logger = logger;
    }

    /// <summary>
    /// Handles an integration event
    /// </summary>
    /// <param name="integrationEvent">The integration event</param>
    public virtual async Task Handle(E integrationEvent)
    {
        using (_correlationContext.SetCorrelationId(integrationEvent.CorrelationId))
        {
            LogHandlingStarted(integrationEvent);

            using var innerTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken);

            try
            {
                await HandleAsync(integrationEvent, innerTokenSource.Token);

                LogHandlingSuccessfullFinished(integrationEvent);
            }
            catch (Exception ex)
            {
                if (IgnoreException(ex))
                {
                    LogHandlingErrorIgnorance(integrationEvent, ex);
                    return;
                }

                switch (ex)
                {
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

    #region Logging methods

    /// <summary>
    /// Logs the start of integration event handling
    /// </summary>
    /// <param name="integrationEvent">The integration event</param>
    protected virtual void LogHandlingStarted(E integrationEvent)
    {
        var msg = string.Format(
            Resources.IntegrationEventHandlingStarted,
            integrationEvent.GetType().Name,
            integrationEvent.EventId);

        _logger?.LogInformation(msg);
    }

    /// <summary>
    /// Logs successful completion of integration event handling
    /// </summary>
    /// <param name="integrationEvent">The integration event</param>
    protected virtual void LogHandlingSuccessfullFinished(E integrationEvent)
    {
        var msg = string.Format(
            Resources.IntegrationEventHandlingSuccessfullyFinished,
            integrationEvent.GetType().Name,
            integrationEvent.EventId);

        _logger?.LogInformation(msg);
    }

    /// <summary>
    /// Logs cancellation of integration event handling
    /// </summary>
    /// <param name="integrationEvent">The integration event</param>
    /// <param name="ex">Cancellation details</param>
    protected virtual void LogHandlingCancelled(E integrationEvent, OperationCanceledException ex)
    {
        var msg = string.Format(
            Resources.IntegrationEventHandlingCancelled,
            integrationEvent.GetType().Name,
            integrationEvent.EventId);

        _logger?.LogInformation(ex, msg);
    }

    /// <summary>
    /// Logs ignored error during integration event handling
    /// </summary>
    /// <param name="integrationEvent">The integration event</param>
    /// <param name="ex">The exception that will be ignored</param>
    protected virtual void LogHandlingErrorIgnorance(E integrationEvent, Exception ex)
    {
        var msg = string.Format(
            Resources.IntegrationEventHandlingErrorWillBeIgrored,
            integrationEvent.GetType().Name,
            integrationEvent.EventId);

        _logger?.LogInformation(ex, msg);
    }

    /// <summary>
    /// Logs a critical error during integration event handling
    /// </summary>
    /// <typeparam name="F">Exception type</typeparam>
    /// <param name="integrationEvent">The integration event</param>
    /// <param name="ex">The exception</param>
    protected virtual void LogCriticalError<F>(E integrationEvent, F ex)
        where F : Exception
    {
        var errorMsg = string.Format(
            Resources.IntegrationEventHandlingError,
            integrationEvent.GetType().Name,
            integrationEvent.EventId);

        _logger?.LogCritical(ex, errorMsg);
    }

    #endregion

    /// <summary>
    /// Determines whether an exception should be ignored
    /// </summary>
    /// <param name="ex">The exception that may be ignored</param>
    protected virtual bool IgnoreException(Exception ex)
    {
        return false;
    }

    /// <summary>
    /// Handles the integration event
    /// </summary>
    /// <param name="integrationEvent">The integration event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected abstract Task HandleAsync(E integrationEvent, CancellationToken cancellationToken);
}