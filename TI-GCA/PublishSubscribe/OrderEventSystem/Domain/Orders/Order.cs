namespace OrderEventSystem.Domain.Orders;

public sealed class Order
{
    private Order(Guid id, string customerEmail, IReadOnlyList<OrderItem> items, DateTimeOffset createdAt)
    {
        Id = id;
        CustomerEmail = customerEmail;
        Items = items;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }

    public string CustomerEmail { get; }

    public IReadOnlyList<OrderItem> Items { get; }

    public DateTimeOffset CreatedAt { get; }

    public decimal Total => Items.Sum(item => item.Subtotal);

    public static Order Create(string customerEmail, IEnumerable<OrderItem> items)
    {
        if (string.IsNullOrWhiteSpace(customerEmail))
        {
            throw new ArgumentException("El correo del cliente es obligatorio.", nameof(customerEmail));
        }

        ArgumentNullException.ThrowIfNull(items);
        var itemList = items.ToArray();
        if (itemList.Length == 0)
        {
            throw new ArgumentException("El pedido debe contener al menos un artículo.", nameof(items));
        }

        return new Order(Guid.NewGuid(), customerEmail, itemList, DateTimeOffset.UtcNow);
    }
}
