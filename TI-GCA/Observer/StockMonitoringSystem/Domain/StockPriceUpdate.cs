namespace StockMonitoringSystem.Domain;

public sealed record StockPriceUpdate(
    string Symbol,
    decimal PreviousPrice,
    decimal CurrentPrice,
    DateTimeOffset OccurredAt);
