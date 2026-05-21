using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.Kafka.IntegrationEvents;
using Eladei.BookInfo.Domain.Commands;
using Eladei.BookInfo.Domain.Exceptions;
using Eladei.BookRating.Contract.Messaging.IntegrationEvents;

namespace Eladei.BookInfo.Api.IntegrationEvents.Handlers;

/// <summary>
/// Handler for book information update event in rating
/// </summary>
public sealed class BookInfoWasUpdatedInRatingIntegrationEventHandler
    : KafkaIntegrationEventHandlerBase<BookInfoWasUpdatedInRatingIntegrationEvent>
{
    private readonly ICommandExecutor _commandExecutor;

    /// <summary>
    /// Creates an instance of BookInfoWasUpdatedInRatingIntegrationEventHandler
    /// </summary>
    /// <param name="commandExecutor">Command executor</param>
    /// <param name="correlationContext">Correlation context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="logger">Logger</param>
    /// <exception cref="ArgumentNullException"></exception>
    public BookInfoWasUpdatedInRatingIntegrationEventHandler(
        ICommandExecutor commandExecutor,
        ICorrelationContext correlationContext,
        CancellationToken cancellationToken,
        ILogger<BookInfoWasUpdatedInRatingIntegrationEventHandler>? logger)
        : base(cancellationToken, correlationContext, logger)
    {
        _commandExecutor = commandExecutor
            ?? throw new ArgumentNullException(nameof(commandExecutor));
    }

    /// <inheritdoc/>
    protected override async Task HandleAsync(
        BookInfoWasUpdatedInRatingIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMainBookInfoCommand(
            integrationEvent.BookId,
            integrationEvent.Name,
            integrationEvent.Author);

        await _commandExecutor.ExecuteAsync(command, cancellationToken);
    }

    /// <inheritdoc/>
    protected override bool IgnoreException(Exception ex)
        => ex is BookWithIdNotFoundException;
}