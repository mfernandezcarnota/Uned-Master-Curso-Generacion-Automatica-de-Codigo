using StockMonitoringSystem.Domain;

namespace StockMonitoringSystem.Abstractions;

public interface IStockPriceObserver
{
    void OnPriceChanged(StockPriceUpdate update);
}
