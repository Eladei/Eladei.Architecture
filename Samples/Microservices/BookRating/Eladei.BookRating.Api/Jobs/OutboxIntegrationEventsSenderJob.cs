using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Jobs.Quartz;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.BookRating.Infrastructure.Outbox;

namespace Eladei.BookRating.Api.Jobs;

/// <summary>
/// Job рассылки событий интеграции из outbox
/// </summary>
/// <remarks>Резервирует события интеграции в outbox и осуществляет их рассылку.
/// Зарезервированные события доступны для резервирования при одном из следующих условий:
/// - успешная отправка события;
/// - ошибка отправки события;
/// - истечение срока резервирования</remarks>
public sealed class OutboxIntegrationEventsSenderJob : QuartzJobBase {
    private readonly OutboxIntegrationEventsSenderJobConfig _jobConfig;
    private readonly Guid _senderId;
    private readonly ICommandExecutor _commandExecutor;
    private readonly IIntegrationEventBus _integrationEventBus;

    public OutboxIntegrationEventsSenderJob(
        OutboxIntegrationEventsSenderJobConfig jobConfig,
        ICommandExecutor commandExecutor,
        IIntegrationEventBus integrationEventBus,
        ICorrelationContext correlationContext,
        ILogger<OutboxIntegrationEventsSenderJob> logger) : base(correlationContext, logger) {
        _jobConfig = jobConfig
            ?? throw new ArgumentNullException(nameof(jobConfig));

        _commandExecutor = commandExecutor
            ?? throw new ArgumentNullException(nameof(commandExecutor));

        _integrationEventBus = integrationEventBus
            ?? throw new ArgumentNullException(nameof(integrationEventBus));

        _senderId = Guid.NewGuid();
    }

    protected override async Task Perform(CancellationToken cancellationToken) {
        var reservedEventsCount = await _commandExecutor.ExecuteAsync(
            new ReserveIntegrationEventsInOutboxForSendingCommand(_senderId, _jobConfig.ReservingTimeInSeconds, _jobConfig.MaxEventsToReserve), cancellationToken);

        if (reservedEventsCount != 0)
            await _commandExecutor.ExecuteAsync(
                new SendIntegrationEventsFromOutboxCommand(_senderId, _jobConfig.ReservingTimeInSeconds, _integrationEventBus), cancellationToken);
    }
}