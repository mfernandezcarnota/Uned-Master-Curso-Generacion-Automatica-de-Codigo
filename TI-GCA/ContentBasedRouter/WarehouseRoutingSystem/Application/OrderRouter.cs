using System.Collections.Concurrent;
using WarehouseRoutingSystem.Domain;

namespace WarehouseRoutingSystem.Application;

public sealed class OrderRouter
{
    private readonly ConcurrentDictionary<string, OrderCategory> _categories =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly object _rulesLock = new();
    private ClassificationRule[] _rules = [];

    public IReadOnlyCollection<OrderCategory> Categories => _categories.Values.ToArray();

    public void RegisterCategory(OrderCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);

        if (!_categories.TryAdd(category.Name, category))
        {
            throw new InvalidOperationException($"La categoría '{category.Name}' ya está registrada.");
        }
    }

    public void RegisterRule(ClassificationRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        if (!_categories.ContainsKey(rule.Category.Name))
        {
            throw new InvalidOperationException(
                $"La categoría '{rule.Category.Name}' debe registrarse antes de añadir reglas.");
        }

        lock (_rulesLock)
        {
            if (_rules.Any(current => string.Equals(current.Name, rule.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"La regla '{rule.Name}' ya está registrada.");
            }

            var updatedRules = _rules.Append(rule)
                .OrderByDescending(current => current.Priority)
                .ToArray();
            Volatile.Write(ref _rules, updatedRules);
        }
    }

    public RoutingDecision Route(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var matchingRule = Volatile.Read(ref _rules).FirstOrDefault(rule => rule.Matches(order))
            ?? throw new InvalidOperationException("Ninguna regla permite clasificar el pedido.");

        return new RoutingDecision(order, matchingRule.Category, matchingRule.Name);
    }

    public async Task<IReadOnlyList<RoutingDecision>> RouteManyAsync(
        IEnumerable<Order> orders,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(orders);

        var indexedOrders = orders.Select((order, index) => (Order: order, Index: index)).ToArray();
        var decisions = new RoutingDecision[indexedOrders.Length];

        await Parallel.ForEachAsync(indexedOrders, cancellationToken, (item, _) =>
        {
            decisions[item.Index] = Route(item.Order);
            return ValueTask.CompletedTask;
        });

        return decisions;
    }
}
