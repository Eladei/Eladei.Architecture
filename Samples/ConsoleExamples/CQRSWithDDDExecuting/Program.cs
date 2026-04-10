using CqrsWithDddExecuting.Application;
using CqrsWithDddExecuting.Infrastructure;
using CqrsWithDddExecuting.ReadModel;
using Eladei.Architecture.Cqrs.Ddd.Commands;
using Eladei.Architecture.Cqrs.Ddd.Queries;
using Microsoft.Extensions.Logging;

namespace CqrsWithDddExecuting;

internal class Program
{
    private static DddCommandExecutor _commandExecutor;
    private static DddQueryExecutor _queryExecutor;

    static async Task Main(string[] args)
    {
        SetExecutors();

        // Выполнить команды
        var registerBookCommand = new RegisterBookCommand("Капитанская дочка", "А.С. Пушкин");

        var bookId = await _commandExecutor.ExecuteAsync(registerBookCommand, CancellationToken.None);
        Console.WriteLine($"\nЗарегистрирована книга Id='{bookId}'\n");

        var voteForBookCommand = new VoteForBookCommand(bookId);
        await _commandExecutor.ExecuteAsync(voteForBookCommand, CancellationToken.None);

        // Выполнить запрос
        var query = new FindBookByIdQuery(bookId);

        var foundBook = await _queryExecutor.ExecuteAsync(query, CancellationToken.None);
        ShowBookInfo(foundBook);
    }

    private static void SetExecutors()
    {
        // Логгеры для команд и запросов
        var loggerFactory = LoggerFactory.Create(builder
            =>
        { builder.AddConsole(); });

        var commandLogger = loggerFactory.CreateLogger<DddCommandExecutorLogger>();
        var eventDaoLogger = loggerFactory.CreateLogger<MockOutboxDomainEventDao>();
        var queryLogger = loggerFactory.CreateLogger<DddQueryExecutorLogger>();

        // Контекст данных
        var contextFactory = new MockUnitOfWorkContextFactory();

        // Исполнитель команд
        _commandExecutor = new DddCommandExecutor(
            contextFactory,
            new MockOperationExecutionPolicyService(),
            new MockOutboxDomainEventDao(eventDaoLogger),
            new DddCommandExecutorLogger(commandLogger));

        // Исполнитель запросов
        _queryExecutor = new DddQueryExecutor(
            contextFactory,
            new DddQueryExecutorLogger(queryLogger));
    }

    private static void ShowBookInfo(BookInRatingReadModel book)
    {
        Console.WriteLine(
@$"
Информация по зарегистрированной книге: 
Id: {book.BookId}
Name: {book.Name}
Author: {book.Author}
Votes: {book.Votes}
");
    }
}