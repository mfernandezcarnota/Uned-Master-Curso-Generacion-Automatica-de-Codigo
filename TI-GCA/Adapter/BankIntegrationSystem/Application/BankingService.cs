using BankIntegrationSystem.Domain;

namespace BankIntegrationSystem.Application;

public sealed class BankingService(IBankGateway bankGateway)
{
    public Task<Money> GetBalanceAsync(BankAccount account, CancellationToken cancellationToken = default) =>
        bankGateway.GetBalanceAsync(account, cancellationToken);

    public Task<TransferReceipt> TransferAsync(
        TransferRequest request,
        CancellationToken cancellationToken = default) => bankGateway.TransferAsync(request, cancellationToken);
}
