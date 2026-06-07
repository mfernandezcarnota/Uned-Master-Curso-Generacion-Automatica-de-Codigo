namespace WarehouseRoutingSystem.Domain;

public sealed record OrderCategory
{
    public OrderCategory(string name, string warehouseCode)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre de la categoría es obligatorio.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(warehouseCode))
        {
            throw new ArgumentException("El código de almacén es obligatorio.", nameof(warehouseCode));
        }

        Name = name;
        WarehouseCode = warehouseCode;
    }

    public string Name { get; }

    public string WarehouseCode { get; }
}
