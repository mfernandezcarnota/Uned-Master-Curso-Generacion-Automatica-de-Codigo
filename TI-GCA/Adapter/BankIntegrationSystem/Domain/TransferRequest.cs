namespace BankIntegrationSystem.Domain;

public sealed record TransferRequest(BankAccount Source, BankAccount Destination, Money Amount);
