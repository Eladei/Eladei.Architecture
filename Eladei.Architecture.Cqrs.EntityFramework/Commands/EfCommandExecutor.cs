using Eladei.Architecture.Cqrs.EntityFramework.Commands.Exceptions;
using Eladei.Architecture.Cqrs.EntityFramework.Properties;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Eladei.Architecture.Cqrs.EntityFramework.Commands;

/// <summary>
/// Command executor for working with Entity Framework
/// </summary>
/// <remarks>
/// Coordinates command execution process:
/// retry policies, transactional execution, and logging.
/// To persist integration events in the database (outbox pattern) within the same transaction,
/// an implementation of <see cref="IEfOutboxIntegrationEventWriter{T}"/> is required
/// </remarks>
/// <typeparam name="T">The database context type</typeparam>
public class EfCommandExecutor<T> : IEfCommandExecutor<T> where T : DbContext
{
    /// <summary>
    /// The database context factory
    /// </summary>
    protected readonly IDbContextFactory<T> ContextFactory;

    /// <summary>
    /// The operation execution policy provider
    /// </summary>
    protected readonly IOperationExecutionPolicyProvider OperationExecutionPolicyProvider;

    /// <summary>
    /// The integration event outbox storage
    /// </summary>
    protected readonly IEfOutboxIntegrationEventWriter<T> IntegrationEventWriter;

    /// <summary>
    /// The logger
    /// </summary>
    protected readonly IEfCommandExecutorLogger? Logger;

    /// <summary>
    /// Creates an instance of the EF command executor
    /// </summary>
    /// <param name="contextFactory">The database context factory</param>
    /// <param name="operationExecutionPolicyProvider">The operation execution policy provider</param>
    /// <param name="integrationEventDao">The integration event outbox storage</param>
    /// <param name="logger">The logger</param>
    public EfCommandExecutor(
        IDbContextFactory<T> contextFactory,
        IOperationExecutionPolicyProvider operationExecutionPolicyProvider,
        IEfOutboxIntegrationEventWriter<T> integrationEventDao,
        IEfCommandExecutorLogger? logger = null)
    {
        ContextFactory = contextFactory
            ?? throw new ArgumentNullException(nameof(contextFactory));

        IntegrationEventWriter = integrationEventDao
            ?? throw new ArgumentNullException(nameof(integrationEventDao));

        OperationExecutionPolicyProvider = operationExecutionPolicyProvider
            ?? throw new ArgumentNullException(nameof(operationExecutionPolicyProvider));

        Logger = logger;
    }

    /// <inheritdoc />
    public virtual async Task ExecuteAsync(IEfCommand<T> command, CancellationToken cancellationToken)
    {
        var commandName = command.GetType().Name;

        Logger?.ExecutionStarted(commandName);

        var commandPolicy = OperationExecutionPolicyProvider.GetExecutionPolicy(command);

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
            catch (EfCommandLogicException ex)
            {
                Logger?.CommandLogicError(commandName, ex);

                throw;
            }
            catch (Exception ex)
            {
                Logger?.CriticalError(commandName, ex);

                throw;
            }

            Exception foundEx;

            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await command.ExecuteAsync(context, cancellationToken);

                await SaveIntegrationEvents(command.Events, context, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                Logger?.ExecutionSucceeded(commandName);

                return;
            }
            catch (EfCommandLogicException ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                Logger?.CommandLogicError(commandName, ex);

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

                Logger?.ExecutionCancelled(commandName, ex);

                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                foundEx = ex;

                Logger?.CriticalError(commandName, foundEx);
            }

            if (!commandPolicy.ShouldRetry(foundEx, attempt))
            {
                var errorMsg = string.Format(
                    Resources.CommandDbUpdateAttemptLimitReachedError,
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
    public virtual async Task<R> ExecuteAsync<R>(IEfCommand<T, R> command, CancellationToken cancellationToken)
    {
        var commandName = command.GetType().Name;

        Logger?.ExecutionStarted(commandName);

        var commandPolicy = OperationExecutionPolicyProvider.GetExecutionPolicy(command);

        for (uint attempt = 1; attempt <= commandPolicy.MaxAttemptsCount; attempt++)
        {
            command.ClearEvents();

            using var context = await CreateDbContextAsync(commandName, cancellationToken);
            
            try
            {
                await command.BeforeExecuteAsync(context, cancellationToken);
            }
            catch (EfCommandLogicException ex)
            {
                Logger?.CommandLogicError(commandName, ex);

                throw;
            }
            catch (Exception ex)
            {
                Logger?.CriticalError(commandName, ex);

                throw;
            }

            Exception foundEx;

            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await command.ExecuteAsync(context, cancellationToken);

                await SaveIntegrationEvents(command.Events, context, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                Logger?.ExecutionSucceeded(commandName);

                return result;
            }
            catch (EfCommandLogicException ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                Logger?.CommandLogicError(commandName, ex);

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

                Logger?.ExecutionCancelled(commandName, ex);

                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                foundEx = ex;

                Logger?.CriticalError(commandName, foundEx);
            }

            if (!commandPolicy.ShouldRetry(foundEx, attempt))
            {
                var errorMsg = string.Format(
                    Resources.CommandDbUpdateAttemptLimitReachedError,
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
    /// Creates a database context
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The database context instance</returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected virtual async Task<T> CreateDbContextAsync(string commandName, CancellationToken cancellationToken)
    {
        T context = await ContextFactory.CreateDbContextAsync(cancellationToken);
        
        if (context is null)
        {
            var invalidOperEx = new InvalidOperationException(Resources.CantCreateDbContext);

            Logger?.CriticalError(commandName, invalidOperEx);

            throw invalidOperEx;
        }

        return context;
    }

    /// <summary>
    /// Handles database concurrency update exceptions
    /// </summary>
    /// <param name="commandName">The command name</param>
    /// <param name="ex">The database update exception</param>
    /// <param name="attempt">The current retry attempt</param>
    /// <param name="maxAttemptsCount">The maximum number of retry attempts</param>
    /// <param name="cancellationToken">The cancellation token</param>
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

            // Throw an exception if the modified or deleted object has already been removed,
            // or if the added object has already been added
            if (databaseValues == null)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Modified:
                        var modifObjEx = new DbModifiedObjectWasRemovedException(Resources.ModifiedObjectWasRemoved, ex);

                        Logger?.CriticalError(commandName, modifObjEx);

                        throw modifObjEx;
                    case EntityState.Deleted:
                        var removingObjEx = new DbRemovingObjectWasRemovedException(Resources.RemovingObjectWasAlreadyRemoved, ex);

                        Logger?.CriticalError(commandName, removingObjEx);

                        throw removingObjEx;
                    case EntityState.Detached:
                        continue;
                    default:
                        var unknownStateEx = new DbUnknownEntityStateException(Resources.UnknownDbEntityState, ex);

                        Logger?.CriticalError(commandName, unknownStateEx);

                        throw unknownStateEx;
                }
            }
        }

        Logger?.UpdateError(commandName, ex, attempt, maxAttemptsCount);
    }

    /// <summary>
    /// Calculates and applies a delay before the next retry attempt using exponential backoff with jitter
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
    /// Saves integration events using the outbox storage mechanism
    /// </summary>
    /// <param name="integrationEvents">The intogration events to persist</param>
    /// <param name="context">The database context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    protected virtual Task SaveIntegrationEvents(IReadOnlyCollection<IIntegrationEvent> integrationEvents, T context, CancellationToken cancellationToken)
    {
        if (integrationEvents.Any())
            return IntegrationEventWriter.SaveAsync(integrationEvents, context, cancellationToken);

        return Task.CompletedTask;
    }
}
