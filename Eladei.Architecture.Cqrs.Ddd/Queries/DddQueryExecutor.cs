using Eladei.Architecture.Cqrs.Ddd.Properties;
using Eladei.Architecture.Cqrs.Queries;

namespace Eladei.Architecture.Cqrs.Ddd.Queries;

/// <summary>
/// Исполнитель запроса, работающей с Entity Framework
/// </summary>
/// <typeparam name="T">Контекст данных</typeparam>
public class DddQueryExecutor : IDddQueryExecutor
{
    protected readonly IUnitOfWorkContextFactory _unitOfWorkContextFactory;
    protected readonly IDddQueryExecutorLogger? _logger;

    /// <summary>
    /// Создает объект класса EfQueryExecutor
    /// </summary>
    /// <param name="unitOfWorkContextFactory">Фабрика контекста данных</param>
    /// <param name="logger">Логгер</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DddQueryExecutor(IUnitOfWorkContextFactory unitOfWorkContextFactory, IDddQueryExecutorLogger? logger = null)
    {
        _unitOfWorkContextFactory = unitOfWorkContextFactory
            ?? throw new ArgumentNullException(nameof(unitOfWorkContextFactory));

        _logger = logger;
    }

    public virtual async Task<R> ExecuteAsync<R>(IDddQuery<R> query, CancellationToken cancellationToken)
    {
        var queryName = query.GetType().Name;

        _logger?.ExecutingStarted(queryName);

        var unitOfWork = _unitOfWorkContextFactory.CreateContext();

        try
        {
            var result = await query.ExecuteAsync(unitOfWork, cancellationToken);

            _logger?.ExecutingSuccessfulFinished(queryName);

            return result;
        }
        catch (OperationCanceledException ex)
        {
            _logger?.ExecutingCancelled(queryName, ex);

            throw;
        }
        catch (Exception ex)
        {
            var unknownEx = new QueryExecutingErrorException(Resources.QueryExecutingError, ex);

            _logger?.CriticalError(queryName, unknownEx);

            throw unknownEx;
        }
    }
}