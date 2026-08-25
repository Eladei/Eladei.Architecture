using Eladei.Architecture.Cqrs.Ddd;
using Eladei.Architecture.Cqrs.Ddd.Commands;
using Eladei.Architecture.Messaging.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace CqrsWithDddExecuting.Infrastructure;

public sealed class MockOutboxIntegrationEventWriter : IDddOutboxIntegrationEventWriter
{
    private readonly ILogger<MockOutboxIntegrationEventWriter> _logger;

    public MockOutboxIntegrationEventWriter(ILogger<MockOutboxIntegrationEventWriter> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task SaveAsync(
        IReadOnlyCollection<IIntegrationEvent> integrationEvents, 
        IRepositoryFactory repositoryFactory, 
        CancellationToken cancellationToken)
    {
        var eventNames = string.Join(',', integrationEvents.Select(evnt => evnt.GetType().Name));

        _logger.LogInformation("Integration events recorded: {0}", eventNames);

        return Task.CompletedTask;
    }
}
