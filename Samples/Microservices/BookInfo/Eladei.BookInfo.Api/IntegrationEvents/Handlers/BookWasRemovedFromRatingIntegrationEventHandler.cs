using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.Kafka.IntegrationEvents;
using Eladei.BookInfo.Domain.Commands;
using Eladei.BookInfo.Domain.Exceptions;
using Eladei.BookRating.Contract.Messaging.IntegrationEvents;

namespace Eladei.BookInfo.Api.IntegrationEvents.Handlers;

/// <summary>
/// Обработчик события удаления книги из рейтинга
/// </summary>
public sealed class BookWasRemovedFromRatingIntegrationEventHandler : KafkaIntegrationEventHandlerBase<BookWasRemovedFromRatingIntegrationEvent>
{
    private readonly ICommandExecutor _commandExecutor;

    /// <summary>
    /// Создает объект класса BookWasRemovedFromRatingIntegrationEventHandler
    /// </summary>
    /// <param name="commandExecutor">Исполнитель команд</param>
    /// <param name="correlationContext">Контекст корреляции</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <param name="logger">Логгер</param>
    /// <exception cref="ArgumentNullException"></exception>
    public BookWasRemovedFromRatingIntegrationEventHandler(
        ICommandExecutor commandExecutor,
        ICorrelationContext correlationContext,
        CancellationToken cancellationToken,
        ILogger<BookWasRemovedFromRatingIntegrationEventHandler>? logger) : base(cancellationToken, correlationContext, logger)
    {
        _commandExecutor = commandExecutor
            ?? throw new ArgumentNullException(nameof(commandExecutor));
    }

    protected override async Task HandleAsync(BookWasRemovedFromRatingIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var command = new RemoveBookInfoCommand(integrationEvent.BookId);

        await _commandExecutor.ExecuteAsync(command, cancellationToken);
    }

    protected override bool IgnoreException(Exception ex)
        => ex is BookWithIdNotFoundException ? true : false;
}