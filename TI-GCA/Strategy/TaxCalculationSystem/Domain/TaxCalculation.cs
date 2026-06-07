namespace TaxCalculationSystem.Domain;

public sealed record TaxCalculation(
    string CountryCode,
    string CountryName,
    decimal TaxableAmount,
    decimal TaxAmount)
{
    public decimal TotalAmount => TaxableAmount + TaxAmount;
}
