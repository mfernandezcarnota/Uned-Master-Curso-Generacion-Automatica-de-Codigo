using BankIntegrationSystem.Application;
using BankIntegrationSystem.Domain;

namespace BankIntegrationSystem.Infrastructure.ModernBank;

public sealed class ModernBankGateway(IModernBankApi bankApi) : IBankGateway
{
    public Task<Money> GetBalanceAsync(BankAccount account, CancellationToken cancellationToken = default) =>
        bankApi.GetBalanceAsync(account.Number, cancellationToken);

    public Task<TransferReceipt> TransferAsync(
        TransferRequest request,
        CancellationToken cancellationToken = default) => bankApi.CreateTransferAsync(request, cancellationToken);
}
