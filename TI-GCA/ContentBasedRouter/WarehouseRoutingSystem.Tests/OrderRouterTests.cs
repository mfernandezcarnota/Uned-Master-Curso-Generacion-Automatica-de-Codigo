namespace WarehouseRoutingSystem.Tests;

public sealed class OrderRouterTests
{
    [Fact]
    public void Route_RefrigeratedOrder_SendsItToRefrigeratedWarehouse()
    {
        var router = OrderRoutingSystemFactory.CreateDefault();
        var order = new Order(Guid.NewGuid(), "Vacunas", requiresRefrigeration: true);

        var decision = router.Route(order);

        Assert.Equal("Refrigerado", decision.Category.Name);
        Assert.Equal("ALM-REF", decision.WarehouseCode);
    }

    [Fact]
    public void Route_FragileOrder_SendsItToFragileWarehouse()
    {
        var router = OrderRoutingSystemFactory.CreateDefault();
        var order = new Order(Guid.NewGuid(), "Vajilla", isFragile: true);

        var decision = router.Route(order);

        Assert.Equal("Frágil", decision.Category.Name);
        Assert.Equal("ALM-FRA", decision.WarehouseCode);
    }

    [Fact]
    public void Route_OrdinaryOrder_SendsItToStandardWarehouse()
    {
        var router = OrderRoutingSystemFactory.CreateDefault();
        var order = new Order(Guid.NewGuid(), "Folios");

        var decision = router.Route(order);

        Assert.Equal("Estándar", decision.Category.Name);
        Assert.Equal("ALM-EST", decision.WarehouseCode);
    }

    [Fact]
    public void RegisterCategoryAndRule_AllowsRoutingANewCategory()
    {
        var router = OrderRoutingSystemFactory.CreateDefault();
        var hazardous = new OrderCategory("Peligroso", "ALM-PEL");
        router.RegisterCategory(hazardous);
        router.RegisterRule(new ClassificationRule(
            "Contiene materiales peligrosos",
            hazardous,
            priority: 300,
            order => order.HasAttribute("Peligroso", "Sí")));
        var order = new Order(
            Guid.NewGuid(),
            "Producto químico",
            attributes: new Dictionary<string, string> { ["Peligroso"] = "Sí" });

        var decision = router.Route(order);

        Assert.Equal(hazardous, decision.Category);
        Assert.Equal("ALM-PEL", decision.WarehouseCode);
    }

    [Fact]
    public async Task RouteManyAsync_ProcessesConcurrentOrdersSafely()
    {
        var router = OrderRoutingSystemFactory.CreateDefault();
        var orders = Enumerable.Range(0, 300)
            .Select(index => new Order(
                Guid.NewGuid(),
                $"Pedido {index}",
                requiresRefrigeration: index % 3 == 0,
                isFragile: index % 3 == 1))
            .ToArray();

        var decisions = await router.RouteManyAsync(orders);

        Assert.Equal(orders.Length, decisions.Count);
        Assert.Equal(100, decisions.Count(decision => decision.Category == InitialCategories.Refrigerated));
        Assert.Equal(100, decisions.Count(decision => decision.Category == InitialCategories.Fragile));
        Assert.Equal(100, decisions.Count(decision => decision.Category == InitialCategories.Standard));
        Assert.Equal(orders.Select(order => order.Id), decisions.Select(decision => decision.Order.Id));
    }
}
