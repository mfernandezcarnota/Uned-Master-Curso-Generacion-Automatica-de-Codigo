using System.Globalization;

namespace BankIntegrationSystem.Infrastructure.LegacySouthBank;

public sealed class SouthBankLegacyApi
{
    private readonly Dictionary<string, (decimal Amount, string Currency)> _balances = new(StringComparer.OrdinalIgnoreCase);

    public SouthBankPayment? LastPayment { get; private set; }

    public void SeedBalance(string customerAccount, decimal amount, string currency) =>
        _balances[customerAccount] = (amount, currency.ToUpperInvariant());

    public string ReadFunds(string customerAccount)
    {
        var balance = _balances.GetValueOrDefault(customerAccount);
        return $"{balance.Currency ?? "USD"}|{balance.Amount.ToString("0.00", CultureInfo.InvariantCulture)}";
    }

    public int ExecutePayment(SouthBankPayment payment)
    {
        LastPayment = payment;
        return 202;
    }
}

public sealed record SouthBankPayment(string From, string To, string Value, string IsoCurrency, string Reference);
