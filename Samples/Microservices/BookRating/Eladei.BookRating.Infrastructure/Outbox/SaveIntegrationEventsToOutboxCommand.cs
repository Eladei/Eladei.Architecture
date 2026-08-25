using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.BookRating.Model;

namespace Eladei.BookRating.Infrastructure.Outbox;

public sealed class SaveIntegrationEventsToOutboxCommand : EfCommandBase<BookRatingDbContext>
{
    private readonly IOutboxMessageFactory _outboxMessageFactory;
    private readonly IReadOnlyCollection<IIntegrationEvent> _integrationEvents;

    /// <exception cref="ArgumentNullException"></exception>
    public SaveIntegrationEventsToOutboxCommand(
        IReadOnlyCollection<IIntegrationEvent> integrationEvents,
        IOutboxMessageFactory outboxMessageFactory)
    {
        _integrationEvents = integrationEvents
            ?? throw new ArgumentNullException(nameof(integrationEvents));

        _outboxMessageFactory = outboxMessageFactory
            ?? throw new ArgumentNullException(nameof(outboxMessageFactory));
    }

    /// <inheritdoc />
    public override Task ExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken)
    {
        if (_integrationEvents.Count != 0)
            context.OutboxMessages.AddRange(_integrationEvents.Select(_outboxMessageFactory.Create));

        return Task.CompletedTask;
    }
}
