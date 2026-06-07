namespace WarehouseRoutingSystem.Domain;

public static class InitialCategories
{
    public static OrderCategory Refrigerated { get; } = new("Refrigerado", "ALM-REF");

    public static OrderCategory Fragile { get; } = new("Frágil", "ALM-FRA");

    public static OrderCategory Standard { get; } = new("Estándar", "ALM-EST");
}
