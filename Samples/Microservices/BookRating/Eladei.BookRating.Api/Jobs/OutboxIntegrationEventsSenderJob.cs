using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Jobs.Quartz;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.BookRating.Infrastructure.Outbox;

namespace Eladei.BookRating.Api.Jobs;

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
            new ReserveOutboxMessagesForSendingCommand(_senderId, _jobConfig.ReservingTimeInSeconds, _jobConfig.MaxEventsToReserve), cancellationToken);

        if (reservedEventsCount != 0)
            await _commandExecutor.ExecuteAsync(
                new SendOutboxMessagesCommand(_senderId, _jobConfig.ReservingTimeInSeconds, _integrationEventBus), cancellationToken);
    }
}
