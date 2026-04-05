using CqrsWithEntityFrameworkExecuting.DomainModel.Commands;
using CqrsWithEntityFrameworkExecuting.DomainModel.Queries;
using CqrsWithEntityFrameworkExecuting.Infrastructure;
using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Cqrs.EntityFramework.Queries;
using Microsoft.Extensions.Logging;

namespace CqrsWithEntityFrameworkExecuting;

internal class Program {
    private static EfCommandExecutor<BookRatingDbContext> _commandExecutor;
    private static EfQueryExecutor<BookRatingDbContext> _queryExecutor;

    static async Task Main(string[] args) {
        SetExecutors();

        var registerBookCommand = new RegisterBookCommand("Капитанская дочка", "А.С.Пушкин");

        var bookId = await _commandExecutor.ExecuteAsync(registerBookCommand, CancellationToken.None);
        Console.WriteLine($"\nЗарегистрирована книга Id='{bookId}'\n");

        var voteForBookCommand = new VoteForBookCommand(bookId);
        await _commandExecutor.ExecuteAsync(voteForBookCommand, CancellationToken.None);

        var findBookQuery = new FindBookByIdQuery(bookId);

        var bookInfo = await _queryExecutor.ExecuteAsync(findBookQuery, CancellationToken.None);
        ShowBookInfo(bookInfo);

        var removeBookCommand = new RemoveBookCommand(bookId);
        await _commandExecutor.ExecuteAsync(removeBookCommand, CancellationToken.None);
    }

    private static void SetExecutors() {
        // Логгеры для команд и запросов
        var loggerFactory = LoggerFactory.Create(builder
            => { builder.AddConsole(); });

        var commandLogger = loggerFactory.CreateLogger<EfCommandExecutorLogger>();
        var eventDaoLogger = loggerFactory.CreateLogger<MockOutboxDomainEventDao>();
        var queryLogger = loggerFactory.CreateLogger<EfQueryExecutorLogger>();

        // Контекст данных
        var contextFactory = new DbContextFactory();

        // Исполнитель команд
        _commandExecutor = new EfCommandExecutor<BookRatingDbContext>(
            contextFactory,
            new MockOperationExecutionPolicyService(),
            new MockOutboxDomainEventDao(eventDaoLogger),
            new EfCommandExecutorLogger(commandLogger));

        // Исполнитель запросов
        _queryExecutor = new EfQueryExecutor<BookRatingDbContext>(
            contextFactory,
            new EfQueryExecutorLogger(queryLogger));
    }

    private static void ShowBookInfo(BookInRatingReadModel book) {
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