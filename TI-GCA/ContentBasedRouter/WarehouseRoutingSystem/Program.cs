using WarehouseRoutingSystem.Application;
using WarehouseRoutingSystem.Domain;

var router = OrderRoutingSystemFactory.CreateDefault();
var orders = new[]
{
    new Order(Guid.NewGuid(), "Productos lácteos", requiresRefrigeration: true),
    new Order(Guid.NewGuid(), "Cristalería", isFragile: true),
    new Order(Guid.NewGuid(), "Material de oficina")
};

var decisions = await router.RouteManyAsync(orders);

foreach (var decision in decisions)
{
    Console.WriteLine(
        $"{decision.Order.Description}: {decision.Category.Name} -> {decision.WarehouseCode}");
}
