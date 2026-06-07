using OrderEventSystem.Application;
using OrderEventSystem.Domain.Events;
using OrderEventSystem.Domain.Orders;
using OrderEventSystem.Infrastructure;
using OrderEventSystem.Messaging;
using OrderEventSystem.Services;

namespace OrderEventSystem.Tests;

public sealed class OrderEventSystemTests
{
    [Fact]
    public async Task CreateOrder_PersistsAndReturnsNewOrder()
    {
        var repository = new InMemoryOrderRepository();
        var service = new OrderService(repository, new InMemoryEventBus());

        var order = await service.CreateOrderAsync(
            "ana@example.com",
            [new OrderItem("PRODUCTO-1", 2, 12.50m)]);

        var persistedOrder = await repository.GetByIdAsync(order.Id);
        Assert.Same(order, persistedOrder);
        Assert.Equal(25m, order.Total);
    }

    [Fact]
    public async Task CreateOrder_NotifiesSubscribedConsumer()
    {
        var eventBus = new InMemoryEventBus();
        var inventory = new InventoryService();
        eventBus.Subscribe<OrderCreatedEvent>(inventory);
        var service = new OrderService(new InMemoryOrderRepository(), eventBus);

        var order = await service.CreateOrderAsync("ana@example.com", SampleItems());

        Assert.Contains(order.Id, inventory.ProcessedOrderIds);
    }

    [Fact]
    public async Task CreateOrder_DistributesEventToEveryInitialService()
    {
        var eventBus = new InMemoryEventBus();
        var consumers = new OrderConsumerBase[]
        {
            new InventoryService(),
            new BillingService(),
            new LogisticsService(),
            new NotificationService()
        };
        foreach (var consumer in consumers)
        {
            eventBus.Subscribe<OrderCreatedEvent>(consumer);
        }

        var service = new OrderService(new InMemoryOrderRepository(), eventBus);
        var order = await service.CreateOrderAsync("ana@example.com", SampleItems());

        Assert.All(consumers, consumer => Assert.Contains(order.Id, consumer.ProcessedOrderIds));
    }

    [Fact]
    public async Task Subscribe_AddsNewConsumerWithoutChangingOrderService()
    {
        var eventBus = new InMemoryEventBus();
        var analytics = new AnalyticsConsumer();
        eventBus.Subscribe<OrderCreatedEvent>(analytics);
        var service = new OrderService(new InMemoryOrderRepository(), eventBus);

        var order = await service.CreateOrderAsync("ana@example.com", SampleItems());

        Assert.Equal(order.Id, analytics.LastOrderId);
    }

    [Fact]
    public async Task Unsubscribe_PreventsConsumerFromReceivingFutureEvents()
    {
        var eventBus = new InMemoryEventBus();
        var notifications = new NotificationService();
        eventBus.Subscribe<OrderCreatedEvent>(notifications);
        Assert.True(eventBus.Unsubscribe<OrderCreatedEvent>(notifications));
        var service = new OrderService(new InMemoryOrderRepository(), eventBus);

        await service.CreateOrderAsync("ana@example.com", SampleItems());

        Assert.Empty(notifications.ProcessedOrderIds);
    }

    private static OrderItem[] SampleItems() => [new("PRODUCTO-1", 1, 10m)];

    private sealed class AnalyticsConsumer : IEventConsumer<OrderCreatedEvent>
    {
        public Guid? LastOrderId { get; private set; }

        public Task HandleAsync(OrderCreatedEvent eventData, CancellationToken cancellationToken = default)
        {
            LastOrderId = eventData.Order.Id;
            return Task.CompletedTask;
        }
    }
}
