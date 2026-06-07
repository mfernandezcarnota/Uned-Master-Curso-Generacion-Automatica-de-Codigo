using BankIntegrationSystem.Application;
using BankIntegrationSystem.Domain;
using BankIntegrationSystem.Infrastructure.LegacyNorthBank;

var legacyApi = new NorthBankLegacyApi();
legacyApi.SeedBalance("ES001234", 1_250.75m);

IBankGateway gateway = new NorthBankGateway(legacyApi);
var banking = new BankingService(gateway);
var balance = await banking.GetBalanceAsync(new BankAccount("ES00-1234"));

Console.WriteLine($"Available balance: {balance.Amount:0.00} {balance.Currency}");
