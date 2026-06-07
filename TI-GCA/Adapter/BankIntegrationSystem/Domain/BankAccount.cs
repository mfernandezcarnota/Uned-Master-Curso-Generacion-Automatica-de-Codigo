namespace BankIntegrationSystem.Domain;

public sealed record BankAccount(string Number)
{
    public string Number { get; } = string.IsNullOrWhiteSpace(Number)
        ? throw new ArgumentException("The account number is required.", nameof(Number))
        : Number;
}
