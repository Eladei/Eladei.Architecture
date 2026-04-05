using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Logging;
using Eladei.Architecture.Messaging.Kafka.IntegrationEvents;
using Eladei.BookInfo.Domain.Commands;
using Eladei.BookInfo.Domain.Exceptions;
using Eladei.BookRating.Contract.Messaging.IntegrationEvents;

namespace Eladei.BookInfo.Api.IntegrationEvents.Handlers;

/// <summary>
/// Обработчик события обновления информации о книге в рейтинге
/// </summary>
public sealed class BookInfoWasUpdatedInRatingIntegrationEventHandler : KafkaIntegrationEventHandlerBase<BookInfoWasUpdatedInRatingIntegrationEvent> {
    private readonly ICommandExecutor _commandExecutor;

    /// <summary>
    /// Создает объект класса BookInfoWasUpdatedInRatingIntegrationEventHandler
    /// </summary>
    /// <param name="commandExecutor">Исполнитель команд</param>
    /// <param name="correlationContext">Контекст корреляции</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <param name="logger">Логгер</param>
    /// <exception cref="ArgumentNullException"></exception>
    public BookInfoWasUpdatedInRatingIntegrationEventHandler(
        ICommandExecutor commandExecutor,
        ICorrelationContext correlationContext,
        CancellationToken cancellationToken,
        ILogger<BookInfoWasUpdatedInRatingIntegrationEventHandler>? logger) : base(cancellationToken, correlationContext, logger) {
        _commandExecutor = commandExecutor 
            ?? throw new ArgumentNullException(nameof(commandExecutor));
    }

    protected override async Task HandleAsync(BookInfoWasUpdatedInRatingIntegrationEvent integrationEvent, CancellationToken cancellationToken) {
        var command = new UpdateMainBookInfoCommand(
            integrationEvent.BookId, integrationEvent.Name, integrationEvent.Author);

        await _commandExecutor.ExecuteAsync(command, cancellationToken);
    }

    protected override bool IgnoreException(Exception ex)
        => ex is BookWithIdNotFoundException ? true : false;
}