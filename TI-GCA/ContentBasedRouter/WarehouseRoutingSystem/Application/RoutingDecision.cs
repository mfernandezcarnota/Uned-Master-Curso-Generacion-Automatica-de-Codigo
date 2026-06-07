using WarehouseRoutingSystem.Domain;

namespace WarehouseRoutingSystem.Application;

public sealed record RoutingDecision(Order Order, OrderCategory Category, string AppliedRule)
{
    public string WarehouseCode => Category.WarehouseCode;
}
