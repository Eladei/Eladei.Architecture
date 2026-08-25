using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.BookRating.Model.Entities.Outbox;

namespace Eladei.BookRating.Infrastructure.Outbox;

public interface IOutboxMessageFactory
{
    OutboxMessage Create(IIntegrationEvent integrationEvent);
}
