namespace BankIntegrationSystem.Domain;

public sealed record Money(decimal Amount, string Currency)
{
    public string Currency { get; } = string.IsNullOrWhiteSpace(Currency)
        ? throw new ArgumentException("The currency is required.", nameof(Currency))
        : Currency.ToUpperInvariant();
}
