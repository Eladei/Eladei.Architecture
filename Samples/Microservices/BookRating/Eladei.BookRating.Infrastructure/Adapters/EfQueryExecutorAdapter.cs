using Eladei.Architecture.Cqrs.EntityFramework.Queries;
using Eladei.Architecture.Cqrs.Queries;
using Eladei.BookRating.Model;
using Microsoft.Extensions.Logging;

namespace Eladei.BookRating.Infrastructure.Adapters;

/// <summary>
/// Query executor adapter
/// </summary>
public class EfQueryExecutorAdapter : IQueryExecutor
{
    private readonly IEfQueryExecutor<BookRatingDbContext> _queryExecutor;
    private readonly ILogger<EfQueryExecutorAdapter> _logger;

    /// <summary>
    /// Creates an instance of the <see cref="EfQueryExecutorAdapter"/> class
    /// </summary>
    /// <remarks>
    /// This adapter wraps an EF-based query executor and adapts it to the
    /// generic CQRS <see cref="IQueryExecutor"/> abstraction.
    /// </remarks>
    /// <param name="queryExecutor">Query executor</param>
    /// <param name="logger">Logger</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="queryExecutor"/> or <paramref name="logger"/> is null.
    /// </exception>
    public EfQueryExecutorAdapter(
        IEfQueryExecutor<BookRatingDbContext> queryExecutor,
        ILogger<EfQueryExecutorAdapter> logger)
    {
        _queryExecutor = queryExecutor
            ?? throw new ArgumentNullException(nameof(queryExecutor));

        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public Task<R> ExecuteAsync<R>(IQuery<R> query, CancellationToken ct)
    {
        if (query is not IEfQuery<BookRatingDbContext, R> efQuery)
        {
            var invalidOperEx = new InvalidOperationException(
                $"{nameof(EfQueryExecutorAdapter)} supports only {nameof(IEfQuery<BookRatingDbContext, R>)}");

            _logger.LogCritical(invalidOperEx, invalidOperEx.Message);

            throw invalidOperEx;
        }

        return _queryExecutor.ExecuteAsync(efQuery, ct);
    }
}