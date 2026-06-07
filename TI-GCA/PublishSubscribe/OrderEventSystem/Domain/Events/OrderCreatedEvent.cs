using OrderEventSystem.Domain.Orders;
using OrderEventSystem.Messaging;

namespace OrderEventSystem.Domain.Events;

public sealed record OrderCreatedEvent(Order Order) : IEvent
{
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
