using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Ddd.DomainEvents;
using Eladei.BookInfo.Model;

namespace Eladei.BookInfo.Infrastructure.Outbox; 

/// <summary>
/// Мок службы сохранения доменных событий в outbox
/// </summary>
public sealed class MockOutboxDomainEventDao : IEfOutboxDomainEventDao<BookInfoDbContext> {
    public Task SaveAsync(IReadOnlyCollection<IDomainEvent> domainEvents, BookInfoDbContext context, CancellationToken cancellationToken)
        => Task.CompletedTask;
}