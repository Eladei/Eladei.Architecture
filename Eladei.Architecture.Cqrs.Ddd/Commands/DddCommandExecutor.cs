using Eladei.Architecture.Cqrs.Ddd.Commands.Exceptions;
using Eladei.Architecture.Cqrs.Ddd.Properties;
using Eladei.Architecture.Messaging.IntegrationEvents;
using System.Diagnostics;

namespace Eladei.Architecture.Cqrs.Ddd.Commands;

/// <summary>
/// Command executor
/// </summary>
/// <remarks>
/// Coordinates command execution process:
/// retry policies, transactional execution, and logging.
/// To persist domain events in the database (outbox pattern) within the same transaction,
/// an implementation of <see cref="IDddOutboxIntegrationEventWriter"/> is required
/// </remarks>
public class DddCommandExecutor : IDddCommandExecutor
{
    /// <summary>
    /// The unit of work context factory
    /// </summary>
    protected readonly IUnitOfWorkContextFactory UnitOfWorkContextFactory;

    /// <summary>
    /// The operation execution policy provider
    /// </summary>
    protected readonly IOperationExecutionPolicyProvider ExecutionPolicyProvider;

    /// <summary>
    /// The domain event persistence service
    /// </summary>
    protected readonly IDddOutboxIntegrationEventWriter OutboxIntegrationEventWriter;

    /// <summary>
    /// The logger
    /// </summary>
    protected readonly IDddCommandExecutorLogger? Logger;

    /// <summary>
    /// Creates a new instance of <see cref="DddCommandExecutor"/>
    /// </summary>
    /// <param name="unitOfWorkContextFactory">The unit of work context factory</param>
    /// <param name="executionPolicyProvider">The operation execution policy provider</param>
    /// <param name="outboxIntegrationEventWriter">The integration event persistence service</param>
    /// <param name="logger">The logger</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DddCommandExecutor(
        IUnitOfWorkContextFactory unitOfWorkContextFactory,
        IOperationExecutionPolicyProvider executionPolicyProvider,
        IDddOutboxIntegrationEventWriter outboxIntegrationEventWriter,
        IDddCommandExecutorLogger? logger = null)
    {
        UnitOfWorkContextFactory = unitOfWorkContextFactory
            ?? throw new ArgumentNullException(nameof(unitOfWorkContextFactory));

        OutboxIntegrationEventWriter = outboxIntegrationEventWriter
            ?? throw new ArgumentNullException(nameof(outboxIntegrationEventWriter));

        ExecutionPolicyProvider = executionPolicyProvider
            ?? throw new ArgumentNullException(nameof(executionPolicyProvider));

        Logger = logger;
    }

    /// <inheritdoc />
    public virtual async Task ExecuteAsync(IDddCommand command, CancellationToken cancellationToken)
    {
        var commandName = command.GetType().Name;

        Logger?.ExecutingStarted(commandName);

        var commandPolicy = ExecutionPolicyProvider.GetExecutionPolicy(command);

        for (uint attempt = 1; attempt <= commandPolicy.MaxAttemptsCount; attempt++)
        {
            command.ClearEvents();

            var unitOfWorkContext = UnitOfWorkContextFactory.CreateContext();

            try
            {
                var continueExecuting = await command.BeforeExecuteAsync(unitOfWorkContext, cancellationToken);

                if (!continueExecuting)
                    return;
            }
            catch (Exception ex)
            {
                Logger?.CriticalError(commandName, ex);

                throw;
            }

            Exception foundEx;

            await unitOfWorkContext.BeginTransactionAsync(cancellationToken);

            try
            {
                await command.ExecuteAsync(unitOfWorkContext, cancellationToken);

                await AddIntegrationEvents(command.Events, unitOfWorkContext, cancellationToken);

                await unitOfWorkContext.CommitTransactionAsync(cancellationToken);

                Logger?.ExecutingSuccessfulFinished(commandName);

                return;
            }
            catch (DddCommandLogicException ex)
            {
                await unitOfWorkContext.RollbackTransactionAsync(cancellationToken);

                Logger?.CommandLogicError(commandName, ex);

                throw;
            }
            catch (OperationCanceledException ex)
            {
                await unitOfWorkContext.RollbackTransactionAsync(cancellationToken);

                foundEx = ex;

                Logger?.ExecutingCancelled(commandName, ex);

                throw;
            }
            catch (Exception ex)
            {
                await unitOfWorkContext.RollbackTransactionAsync(cancellationToken);

                foundEx = ex;

                Logger?.CriticalError(commandName, foundEx);
            }

            if (!commandPolicy.ShouldRetry(foundEx, attempt))
            {
                var errorMsg = string.Format(
                    Resources.CommandDataSourceUpdateAttemptLimitReachedError,
                    commandName,
                    commandPolicy.MaxAttemptsCount);

                var maxRetryEx = new CommandExecutionAttemptLimitReachedException(errorMsg, foundEx);

                Logger?.AttemptLimitReachedError(commandName, maxRetryEx, commandPolicy.MaxAttemptsCount);

                throw maxRetryEx;
            }

            await DelayBeforeNewAttempt(attempt, commandPolicy.MaxDelayInMilliseconds, cancellationToken);
        }
    }

    /// <inheritdoc />
    public virtual async Task<R> ExecuteAsync<R>(IDddCommand<R> command, CancellationToken cancellationToken)
    {
        var commandName = command.GetType().Name;

        Logger?.ExecutingStarted(commandName);

        var commandPolicy = ExecutionPolicyProvider.GetExecutionPolicy(command);

        for (uint attempt = 1; attempt <= commandPolicy.MaxAttemptsCount; attempt++)
        {
            command.ClearEvents();

            var unitOfWorkContext = UnitOfWorkContextFactory.CreateContext();

            try
            {
                await command.BeforeExecuteAsync(unitOfWorkContext, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger?.CriticalError(commandName, ex);

                throw;
            }

            Exception foundEx;

            await unitOfWorkContext.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await command.ExecuteAsync(unitOfWorkContext, cancellationToken);

                await AddIntegrationEvents(command.Events, unitOfWorkContext, cancellationToken);

                await unitOfWorkContext.SaveChangesAsync(cancellationToken);

                await unitOfWorkContext.CommitTransactionAsync(cancellationToken);

                Logger?.ExecutingSuccessfulFinished(commandName);

                return result;
            }
            catch (DddCommandLogicException ex)
            {
                await unitOfWorkContext.RollbackTransactionAsync(cancellationToken);

                Logger?.CommandLogicError(commandName, ex);

                throw;
            }
            catch (OperationCanceledException ex)
            {
                await unitOfWorkContext.RollbackTransactionAsync(cancellationToken);

                Logger?.ExecutingCancelled(commandName, ex);

                throw;
            }
            catch (Exception ex)
            {
                await unitOfWorkContext.RollbackTransactionAsync(cancellationToken);

                foundEx = ex;

                Logger?.CriticalError(commandName, foundEx);
            }

            if (!commandPolicy.ShouldRetry(foundEx, attempt))
            {
                var errorMsg = string.Format(
                    Resources.CommandDataSourceUpdateAttemptLimitReachedError,
                    commandName,
                    commandPolicy.MaxAttemptsCount);

                var maxRetryEx = new CommandExecutionAttemptLimitReachedException(errorMsg, foundEx);

                Logger?.AttemptLimitReachedError(commandName, maxRetryEx, commandPolicy.MaxAttemptsCount);

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
    /// Saves integration events
    /// </summary>
    /// <param name="integrationEvents">The integration events</param>
    /// <param name="repositoryFactory">The repository factory</param>
    /// <param name="cancellationToken">The cancellation token</param>
    protected virtual Task AddIntegrationEvents(IReadOnlyCollection<IIntegrationEvent> integrationEvents, IRepositoryFactory repositoryFactory, CancellationToken cancellationToken)
    {
        if (integrationEvents.Any())
            return OutboxIntegrationEventWriter.SaveAsync(integrationEvents, repositoryFactory, cancellationToken);

        return Task.CompletedTask;
    }
}
