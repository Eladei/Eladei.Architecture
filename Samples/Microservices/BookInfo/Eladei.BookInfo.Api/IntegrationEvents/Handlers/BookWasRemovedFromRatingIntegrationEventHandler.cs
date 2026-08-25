using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.Kafka.IntegrationEvents;
using Eladei.BookInfo.Application.Commands;
using Eladei.BookInfo.Application.Exceptions;
using Eladei.BookRating.Contract.Messaging.IntegrationEvents;

namespace Eladei.BookInfo.Api.IntegrationEvents.Handlers;

public sealed class BookWasRemovedFromRatingIntegrationEventHandler
    : KafkaIntegrationEventHandlerBase<BookWasRemovedFromRatingIntegrationEvent>
{
    private readonly ICommandExecutor _commandExecutor;

    /// <exception cref="ArgumentNullException"></exception>
    public BookWasRemovedFromRatingIntegrationEventHandler(
        ICommandExecutor commandExecutor,
        ICorrelationContext correlationContext,
        KafkaIntegrationEventMetadata eventMetadata,
        CancellationToken cancellationToken,
        ILogger<BookWasRemovedFromRatingIntegrationEventHandler>? logger)
        : base(cancellationToken, correlationContext, eventMetadata, logger)
    {
        _commandExecutor = commandExecutor
            ?? throw new ArgumentNullException(nameof(commandExecutor));
    }

    /// <inheritdoc/>
    protected override async Task HandleAsync(
        BookWasRemovedFromRatingIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var command = new RemoveBookInfoCommand(integrationEvent.BookId);

        await _commandExecutor.ExecuteAsync(command, cancellationToken);
    }

    /// <inheritdoc/>
    protected override bool IgnoreException(Exception ex)
        => ex is BookWithIdNotFoundException;
}
