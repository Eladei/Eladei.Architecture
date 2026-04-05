using Eladei.Architecture.Cqrs.EntityFramework.Queries;
using Eladei.Architecture.Cqrs.Queries;
using Eladei.BookRating.Model;
using Microsoft.Extensions.Logging;

namespace Eladei.BookRating.Infrastructure.Adapters;

/// <summary>
/// Адаптер исполнителя запросов
/// </summary>
public class EfQueryExecutorAdapter : IQueryExecutor {
    private readonly IEfQueryExecutor<BookRatingDbContext> _queryExecutor;
    private readonly ILogger<EfQueryExecutorAdapter> _logger;

    public EfQueryExecutorAdapter(
        IEfQueryExecutor<BookRatingDbContext> queryExecutor,
        ILogger<EfQueryExecutorAdapter> logger) {
        _queryExecutor = queryExecutor
            ?? throw new ArgumentNullException(nameof(queryExecutor));

        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<R> ExecuteAsync<R>(IQuery<R> query, CancellationToken ct) {
        if (query is not IEfQuery<BookRatingDbContext, R> efQuery) {
            var invalidOperEx = new InvalidOperationException(
                $"{nameof(EfQueryExecutorAdapter)} supports only {nameof(IEfQuery<BookRatingDbContext, R>)}");

            _logger.LogCritical(invalidOperEx, invalidOperEx.Message);

            throw invalidOperEx;
        }

        return _queryExecutor.ExecuteAsync(efQuery, ct);
    }
}