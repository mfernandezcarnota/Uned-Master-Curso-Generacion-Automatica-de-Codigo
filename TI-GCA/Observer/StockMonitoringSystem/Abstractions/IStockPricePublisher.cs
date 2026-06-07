namespace StockMonitoringSystem.Abstractions;

public interface IStockPricePublisher
{
    void Register(IStockPriceObserver observer);

    bool Remove(IStockPriceObserver observer);
}
