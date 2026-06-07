using System.Collections.Concurrent;
using OrderEventSystem.Domain.Events;
using OrderEventSystem.Messaging;

namespace OrderEventSystem.Services;

public abstract class OrderConsumerBase : IEventConsumer<OrderCreatedEvent>
{
    private readonly ConcurrentQueue<Guid> _processedOrderIds = new();

    public IReadOnlyCollection<Guid> ProcessedOrderIds => _processedOrderIds.ToArray();

    public Task HandleAsync(OrderCreatedEvent eventData, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        cancellationToken.ThrowIfCancellationRequested();
        _processedOrderIds.Enqueue(eventData.Order.Id);
        return Task.CompletedTask;
    }
}
