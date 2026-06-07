namespace OrderEventSystem.Domain.Orders;

public sealed record OrderItem
{
    public OrderItem(string productId, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productId))
        {
            throw new ArgumentException("El identificador del producto es obligatorio.", nameof(productId));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "La cantidad debe ser mayor que cero.");
        }

        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "El precio no puede ser negativo.");
        }

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public string ProductId { get; }

    public int Quantity { get; }

    public decimal UnitPrice { get; }

    public decimal Subtotal => Quantity * UnitPrice;
}
