using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.BookRating.Model;

namespace Eladei.BookRating.Infrastructure.Outbox;

public sealed class OutboxIntegrationEventWriter : IEfOutboxIntegrationEventWriter<BookRatingDbContext>
{
    private readonly IOutboxMessageFactory _outboxMessageFactory;

    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="outboxMessageFactory"/> is null.
    /// </exception>
    public OutboxIntegrationEventWriter(IOutboxMessageFactory outboxMessageFactory)
    {
        _outboxMessageFactory = outboxMessageFactory
            ?? throw new ArgumentNullException(nameof(outboxMessageFactory));
    }

    /// <inheritdoc />
    public Task SaveAsync(
        IReadOnlyCollection<IIntegrationEvent> integrationEvents,
        BookRatingDbContext context,
        CancellationToken cancellationToken)
    {
        var command = new SaveIntegrationEventsToOutboxCommand(integrationEvents, _outboxMessageFactory);

        return command.ExecuteAsync(context, cancellationToken);
    }
}
