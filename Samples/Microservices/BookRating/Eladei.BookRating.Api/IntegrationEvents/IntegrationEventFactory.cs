using Eladei.Architecture.Ddd.DomainEvents;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Eladei.BookRating.Contract.Messaging.IntegrationEvents;
using Eladei.BookRating.Domain.Commands.DomainEvents;

namespace Eladei.BookRating.Api.IntegrationEvents;

/// <summary>
/// Фабрика для формирования событий интеграции
/// </summary>
public sealed class IntegrationEventFactory : IIntegrationEventFactory {
    public IIntegrationEvent? Create(IDomainEvent domainEvent, Guid correlationId)
        =>  domainEvent switch {
            BookWasRegisteredInRatingDomainEvent evnt => new BookWasRegisteredInRatingIntegrationEvent(
                evnt.BookId, correlationId, evnt.Name, evnt.Author),
            BookInfoWasUpdatedInRatingDomainEvent evnt => new BookInfoWasUpdatedInRatingIntegrationEvent(
                evnt.BookId, correlationId, evnt.Name, evnt.Author),
            BookWasRemovedFromRatingDomainEvent evnt => new BookWasRemovedFromRatingIntegrationEvent(
                evnt.BookId, correlationId),
            _ => null
        };
}