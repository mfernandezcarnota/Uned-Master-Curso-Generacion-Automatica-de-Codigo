using System.Collections.ObjectModel;

namespace WarehouseRoutingSystem.Domain;

public sealed class Order
{
    private readonly IReadOnlyDictionary<string, string> _attributes;

    public Order(
        Guid id,
        string description,
        bool requiresRefrigeration = false,
        bool isFragile = false,
        IReadOnlyDictionary<string, string>? attributes = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("El identificador del pedido no puede estar vacío.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("La descripción del pedido es obligatoria.", nameof(description));
        }

        Id = id;
        Description = description;
        RequiresRefrigeration = requiresRefrigeration;
        IsFragile = isFragile;
        _attributes = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>(attributes ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase));
    }

    public Guid Id { get; }

    public string Description { get; }

    public bool RequiresRefrigeration { get; }

    public bool IsFragile { get; }

    public IReadOnlyDictionary<string, string> Attributes => _attributes;

    public bool HasAttribute(string name, string value) =>
        _attributes.TryGetValue(name, out var currentValue) &&
        string.Equals(currentValue, value, StringComparison.OrdinalIgnoreCase);
}
