using System.Collections.Concurrent;
using OrderEventSystem.Application;
using OrderEventSystem.Domain.Orders;

namespace OrderEventSystem.Infrastructure;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(order);
        cancellationToken.ThrowIfCancellationRequested();

        if (!_orders.TryAdd(order.Id, order))
        {
            throw new InvalidOperationException($"Ya existe el pedido {order.Id}.");
        }

        return Task.CompletedTask;
    }

    public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _orders.TryGetValue(orderId, out var order);
        return Task.FromResult(order);
    }
}
