using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Cqrs.Ddd.Properties;
using Eladei.Architecture.Ddd.DomainEvents;
using Eladei.Architecture.Ddd.Entities;
using System.Diagnostics;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Исполнитель команд
/// </summary>
/// <remarks>Координирует процесс обработки команд: 
/// повторные попытки на основе политик; сохранение результатов в одной транзакции; логирование.
/// Для сохранение событий в БД (outbox) в одной транзакции с результатами работы команды 
/// необходима реализация интерфейса IDddOutboxDomainEventDao
/// </remarks>
public class DddCommandExecutor : IDddCommandExecutor
{
    protected readonly IUnitOfWorkContextFactory _unitOfWorkContextFactory;
    protected readonly IOperationExecutionPolicyService _executionRetryPolicy;
    protected readonly IDddOutboxDomainEventDao _domainEventDao;
    protected readonly IDddCommandExecutorLogger? _logger;

    /// <summary>
    /// Создает объект класса EfCommandExecutor
    /// </summary>
    /// <param name="unitOfWorkContextFactory">Фабрика контекста данных</param>
    /// <param name="executionPolicyService">Служба запроса политики выполнения операции</param>
    /// <param name="domainEventDao">Служба для сохранения событий предметной области</param>
    /// <param name="logger">Логгер</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DddCommandExecutor(
        IUnitOfWorkContextFactory unitOfWorkContextFactory,
        IOperationExecutionPolicyService executionPolicyService,
        IDddOutboxDomainEventDao domainEventDao,
        IDddCommandExecutorLogger? logger = null)
    {
        _unitOfWorkContextFactory = unitOfWorkContextFactory
            ?? throw new ArgumentNullException(nameof(unitOfWorkContextFactory));

        _domainEventDao = domainEventDao
            ?? throw new ArgumentNullException(nameof(domainEventDao));

        _executionRetryPolicy = executionPolicyService
            ?? throw new ArgumentNullException(nameof(executionPolicyService));

        _logger = logger;
    }

    public virtual async Task ExecuteAsync(IDddCommand command, CancellationToken cancellationToken)
    {
        var commandName = command.GetType().Name;

        _logger?.ExecutingStarted(commandName);

        var commandPolicy = _executionRetryPolicy.GetExecutionPolicy(command);

        for (uint attempt = 1; attempt <= commandPolicy.MaxAttemptsCount; attempt++)
        {
            command.ClearEvents();

            var unitOfWorkContext = _unitOfWorkContextFactory.CreateContext();

            try
            {
                var continueExecuting = await command.BeforeExecuteAsync(unitOfWorkContext, cancellationToken);

                if (!continueExecuting)
                    return;
            }
            catch (Exception ex)
            {
                _logger?.CriticalError(commandName, ex);

                throw;
            }

            Exception foundEx;

            await unitOfWorkContext.BeginTransactionAsync(cancellationToken);

            try
            {
                await command.ExecuteAsync(unitOfWorkContext, cancellationToken);

                await SaveDomainEvents(command.Events, unitOfWorkContext, cancellationToken);

                await unitOfWorkContext.CommitTransactionAsync(cancellationToken);

                _logger?.ExecutingSuccessfulFinished(commandName);

                return;
            }
            catch (DomainLogicException ex)
            {
                await unitOfWorkContext.RollbackTransactionAsync(cancellationToken);

                _logger?.DomainLogicError(commandName, ex);

                throw;
            }
            catch (OperationCanceledException ex)
            {
                await unitOfWorkContext.RollbackTransactionAsync(cancellationToken);

                foundEx = ex;

                _logger?.ExecutingCancelled(commandName, ex);

                throw;
            }
            catch (Exception ex)
            {
                await unitOfWorkContext.RollbackTransactionAsync(cancellationToken);

                foundEx = ex;

                _logger?.CriticalError(commandName, foundEx);
            }

            if (!commandPolicy.ShouldRetry(foundEx, attempt))
            {
                var errorMsg = string.Format(
                    Resources.CommandDataSourceUpdateAttemptLimitReachedError,
                    commandName,
                    commandPolicy.MaxAttemptsCount);

                var maxRetryEx = new CommandExecutionAttemptLimitReachedException(errorMsg, foundEx);

                _logger?.AttemptLimitReachedError(commandName, maxRetryEx, commandPolicy.MaxAttemptsCount);

                throw maxRetryEx;
            }

            await DelayBeforeNewAttempt(attempt, commandPolicy.MaxDelayInMilliseconds, cancellationToken);
        }
    }

    public virtual async Task<R> ExecuteAsync<R>(IDddCommand<R> command, CancellationToken cancellationToken)
    {
        var commandName = command.GetType().Name;

        _logger?.ExecutingStarted(commandName);

        var commandPolicy = _executionRetryPolicy.GetExecutionPolicy(command);

        for (uint attempt = 1; attempt <= commandPolicy.MaxAttemptsCount; attempt++)
        {
            command.ClearEvents();

            var unitOfWorkContext = _unitOfWorkContextFactory.CreateContext();

            try
            {
                await command.BeforeExecuteAsync(unitOfWorkContext, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger?.CriticalError(commandName, ex);

                throw;
            }

            Exception foundEx;

            await unitOfWorkContext.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await command.ExecuteAsync(unitOfWorkContext, cancellationToken);

                await SaveDomainEvents(command.Events, unitOfWorkContext, cancellationToken);

                await unitOfWorkContext.SaveChangesAsync(cancellationToken);

                await unitOfWorkContext.CommitTransactionAsync(cancellationToken);

                _logger?.ExecutingSuccessfulFinished(commandName);

                return result;
            }
            catch (DomainLogicException ex)
            {
                await unitOfWorkContext.RollbackTransactionAsync(cancellationToken);

                _logger?.DomainLogicError(commandName, ex);

                throw;
            }
            catch (OperationCanceledException ex)
            {
                await unitOfWorkContext.RollbackTransactionAsync(cancellationToken);

                _logger?.ExecutingCancelled(commandName, ex);

                throw;
            }
            catch (Exception ex)
            {
                await unitOfWorkContext.RollbackTransactionAsync(cancellationToken);

                foundEx = ex;

                _logger?.CriticalError(commandName, foundEx);
            }

            if (!commandPolicy.ShouldRetry(foundEx, attempt))
            {
                var errorMsg = string.Format(
                    Resources.CommandDataSourceUpdateAttemptLimitReachedError,
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

    protected virtual Task SaveDomainEvents(IReadOnlyCollection<IDomainEvent> domainEvents, IRepositoryFactory repositoryFactory, CancellationToken cancellationToken)
    {
        if (domainEvents.Any())
            return _domainEventDao.SaveAsync(domainEvents, repositoryFactory, cancellationToken);

        return Task.CompletedTask;
    }
}