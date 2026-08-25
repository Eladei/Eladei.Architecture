using CqrsWithEntityFrameworkExecuting.Application.Commands;
using CqrsWithEntityFrameworkExecuting.Application.Queries;
using CqrsWithEntityFrameworkExecuting.Infrastructure;
using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Cqrs.EntityFramework.Queries;
using Microsoft.Extensions.Logging;

namespace CqrsWithEntityFrameworkExecuting;

internal class Program
{
    private static EfCommandExecutor<BookRatingDbContext> _commandExecutor = null!;
    private static EfQueryExecutor<BookRatingDbContext> _queryExecutor = null!;

    static async Task Main(string[] args)
    {
        SetExecutors();

        var registerBookCommand = new RegisterBookCommand("The Captain's Daughter", "A.S. Pushkin");

        var bookId = await _commandExecutor.ExecuteAsync(registerBookCommand, CancellationToken.None);
        Console.WriteLine($"\nBook registered with Id='{bookId}'\n");

        var voteForBookCommand = new VoteForBookCommand(bookId);
        await _commandExecutor.ExecuteAsync(voteForBookCommand, CancellationToken.None);

        var findBookQuery = new FindBookByIdQuery(bookId);

        var bookInfo = await _queryExecutor.ExecuteAsync(findBookQuery, CancellationToken.None);
        ShowBookInfo(bookInfo);

        var removeBookCommand = new RemoveBookCommand(bookId);
        await _commandExecutor.ExecuteAsync(removeBookCommand, CancellationToken.None);
    }

    private static void SetExecutors()
    {
        // Loggers for commands and queries
        var loggerFactory = LoggerFactory.Create(builder
            =>
        { builder.AddConsole(); });

        var commandLogger = loggerFactory.CreateLogger<EfCommandExecutorLogger>();
        var eventDaoLogger = loggerFactory.CreateLogger<MockOutboxIntegrationEventWriter>();
        var queryLogger = loggerFactory.CreateLogger<EfQueryExecutorLogger>();

        // Db context factory
        var contextFactory = new DbContextFactory();

        // Command executor
        _commandExecutor = new EfCommandExecutor<BookRatingDbContext>(
            contextFactory,
            new MockOperationExecutionPolicyProvider(),
            new MockOutboxIntegrationEventWriter(eventDaoLogger),
            new EfCommandExecutorLogger(commandLogger));

        // Query executor
        _queryExecutor = new EfQueryExecutor<BookRatingDbContext>(
            contextFactory,
            new EfQueryExecutorLogger(queryLogger));
    }

    private static void ShowBookInfo(BookInRatingReadModel book)
    {
        Console.WriteLine(
@$"
Information about the registered book:
Id: {book.BookId}
Name: {book.Name}
Author: {book.Author}
Votes: {book.Votes}
");
    }
}
