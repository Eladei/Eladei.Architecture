using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.Kafka.IntegrationEvents;
using Eladei.BookInfo.Application.Commands;
using Eladei.BookInfo.Application.Exceptions;
using Eladei.BookRating.Contract.Messaging.IntegrationEvents;

namespace Eladei.BookInfo.Api.IntegrationEvents.Handlers;

public sealed class BookWasRegisteredInRatingIntegrationEventHandler
    : KafkaIntegrationEventHandlerBase<BookWasRegisteredInRatingIntegrationEvent>
{
    private readonly ICommandExecutor _commandExecutor;

    /// <exception cref="ArgumentNullException"></exception>
    public BookWasRegisteredInRatingIntegrationEventHandler(
        ICommandExecutor commandExecutor,
        ICorrelationContext correlationContext,
        KafkaIntegrationEventMetadata eventMetadata,
        CancellationToken cancellationToken,
        ILogger<BookWasRegisteredInRatingIntegrationEventHandler>? logger)
        : base(cancellationToken, correlationContext, eventMetadata, logger)
    {
        _commandExecutor = commandExecutor
            ?? throw new ArgumentNullException(nameof(commandExecutor));
    }

    /// <inheritdoc/>
    protected override async Task HandleAsync(
        BookWasRegisteredInRatingIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var command = new AddBookCommand(
            integrationEvent.BookId,
            integrationEvent.Name,
            integrationEvent.Author);

        await _commandExecutor.ExecuteAsync(command, cancellationToken);
    }

    /// <inheritdoc/>
    protected override bool IgnoreException(Exception ex)
        => ex is BookWithCurrentIdAlreadyExistsException;
}
