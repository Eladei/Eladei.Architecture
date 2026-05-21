using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Cqrs.EntityFramework.Queries;
using Eladei.Architecture.Cqrs.Queries;
using Eladei.BookInfo.Model;
using Microsoft.Extensions.Logging;

namespace Eladei.BookInfo.Infrastructure.Adapters;

/// <summary>
/// Query executor adapter
/// </summary>
public sealed class EfQueryExecutorAdapter : IQueryExecutor
{
    private readonly IEfQueryExecutor<BookInfoDbContext> _queryExecutor;
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
        IEfQueryExecutor<BookInfoDbContext> queryExecutor,
        ILogger<EfQueryExecutorAdapter> logger)
    {
        _queryExecutor = queryExecutor
            ?? throw new ArgumentNullException(nameof(queryExecutor));

        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<R> ExecuteAsync<R>(IQuery<R> query, CancellationToken ct)
    {
        if (query is not IEfQuery<BookInfoDbContext, R> efQuery)
        {
            var invalidOperEx = new InvalidOperationException(
                $"{nameof(EfQueryExecutorAdapter)} supports only {nameof(IEfCommand<BookInfoDbContext, R>)}");

            _logger.LogCritical(invalidOperEx, invalidOperEx.Message);

            throw invalidOperEx;
        }

        return _queryExecutor.ExecuteAsync(efQuery, ct);
    }
}