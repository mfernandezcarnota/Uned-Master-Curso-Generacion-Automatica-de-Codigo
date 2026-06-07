using System.Globalization;
using BankIntegrationSystem.Application;
using BankIntegrationSystem.Domain;

namespace BankIntegrationSystem.Infrastructure.LegacySouthBank;

public sealed class SouthBankGateway(SouthBankLegacyApi legacyApi) : IBankGateway
{
    public Task<Money> GetBalanceAsync(BankAccount account, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var parts = legacyApi.ReadFunds(account.Number).Split('|');
        if (parts.Length != 2 || !decimal.TryParse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
        {
            throw new InvalidOperationException("The bank returned an invalid balance.");
        }

        return Task.FromResult(new Money(amount, parts[0]));
    }

    public Task<TransferReceipt> TransferAsync(
        TransferRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var reference = Guid.NewGuid().ToString("N");
        var payment = new SouthBankPayment(
            request.Source.Number,
            request.Destination.Number,
            request.Amount.Amount.ToString("0.00", CultureInfo.InvariantCulture),
            request.Amount.Currency,
            reference);

        var statusCode = legacyApi.ExecutePayment(payment);
        return Task.FromResult(new TransferReceipt(reference, statusCode is >= 200 and < 300));
    }
}
