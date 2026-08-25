using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookRating.Model;
using Microsoft.Extensions.Logging;

namespace Eladei.BookRating.Infrastructure.Adapters;

public class EfCommandExecutorAdapter : ICommandExecutor
{
    private readonly IEfCommandExecutor<BookRatingDbContext> _commandExecutor;
    private readonly ILogger<EfCommandExecutorAdapter> _logger;

    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="commandExecutor"/> or <paramref name="logger"/> is null.
    /// </exception>
    public EfCommandExecutorAdapter(
        IEfCommandExecutor<BookRatingDbContext> commandExecutor,
        ILogger<EfCommandExecutorAdapter> logger)
    {
        _commandExecutor = commandExecutor
            ?? throw new ArgumentNullException(nameof(commandExecutor));

        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Task ExecuteAsync(ICommand command, CancellationToken ct)
    {
        if (command is not IEfCommand<BookRatingDbContext> efCommand)
        {
            var invalidOperEx = new InvalidOperationException(
                $"{nameof(EfCommandExecutorAdapter)} supports only {nameof(IEfCommand<BookRatingDbContext>)}");

            _logger.LogCritical(invalidOperEx, invalidOperEx.Message);

            throw invalidOperEx;
        }

        return _commandExecutor.ExecuteAsync(efCommand, ct);
    }

    /// <inheritdoc/>
    public Task<R> ExecuteAsync<R>(ICommand<R> command, CancellationToken ct)
    {
        if (command is not IEfCommand<BookRatingDbContext, R> efCommand)
        {
            var invalidOperEx = new InvalidOperationException(
                $"{nameof(EfCommandExecutorAdapter)} supports only {nameof(IEfCommand<BookRatingDbContext, R>)}");

            _logger.LogCritical(invalidOperEx, invalidOperEx.Message);

            throw invalidOperEx;
        }

        return _commandExecutor.ExecuteAsync(efCommand, ct);
    }
}
