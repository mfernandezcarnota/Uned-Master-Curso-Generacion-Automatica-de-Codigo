using WarehouseRoutingSystem.Domain;

namespace WarehouseRoutingSystem.Application;

public sealed class ClassificationRule
{
    private readonly Func<Order, bool> _condition;

    public ClassificationRule(string name, OrderCategory category, int priority, Func<Order, bool> condition)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre de la regla es obligatorio.", nameof(name));
        }

        Name = name;
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Priority = priority;
        _condition = condition ?? throw new ArgumentNullException(nameof(condition));
    }

    public string Name { get; }

    public OrderCategory Category { get; }

    public int Priority { get; }

    public bool Matches(Order order) => _condition(order ?? throw new ArgumentNullException(nameof(order)));
}
