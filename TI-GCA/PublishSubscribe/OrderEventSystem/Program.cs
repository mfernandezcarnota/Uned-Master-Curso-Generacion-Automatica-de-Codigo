using OrderEventSystem.Application;
using OrderEventSystem.Domain.Events;
using OrderEventSystem.Domain.Orders;
using OrderEventSystem.Infrastructure;
using OrderEventSystem.Messaging;
using OrderEventSystem.Services;

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

var orderService = new OrderService(new InMemoryOrderRepository(), eventBus);
var order = await orderService.CreateOrderAsync(
    "cliente@example.com",
    [new OrderItem("PORTATIL-001", 1, 999.99m)]);

Console.WriteLine($"Pedido {order.Id} creado por {order.Total:C}.");
Console.WriteLine($"Servicios informados: {consumers.Count(consumer => consumer.ProcessedOrderIds.Contains(order.Id))}.");
