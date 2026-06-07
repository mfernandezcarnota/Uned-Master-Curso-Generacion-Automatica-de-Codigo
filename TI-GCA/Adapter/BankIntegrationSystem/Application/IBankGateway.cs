using BankIntegrationSystem.Domain;

namespace BankIntegrationSystem.Application;

public interface IBankGateway
{
    Task<Money> GetBalanceAsync(BankAccount account, CancellationToken cancellationToken = default);

    Task<TransferReceipt> TransferAsync(TransferRequest request, CancellationToken cancellationToken = default);
}
