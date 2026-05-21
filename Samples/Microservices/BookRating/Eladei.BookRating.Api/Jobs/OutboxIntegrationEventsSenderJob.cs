using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Jobs.Quartz;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.BookRating.Infrastructure.Outbox;

namespace Eladei.BookRating.Api.Jobs;

/// <summary>
/// Job for sending integration events from the outbox
/// </summary>
/// <remarks>
/// Reserves integration events in the outbox and publishes them.
/// Reserved events become available for re-reservation when one of the following occurs:
/// - successful event sending;
/// - sending error;
/// - expiration of the reservation period
/// </remarks>
public sealed class OutboxIntegrationEventsSenderJob : QuartzJobBase
{
    private readonly OutboxIntegrationEventsSenderJobConfig _jobConfig;
    private readonly Guid _senderId;
    private readonly ICommandExecutor _commandExecutor;
    private readonly IIntegrationEventBus _integrationEventBus;

    /// <summary>
    /// Job responsible for sending integration events from the outbox
    /// </summary>
    /// <param name="jobConfig">Configuration for outbox integration events sender job</param>
    /// <param name="commandExecutor">Command executor</param>
    /// <param name="integrationEventBus">Integration event bus used to publish events</param>
    /// <param name="correlationContext">Correlation context for distributed tracing and logging</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="ArgumentNullException"></exception>
    public OutboxIntegrationEventsSenderJob(
        OutboxIntegrationEventsSenderJobConfig jobConfig,
        ICommandExecutor commandExecutor,
        IIntegrationEventBus integrationEventBus,
        ICorrelationContext correlationContext,
        ILogger<OutboxIntegrationEventsSenderJob> logger) : base(correlationContext, logger)
    {
        _jobConfig = jobConfig
            ?? throw new ArgumentNullException(nameof(jobConfig));

        _commandExecutor = commandExecutor
            ?? throw new ArgumentNullException(nameof(commandExecutor));

        _integrationEventBus = integrationEventBus
            ?? throw new ArgumentNullException(nameof(integrationEventBus));

        _senderId = Guid.NewGuid();
    }

    /// <inheritdoc />
    protected override async Task Perform(CancellationToken cancellationToken)
    {
        var reservedEventsCount = await _commandExecutor.ExecuteAsync(
            new ReserveIntegrationEventsInOutboxForSendingCommand(_senderId, _jobConfig.ReservingTimeInSeconds, _jobConfig.MaxEventsToReserve), cancellationToken);

        if (reservedEventsCount != 0)
            await _commandExecutor.ExecuteAsync(
                new SendIntegrationEventsFromOutboxCommand(_senderId, _jobConfig.ReservingTimeInSeconds, _integrationEventBus), cancellationToken);
    }
}