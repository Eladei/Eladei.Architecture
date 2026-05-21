using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookInfo.Model;
using Microsoft.Extensions.Logging;

namespace Eladei.BookInfo.Infrastructure.Adapters;

/// <summary>
/// Command executor adapter
/// </summary>
public sealed class EfCommandExecutorAdapter : ICommandExecutor
{
    private readonly IEfCommandExecutor<BookInfoDbContext> _commandExecutor;
    private readonly ILogger<EfCommandExecutorAdapter> _logger;

    /// <summary>
    /// Creates an instance of the <see cref="EfCommandExecutorAdapter"/> class
    /// </summary>
    /// <remarks>
    /// This adapter wraps an EF-based command executor and adapts it to the
    /// generic CQRS <see cref="ICommandExecutor"/> abstraction.
    /// </remarks>
    /// <param name="commandExecutor">Entity Framework command executor</param>
    /// <param name="logger">Logger</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="commandExecutor"/> or <paramref name="logger"/> is null.
    /// </exception>
    public EfCommandExecutorAdapter(
        IEfCommandExecutor<BookInfoDbContext> commandExecutor,
        ILogger<EfCommandExecutorAdapter> logger)
    {
        _commandExecutor = commandExecutor
            ?? throw new ArgumentNullException(nameof(commandExecutor));

        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public Task ExecuteAsync(ICommand command, CancellationToken ct)
    {
        if (command is not IEfCommand<BookInfoDbContext> efCommand)
        {
            var invalidOperEx = new InvalidOperationException(
                $"{nameof(EfCommandExecutorAdapter)} supports only {nameof(IEfCommand<BookInfoDbContext>)}");

            _logger.LogCritical(invalidOperEx, invalidOperEx.Message);

            throw invalidOperEx;
        }

        return _commandExecutor.ExecuteAsync(efCommand, ct);
    }

    /// <inheritdoc />
    public Task<R> ExecuteAsync<R>(ICommand<R> command, CancellationToken ct)
    {
        if (command is not IEfCommand<BookInfoDbContext, R> efCommand)
        {
            var invalidOperEx = new InvalidOperationException(
                $"{nameof(EfCommandExecutorAdapter)} supports only {nameof(IEfCommand<BookInfoDbContext, R>)}");

            _logger.LogCritical(invalidOperEx, invalidOperEx.Message);

            throw invalidOperEx;
        }

        return _commandExecutor.ExecuteAsync(efCommand, ct);
    }
}