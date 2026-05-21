using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Ddd.DomainEvents;
using Microsoft.Extensions.Logging;

namespace CqrsWithEntityFrameworkExecuting.Infrastructure;

/// <summary>
/// Mock service for saving domain events to the outbox
/// </summary>
public sealed class MockOutboxDomainEventDao : IEfOutboxDomainEventDao<BookRatingDbContext>
{
    private readonly ILogger<MockOutboxDomainEventDao> _logger;

    public MockOutboxDomainEventDao(ILogger<MockOutboxDomainEventDao> logger)
    {
        _logger = logger;
    }

    public Task SaveAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        BookRatingDbContext context,
        CancellationToken cancellationToken)
    {
        var eventNames = string.Join(',', domainEvents.Select(evnt => evnt.GetType().Name));

        _logger?.LogInformation("Domain events recorded: {0}", eventNames);

        return Task.CompletedTask;
    }
}