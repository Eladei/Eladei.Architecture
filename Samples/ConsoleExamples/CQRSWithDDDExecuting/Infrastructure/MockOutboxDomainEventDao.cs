using Eladei.Architecture.Cqrs.Ddd;
using Eladei.Architecture.Cqrs.Ddd.Commands;
using Eladei.Architecture.Ddd.DomainEvents;
using Microsoft.Extensions.Logging;

namespace CqrsWithDddExecuting.Infrastructure;

/// <summary>
/// Мок службы сохранения доменных событий
/// </summary>
public sealed class MockOutboxDomainEventDao : IDddOutboxDomainEventDao {
    private readonly ILogger<MockOutboxDomainEventDao> _logger;

    public MockOutboxDomainEventDao(ILogger<MockOutboxDomainEventDao> logger) {
        _logger = logger;
    }

    public Task SaveAsync(IReadOnlyCollection<IDomainEvent> domainEvents, IRepositoryFactory repositoryFactory, CancellationToken cancellationToken) {
        var eventNames = string.Join(',', domainEvents.Select(evnt => evnt.GetType().Name));

        _logger?.LogInformation("Зафиксированы доменные события {0}", eventNames);

        return Task.CompletedTask;
    }
}