using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.Architecture.Messaging.Kafka.Properties;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Eladei.Architecture.Messaging.Kafka.IntegrationEvents;

/// <summary>
/// Base class for Kafka integration event handlers
/// </summary>
/// <typeparam name="E">The type of the integration event</typeparam>
public abstract class KafkaIntegrationEventHandlerBase<E>
    : IHandleMessages<E> where E : IIntegrationEvent
{
    /// <summary>
    /// Cancellation token
    /// </summary>
    /// <remarks>
    /// Propagates notification that current event handling should be cancelled
    /// </remarks>
    protected readonly CancellationToken CancellationToken;

    /// <summary>
    /// Correlation context
    /// </summary>
    /// <remarks>
    /// Used to propagate a correlationId across the system for tracing
    /// the full execution flow of operations
    /// </remarks>
    protected readonly ICorrelationContext CorrelationContext;

    /// <summary>
    /// Metadata for the integration event
    /// </summary>
    protected readonly KafkaIntegrationEventMetadata EventMetadata;

    /// <summary>
    /// The logger
    /// </summary>
    protected readonly ILogger? Logger;

    /// <summary>
    /// Creates an instance of the KafkaIntegrationEventHandlerBase class
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="correlationContext">Correlation context</param>
    /// <param name="eventMetadata">Metadata for the integration event</param>
    /// <param name="logger">The logger</param>
    public KafkaIntegrationEventHandlerBase(
        CancellationToken cancellationToken,
        ICorrelationContext correlationContext,
        KafkaIntegrationEventMetadata eventMetadata,
        ILogger? logger = null)
    {
        CancellationToken = cancellationToken;

        CorrelationContext = correlationContext
            ?? throw new ArgumentNullException(nameof(correlationContext));

        EventMetadata = eventMetadata
            ?? throw new ArgumentNullException(nameof(eventMetadata));

        Logger = logger;
    }

    /// <summary>
    /// Handles an integration event
    /// </summary>
    /// <param name="integrationEvent">The integration event</param>
    public virtual async Task Handle(E integrationEvent)
    {
        using (CorrelationContext.SetCorrelationId(EventMetadata.CorrelationId))
        {
            LogHandlingStarted(integrationEvent);

            using var innerTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(CancellationToken);

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
            EventMetadata.EventId);

        Logger?.LogInformation(msg);
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
            EventMetadata.EventId);

        Logger?.LogInformation(msg);
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
            EventMetadata.EventId);

        Logger?.LogInformation(ex, msg);
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
            EventMetadata.EventId);

        Logger?.LogInformation(ex, msg);
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
            EventMetadata.EventId);

        Logger?.LogCritical(ex, errorMsg);
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
