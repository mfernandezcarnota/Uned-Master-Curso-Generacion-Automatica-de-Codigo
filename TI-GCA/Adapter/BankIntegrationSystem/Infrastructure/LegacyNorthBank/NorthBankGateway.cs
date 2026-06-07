using BankIntegrationSystem.Application;
using BankIntegrationSystem.Domain;

namespace BankIntegrationSystem.Infrastructure.LegacyNorthBank;

public sealed class NorthBankGateway(NorthBankLegacyApi legacyApi) : IBankGateway
{
    public Task<Money> GetBalanceAsync(BankAccount account, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var amount = legacyApi.ConsultBalance(ToLegacyAccount(account));
        return Task.FromResult(new Money(amount, "EUR"));
    }

    public Task<TransferReceipt> TransferAsync(
        TransferRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var amountInCents = checked(decimal.ToInt64(request.Amount.Amount * 100m));
        var accepted = legacyApi.SendWire(
            ToLegacyAccount(request.Source),
            ToLegacyAccount(request.Destination),
            amountInCents,
            request.Amount.Currency,
            out var operationCode);

        return Task.FromResult(new TransferReceipt(operationCode, accepted));
    }

    private static string ToLegacyAccount(BankAccount account) => account.Number.Replace("-", string.Empty);
}
