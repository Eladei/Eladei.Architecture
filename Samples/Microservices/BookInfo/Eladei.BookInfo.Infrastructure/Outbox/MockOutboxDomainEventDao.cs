using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Ddd.DomainEvents;
using Eladei.BookInfo.Model;

namespace Eladei.BookInfo.Infrastructure.Outbox;

/// <summary>
/// Mock implementation of a service for saving domain events to the outbox
/// </summary>
public sealed class MockOutboxDomainEventDao : IEfOutboxDomainEventDao<BookInfoDbContext>
{
    public Task SaveAsync(IReadOnlyCollection<IDomainEvent> domainEvents, BookInfoDbContext context, CancellationToken cancellationToken)
        => Task.CompletedTask;
}