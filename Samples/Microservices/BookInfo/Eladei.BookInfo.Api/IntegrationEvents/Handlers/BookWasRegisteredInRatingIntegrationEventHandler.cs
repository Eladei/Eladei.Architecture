using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.Kafka.IntegrationEvents;
using Eladei.BookInfo.Domain.Commands;
using Eladei.BookInfo.Domain.Exceptions;
using Eladei.BookRating.Contract.Messaging.IntegrationEvents;

namespace Eladei.BookInfo.Api.IntegrationEvents.Handlers;

/// <summary>
/// Handler for book registration event in rating
/// </summary>
public sealed class BookWasRegisteredInRatingIntegrationEventHandler
    : KafkaIntegrationEventHandlerBase<BookWasRegisteredInRatingIntegrationEvent>
{
    private readonly ICommandExecutor _commandExecutor;

    /// <summary>
    /// Creates an instance of <see cref="BookWasRegisteredInRatingIntegrationEventHandler"/>
    /// </summary>
    /// <param name="commandExecutor">Command executor</param>
    /// <param name="correlationContext">Correlation context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="logger">Logger</param>
    /// <exception cref="ArgumentNullException"></exception>
    public BookWasRegisteredInRatingIntegrationEventHandler(
        ICommandExecutor commandExecutor,
        ICorrelationContext correlationContext,
        CancellationToken cancellationToken,
        ILogger<BookWasRegisteredInRatingIntegrationEventHandler>? logger)
        : base(cancellationToken, correlationContext, logger)
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