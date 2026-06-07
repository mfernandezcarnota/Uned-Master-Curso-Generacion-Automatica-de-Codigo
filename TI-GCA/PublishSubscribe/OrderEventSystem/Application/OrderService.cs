using OrderEventSystem.Domain.Events;
using OrderEventSystem.Domain.Orders;
using OrderEventSystem.Messaging;

namespace OrderEventSystem.Application;

public sealed class OrderService(IOrderRepository orderRepository, IEventBus eventBus)
{
    public async Task<Order> CreateOrderAsync(
        string customerEmail,
        IEnumerable<OrderItem> items,
        CancellationToken cancellationToken = default)
    {
        var order = Order.Create(customerEmail, items);
        await orderRepository.AddAsync(order, cancellationToken);
        await eventBus.PublishAsync(new OrderCreatedEvent(order), cancellationToken);
        return order;
    }
}
