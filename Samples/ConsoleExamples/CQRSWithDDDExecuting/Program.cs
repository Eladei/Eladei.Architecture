using CqrsWithDddExecuting.Application;
using CqrsWithDddExecuting.Infrastructure;
using CqrsWithDddExecuting.ReadModel;
using Eladei.Architecture.Cqrs.Ddd.Commands;
using Eladei.Architecture.Cqrs.Ddd.Queries;
using Microsoft.Extensions.Logging;

namespace CqrsWithDddExecuting;

internal class Program
{
    private static DddCommandExecutor _commandExecutor = null!;
    private static DddQueryExecutor _queryExecutor = null!;

    static async Task Main(string[] args)
    {
        SetExecutors();

        // Execute commands
        var registerBookCommand = new RegisterBookCommand("The Captain's Daughter", "A.S. Pushkin");

        var bookId = await _commandExecutor.ExecuteAsync(registerBookCommand, CancellationToken.None);
        Console.WriteLine($"\nBook registered with Id='{bookId}'\n");

        var voteForBookCommand = new VoteForBookCommand(bookId);
        await _commandExecutor.ExecuteAsync(voteForBookCommand, CancellationToken.None);

        // Execute query
        var query = new FindBookByIdQuery(bookId);

        var foundBook = await _queryExecutor.ExecuteAsync(query, CancellationToken.None);
        ShowBookInfo(foundBook);
    }

    private static void SetExecutors()
    {
        // Loggers for commands and queries
        var loggerFactory = LoggerFactory.Create(builder
            =>
        { builder.AddConsole(); });

        var commandLogger = loggerFactory.CreateLogger<DddCommandExecutorLogger>();
        var eventDaoLogger = loggerFactory.CreateLogger<MockOutboxIntegrationEventWriter>();
        var queryLogger = loggerFactory.CreateLogger<DddQueryExecutorLogger>();

        // Db context factory
        var contextFactory = new MockUnitOfWorkContextFactory();

        // Command executor
        _commandExecutor = new DddCommandExecutor(
            contextFactory,
            new MockOperationExecutionPolicyProvider(),
            new MockOutboxIntegrationEventWriter(eventDaoLogger),
            new DddCommandExecutorLogger(commandLogger));

        // Query executor
        _queryExecutor = new DddQueryExecutor(
            contextFactory,
            new DddQueryExecutorLogger(queryLogger));
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
