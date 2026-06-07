using BankIntegrationSystem.Application;
using BankIntegrationSystem.Domain;
using BankIntegrationSystem.Infrastructure.LegacyNorthBank;
using BankIntegrationSystem.Infrastructure.LegacySouthBank;
using BankIntegrationSystem.Infrastructure.ModernBank;

namespace BankIntegrationSystem.Tests;

public sealed class BankIntegrationTests
{
    [Fact]
    public async Task Transfer_translates_uniform_call_to_north_bank_operation()
    {
        var api = new NorthBankLegacyApi();
        var service = new BankingService(new NorthBankGateway(api));
        var request = CreateTransfer("ES00-1111", "ES00-2222", 19.95m, "eur");

        var receipt = await service.TransferAsync(request);

        Assert.True(receipt.Accepted);
        Assert.NotNull(api.LastTransfer);
        Assert.Equal("ES001111", api.LastTransfer.DebitAccount);
        Assert.Equal("ES002222", api.LastTransfer.CreditAccount);
        Assert.Equal(1995, api.LastTransfer.AmountInCents);
        Assert.Equal("EUR", api.LastTransfer.CurrencyCode);
        Assert.Equal(api.LastTransfer.OperationCode, receipt.TransactionId);
    }

    [Fact]
    public async Task Gateway_converts_legacy_balance_and_transfer_formats()
    {
        var api = new SouthBankLegacyApi();
        api.SeedBalance("customer-7", 1234.56m, "usd");
        var service = new BankingService(new SouthBankGateway(api));

        var balance = await service.GetBalanceAsync(new BankAccount("customer-7"));
        await service.TransferAsync(CreateTransfer("customer-7", "customer-8", 10.5m, "usd"));

        Assert.Equal(new Money(1234.56m, "USD"), balance);
        Assert.NotNull(api.LastPayment);
        Assert.Equal("10.50", api.LastPayment.Value);
        Assert.Equal("USD", api.LastPayment.IsoCurrency);
    }

    [Fact]
    public async Task Banking_service_accepts_another_legacy_api_without_changes()
    {
        var northApi = new NorthBankLegacyApi();
        northApi.SeedBalance("ES009999", 88m);
        var northService = new BankingService(new NorthBankGateway(northApi));

        var southApi = new SouthBankLegacyApi();
        southApi.SeedBalance("ES00-9999", 88m, "eur");
        var southService = new BankingService(new SouthBankGateway(southApi));

        var northBalance = await northService.GetBalanceAsync(new BankAccount("ES00-9999"));
        var southBalance = await southService.GetBalanceAsync(new BankAccount("ES00-9999"));

        Assert.Equal(northBalance, southBalance);
    }

    [Fact]
    public async Task Banking_service_can_replace_legacy_provider_with_modern_provider()
    {
        var replacement = new RecordingModernBankApi();
        var service = new BankingService(new ModernBankGateway(replacement));
        var request = CreateTransfer("source", "destination", 75m, "eur");

        var balance = await service.GetBalanceAsync(request.Source);
        var receipt = await service.TransferAsync(request);

        Assert.Equal(new Money(900m, "EUR"), balance);
        Assert.True(receipt.Accepted);
        Assert.Equal("source", replacement.LastBalanceAccount);
        Assert.Same(request, replacement.LastTransfer);
    }

    private static TransferRequest CreateTransfer(string source, string destination, decimal amount, string currency) =>
        new(new BankAccount(source), new BankAccount(destination), new Money(amount, currency));

    private sealed class RecordingModernBankApi : IModernBankApi
    {
        public string? LastBalanceAccount { get; private set; }

        public TransferRequest? LastTransfer { get; private set; }

        public Task<Money> GetBalanceAsync(string accountNumber, CancellationToken cancellationToken)
        {
            LastBalanceAccount = accountNumber;
            return Task.FromResult(new Money(900m, "EUR"));
        }

        public Task<TransferReceipt> CreateTransferAsync(TransferRequest request, CancellationToken cancellationToken)
        {
            LastTransfer = request;
            return Task.FromResult(new TransferReceipt("replacement-1", true));
        }
    }
}
