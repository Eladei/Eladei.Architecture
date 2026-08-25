using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.BookInfo.Model;

namespace Eladei.BookInfo.Infrastructure.Outbox;

public sealed class MockOutboxIntegrationEventWriter : IEfOutboxIntegrationEventWriter<BookInfoDbContext>
{
    public Task SaveAsync(IReadOnlyCollection<IIntegrationEvent> integrationEvents, BookInfoDbContext context, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
