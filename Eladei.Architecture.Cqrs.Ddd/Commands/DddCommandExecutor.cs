using Eladei.Architecture.Cqrs.Commands;
using Eladei.Architecture.Cqrs.Ddd.Properties;
using Eladei.Architecture.Ddd.DomainEvents;
using Eladei.Architecture.Ddd.Entities;
using System.Diagnostics;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Command executor
/// </summary>
/// <remarks>
/// Coordinates command execution process:
/// retry policies, transactional execution, and logging.
/// To persist domain events in the database (outbox pattern) within the same transaction,
/// an implementation of <see cref="IDddOutboxDomainEventDao"/> is required
/// </remarks>
public class DddCommandExecutor : IDddCommandExecutor
{
    protected readonly IUnitOfWorkContextFactory _unitOfWorkContextFactory;
    protected readonly IOperationExecutionPolicyService _executionRetryPolicy;
    protected readonly IDddOutboxDomainEventDao _domainEventDao;
    protected readonly IDddCommandExecutorLogger? _logger;

    /// <summary>
    /// Creates a new instance of <see cref="DddCommandExecutor"/>
    /// </summary>
    /// <param name="unitOfWorkContextFactory">The unit of work context factory</param>
    /// <param name="executionPolicyService">The operation execution policy service</param>
    /// <param name="domainEventDao">The domain event persistence service</param>
    /// <param name="logger">The logger</param>
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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
    /// Delays execution before the next retry attempt
    /// </summary>
    /// <param name="currentAttempt">The current attempt number</param>
    /// <param name="maxDelayInMilliseconds">The maximum delay in milliseconds</param>
    /// <param name="cancellationToken">The cancellation token</param>
    protected virtual async Task DelayBeforeNewAttempt(
        uint currentAttempt, uint maxDelayInMilliseconds, CancellationToken cancellationToken)
    {

        uint baseDelay = Math.Min(1000 * (uint)Math.Pow(2, currentAttempt - 1), maxDelayInMilliseconds);

        // Add jitter ±20%
        var jitterFactor = 0.2;
        var jitter = (float)(Random.Shared.NextDouble() * 2 - 1) * jitterFactor; // [-0.2, +0.2]
        var delayWithJitter = baseDelay * (1 + jitter);

        // Safe cast to int (do not exceed int.MaxValue)
        var delayMs = Math.Min((int)Math.Round(delayWithJitter), int.MaxValue);

        await Task.Delay(delayMs, cancellationToken);
    }

    /// <summary>
    /// Saves domain events
    /// </summary>
    /// <param name="domainEvents">The domain events</param>
    /// <param name="repositoryFactory">The repository factory</param>
    /// <param name="cancellationToken">The cancellation token</param>
    protected virtual Task SaveDomainEvents(IReadOnlyCollection<IDomainEvent> domainEvents, IRepositoryFactory repositoryFactory, CancellationToken cancellationToken)
    {
        if (domainEvents.Any())
            return _domainEventDao.SaveAsync(domainEvents, repositoryFactory, cancellationToken);

        return Task.CompletedTask;
    }
}