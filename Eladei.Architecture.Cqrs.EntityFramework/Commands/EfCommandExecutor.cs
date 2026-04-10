using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Cqrs.EntityFramework.Properties;
using Eladei.Architecture.Ddd.DomainEvents;
using Eladei.Architecture.Ddd.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Исполнитель команды, работающий с Entity Framework
/// </summary>
/// <remarks>Координирует процесс обработки команд: 
/// повторные попытки на основе политик; сохранение результатов в одной транзакции; логирование.
/// Для сохранение событий в БД (outbox) в одной транзакции с результатами работы команды 
/// необходима реализация интерфейса IEfOutboxDomainEventDao
/// </remarks>
/// <typeparam name="T">Контекст данных</typeparam>
public class EfCommandExecutor<T> : IEfCommandExecutor<T> where T : DbContext
{
    protected readonly IDbContextFactory<T> _contextFactory;
    protected readonly IOperationExecutionPolicyService _executionRetryPolicy;
    protected readonly IEfOutboxDomainEventDao<T> _domainEventDao;
    protected readonly IEfCommandExecutorLogger? _logger;

    /// <summary>
    /// Создает объект класса EfCommandExecutor
    /// </summary>
    /// <param name="contextFactory">Фабрика контекста данных</param>
    /// <param name="executionPolicyService">Служба запроса политики выполнения операции</param>
    /// <param name="domainEventDao">Служба для сохранения событий предметной области</param>
    /// <param name="logger">Логгер</param>
    /// <exception cref="ArgumentNullException"></exception>
    public EfCommandExecutor(
        IDbContextFactory<T> contextFactory,
        IOperationExecutionPolicyService executionPolicyService,
        IEfOutboxDomainEventDao<T> domainEventDao,
        IEfCommandExecutorLogger? logger = null)
    {
        _contextFactory = contextFactory
            ?? throw new ArgumentNullException(nameof(contextFactory));

        _domainEventDao = domainEventDao
            ?? throw new ArgumentNullException(nameof(domainEventDao));

        _executionRetryPolicy = executionPolicyService
            ?? throw new ArgumentNullException(nameof(executionPolicyService));

        _logger = logger;
    }

    public virtual async Task ExecuteAsync(IEfCommand<T> command, CancellationToken cancellationToken)
    {
        var commandName = command.GetType().Name;

        _logger?.ExecutingStarted(commandName);

        var commandPolicy = _executionRetryPolicy.GetExecutionPolicy(command);

        for (uint attempt = 1; attempt <= commandPolicy.MaxAttemptsCount; attempt++)
        {
            command.ClearEvents();

            using var context = await CreateDbContextAsync(commandName, cancellationToken);

            try
            {
                var continueExecuting = await command.BeforeExecuteAsync(context, cancellationToken);

                if (!continueExecuting)
                    return;
            }
            catch (Exception ex)
            {
                _logger?.CriticalError(commandName, ex);

                throw;
            }

            Exception foundEx;

            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await command.ExecuteAsync(context, cancellationToken);

                await SaveDomainEvents(command.Events, context, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                _logger?.ExecutingSuccessfulFinished(commandName);

                return;
            }
            catch (DomainLogicException ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                _logger?.DomainLogicError(commandName, ex);

                throw;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                foundEx = ex;

                await HandleDbUpdateException(
                    commandName, ex, attempt, commandPolicy.MaxAttemptsCount, cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                foundEx = ex;

                _logger?.ExecutingCancelled(commandName, ex);

                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                foundEx = ex;

                _logger?.CriticalError(commandName, foundEx);
            }

            if (!commandPolicy.ShouldRetry(foundEx, attempt))
            {
                var errorMsg = string.Format(
                    Resources.CommandDbUpdateAttemptLimitReachedError,
                    commandName,
                    commandPolicy.MaxAttemptsCount);

                var maxRetryEx = new CommandExecutionAttemptLimitReachedException(errorMsg, foundEx);

                _logger?.AttemptLimitReachedError(commandName, maxRetryEx, commandPolicy.MaxAttemptsCount);

                throw maxRetryEx;
            }

            await DelayBeforeNewAttempt(attempt, commandPolicy.MaxDelayInMilliseconds, cancellationToken);
        }
    }

    public virtual async Task<R> ExecuteAsync<R>(IEfCommand<T, R> command, CancellationToken cancellationToken)
    {
        var commandName = command.GetType().Name;

        _logger?.ExecutingStarted(commandName);

        var commandPolicy = _executionRetryPolicy.GetExecutionPolicy(command);

        for (uint attempt = 1; attempt <= commandPolicy.MaxAttemptsCount; attempt++)
        {
            command.ClearEvents();

            using var context = await CreateDbContextAsync(commandName, cancellationToken);

            try
            {
                await command.BeforeExecuteAsync(context, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger?.CriticalError(commandName, ex);

                throw;
            }

            Exception foundEx;

            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await command.ExecuteAsync(context, cancellationToken);

                await SaveDomainEvents(command.Events, context, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                _logger?.ExecutingSuccessfulFinished(commandName);

                return result;
            }
            catch (DomainLogicException ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                _logger?.DomainLogicError(commandName, ex);

                throw;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                foundEx = ex;

                await HandleDbUpdateException(
                    commandName, ex, attempt, commandPolicy.MaxAttemptsCount, cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                _logger?.ExecutingCancelled(commandName, ex);

                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                foundEx = ex;

                _logger?.CriticalError(commandName, foundEx);
            }

            if (!commandPolicy.ShouldRetry(foundEx, attempt))
            {
                var errorMsg = string.Format(
                    Resources.CommandDbUpdateAttemptLimitReachedError,
                    commandName,
                    commandPolicy.MaxAttemptsCount);

                var maxRetryEx = new CommandExecutionAttemptLimitReachedException(errorMsg, foundEx);

                _logger?.AttemptLimitReachedError(commandName, maxRetryEx, commandPolicy.MaxAttemptsCount);

                throw maxRetryEx;
            }

            await DelayBeforeNewAttempt(attempt, commandPolicy.MaxDelayInMilliseconds, cancellationToken);
        }

        throw new UnreachableException(Resources.UnreachableCodeError);
    }

    protected virtual async Task<T> CreateDbContextAsync(string commandName, CancellationToken cancellationToken)
    {
        T context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        if (context is null)
        {
            var invalidOperEx = new InvalidOperationException(Resources.CantCreateDbContext);

            _logger?.CriticalError(commandName, invalidOperEx);

            throw invalidOperEx;
        }

        return context;
    }

    /// <summary>
    /// Обрабатывает ошибку конкуретного обновления БД
    /// </summary>
    /// <param name="commandName">Название команды</param>
    /// <param name="ex">Исключение конкуретного обновления БД</param>
    /// <param name="attempt">Число попыток обновления БД</param>
    /// <param name="maxAttemptsCount">Максимальное число попыток обновление БД</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <exception cref="DbModifiedObjectWasRemovedException"></exception>
    /// <exception cref="DbRemovingObjectWasRemovedException"></exception>
    /// <exception cref="DbUnknownEntityStateException"></exception>
    protected virtual async Task HandleDbUpdateException(
        string commandName, DbUpdateException ex,
        uint attempt, uint maxAttemptsCount, CancellationToken cancellationToken)
    {
        foreach (var entry in ex.Entries)
        {
            var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);

            // Выбросить исключение, если изменяемый или удаляемый объект уже был удален,
            // или если добавляемый объект уже был добавлен
            if (databaseValues == null)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Modified:
                        var modifObjEx = new DbModifiedObjectWasRemovedException(Resources.ModifiedObjectWasRemoved, ex);

                        _logger?.CriticalError(commandName, modifObjEx);

                        throw modifObjEx;
                    case EntityState.Deleted:
                        var removingObjEx = new DbRemovingObjectWasRemovedException(Resources.RemovingObjectWasAlreadyRemoved, ex);

                        _logger?.CriticalError(commandName, removingObjEx);

                        throw removingObjEx;
                    case EntityState.Detached:
                        continue;
                    default:
                        var unknownStateEx = new DbUnknownEntityStateException(Resources.UnknownDbEntityState, ex);

                        _logger?.CriticalError(commandName, unknownStateEx);

                        throw unknownStateEx;
                }
            }
        }

        _logger?.UpdateError(commandName, ex, attempt, maxAttemptsCount);
    }

    /// <summary>
    /// Сделать задержку перед новой попыткой выполнения команды
    /// </summary>
    /// <param name="currentAttempt">Текущая попытка</param>
    /// <param name="maxDelayInMilliseconds">Максимальная величина задержки в миллисекундах</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    protected virtual async Task DelayBeforeNewAttempt(
        uint currentAttempt, uint maxDelayInMilliseconds, CancellationToken cancellationToken)
    {

        uint baseDelay = Math.Min(1000 * (uint)Math.Pow(2, currentAttempt - 1), maxDelayInMilliseconds);

        // Добавляем jitter ±20%
        var jitterFactor = 0.2;
        var jitter = (float)(Random.Shared.NextDouble() * 2 - 1) * jitterFactor; // [-0.2, +0.2]
        var delayWithJitter = baseDelay * (1 + jitter);

        // Безопасное приведение к int (не превышаем int.MaxValue)
        var delayMs = Math.Min((int)Math.Round(delayWithJitter), int.MaxValue);

        await Task.Delay(delayMs, cancellationToken);
    }

    protected virtual Task SaveDomainEvents(IReadOnlyCollection<IDomainEvent> domainEvents, T context, CancellationToken cancellationToken)
    {
        if (domainEvents.Any())
            return _domainEventDao.SaveAsync(domainEvents, context, cancellationToken);

        return Task.CompletedTask;
    }
}