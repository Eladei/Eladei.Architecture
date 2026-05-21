using Eladei.Architecture.Cqrs.Ddd;
using Eladei.Architecture.Cqrs.Ddd.Commands;
using Eladei.Architecture.Ddd.DomainEvents;
using Microsoft.Extensions.Logging;

namespace CqrsWithDddExecuting.Infrastructure;

/// <summary>
/// Mock of outbox domain event DAO
/// </summary>
public sealed class MockOutboxDomainEventDao : IDddOutboxDomainEventDao
{
    private readonly ILogger<MockOutboxDomainEventDao> _logger;

    public MockOutboxDomainEventDao(ILogger<MockOutboxDomainEventDao> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task SaveAsync(IReadOnlyCollection<IDomainEvent> domainEvents, IRepositoryFactory repositoryFactory, CancellationToken cancellationToken)
    {
        var eventNames = string.Join(',', domainEvents.Select(evnt => evnt.GetType().Name));

        _logger?.LogInformation("Зафиксированы доменные события {0}", eventNames);

        return Task.CompletedTask;
    }
}