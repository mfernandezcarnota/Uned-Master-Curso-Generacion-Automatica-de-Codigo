using StockMonitoringSystem.Abstractions;
using StockMonitoringSystem.Domain;

namespace StockMonitoringSystem.Monitoring;

public sealed class Stock : IStockPricePublisher
{
    private readonly List<IStockPriceObserver> _observers = [];

    public Stock(string symbol, decimal initialPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);
        EnsureValidPrice(initialPrice);

        Symbol = symbol.Trim().ToUpperInvariant();
        Price = initialPrice;
    }

    public string Symbol { get; }

    public decimal Price { get; private set; }

    public void Register(IStockPriceObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public bool Remove(IStockPriceObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        return _observers.Remove(observer);
    }

    public void UpdatePrice(decimal newPrice)
    {
        EnsureValidPrice(newPrice);

        if (newPrice == Price)
        {
            return;
        }

        var update = new StockPriceUpdate(Symbol, Price, newPrice, DateTimeOffset.UtcNow);
        Price = newPrice;

        foreach (var observer in _observers.ToArray())
        {
            observer.OnPriceChanged(update);
        }
    }

    private static void EnsureValidPrice(decimal price)
    {
        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), price, "The stock price cannot be negative.");
        }
    }
}
