using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace CqrsWithEntityFrameworkExecuting.Infrastructure;

public sealed class MockOutboxIntegrationEventWriter : IEfOutboxIntegrationEventWriter<BookRatingDbContext>
{
    private readonly ILogger<MockOutboxIntegrationEventWriter> _logger;

    public MockOutboxIntegrationEventWriter(ILogger<MockOutboxIntegrationEventWriter> logger)
    {
        _logger = logger;
    }

    public Task SaveAsync(
        IReadOnlyCollection<IIntegrationEvent> domainEvents,
        BookRatingDbContext context,
        CancellationToken cancellationToken)
    {
        var eventNames = string.Join(',', domainEvents.Select(evnt => evnt.GetType().Name));

        _logger.LogInformation("Integration events recorded: {0}", eventNames);

        return Task.CompletedTask;
    }
}
