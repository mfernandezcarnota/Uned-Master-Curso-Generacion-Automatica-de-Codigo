using StockMonitoringSystem.Abstractions;
using StockMonitoringSystem.Domain;

namespace StockMonitoringSystem.Investors;

public sealed class Investor : IStockPriceObserver
{
    private readonly List<StockPriceUpdate> _notifications = [];

    public Investor(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
    }

    public string Name { get; }

    public IReadOnlyList<StockPriceUpdate> Notifications => _notifications.AsReadOnly();

    public void OnPriceChanged(StockPriceUpdate update)
    {
        ArgumentNullException.ThrowIfNull(update);
        _notifications.Add(update);
    }
}
