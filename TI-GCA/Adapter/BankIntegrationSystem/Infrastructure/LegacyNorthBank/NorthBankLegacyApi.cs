namespace BankIntegrationSystem.Infrastructure.LegacyNorthBank;

public sealed class NorthBankLegacyApi
{
    private readonly Dictionary<string, decimal> _balances = new(StringComparer.OrdinalIgnoreCase);

    public string? LastBalanceAccount { get; private set; }

    public NorthBankTransferCall? LastTransfer { get; private set; }

    public void SeedBalance(string accountNumber, decimal amount) => _balances[accountNumber] = amount;

    public decimal ConsultBalance(string legacyAccount)
    {
        LastBalanceAccount = legacyAccount;
        return _balances.GetValueOrDefault(legacyAccount);
    }

    public bool SendWire(
        string debitAccount,
        string creditAccount,
        long amountInCents,
        string currencyCode,
        out string operationCode)
    {
        operationCode = $"NB-{Guid.NewGuid():N}";
        LastTransfer = new NorthBankTransferCall(
            debitAccount,
            creditAccount,
            amountInCents,
            currencyCode,
            operationCode);
        return true;
    }
}

public sealed record NorthBankTransferCall(
    string DebitAccount,
    string CreditAccount,
    long AmountInCents,
    string CurrencyCode,
    string OperationCode);
