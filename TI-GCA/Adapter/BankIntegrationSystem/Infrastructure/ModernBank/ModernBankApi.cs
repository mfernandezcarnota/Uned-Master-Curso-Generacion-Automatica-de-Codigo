using BankIntegrationSystem.Domain;

namespace BankIntegrationSystem.Infrastructure.ModernBank;

public interface IModernBankApi
{
    Task<Money> GetBalanceAsync(string accountNumber, CancellationToken cancellationToken);

    Task<TransferReceipt> CreateTransferAsync(TransferRequest request, CancellationToken cancellationToken);
}

public sealed class ModernBankApi : IModernBankApi
{
    public Task<Money> GetBalanceAsync(string accountNumber, CancellationToken cancellationToken) =>
        Task.FromResult(new Money(2_500m, "EUR"));

    public Task<TransferReceipt> CreateTransferAsync(TransferRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(new TransferReceipt($"MB-{Guid.NewGuid():N}", true));
}
