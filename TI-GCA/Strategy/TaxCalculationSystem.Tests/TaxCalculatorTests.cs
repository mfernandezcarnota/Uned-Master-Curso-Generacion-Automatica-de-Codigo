using TaxCalculationSystem.Application;
using TaxCalculationSystem.Domain;
using TaxCalculationSystem.TaxRules;

namespace TaxCalculationSystem.Tests;

public sealed class TaxCalculatorTests
{
    [Fact]
    public void Calculate_WithSpainRule_AppliesSpanishTax()
    {
        var calculator = new TaxCalculator(new SpainTaxRule());

        var result = calculator.Calculate(1_000m);

        Assert.Equal("ES", result.CountryCode);
        Assert.Equal(210m, result.TaxAmount);
        Assert.Equal(1_210m, result.TotalAmount);
    }

    [Fact]
    public void Calculate_WithGermanyRule_AppliesGermanTax()
    {
        var calculator = new TaxCalculator(new GermanyTaxRule());

        var result = calculator.Calculate(1_000m);

        Assert.Equal("DE", result.CountryCode);
        Assert.Equal(190m, result.TaxAmount);
        Assert.Equal(1_190m, result.TotalAmount);
    }

    [Fact]
    public void ChangeRule_ReplacesTheRuleDuringExecution()
    {
        var calculator = new TaxCalculator(new SpainTaxRule());
        var spanishResult = calculator.Calculate(500m);

        calculator.ChangeRule(new GermanyTaxRule());
        var germanResult = calculator.Calculate(500m);

        Assert.Equal(105m, spanishResult.TaxAmount);
        Assert.Equal(95m, germanResult.TaxAmount);
    }

    [Fact]
    public void Calculate_WithNewFranceRule_DoesNotRequireChangingCalculator()
    {
        var calculator = new TaxCalculator(new FranceTaxRule());

        var result = calculator.Calculate(1_000m);

        Assert.Equal("FR", result.CountryCode);
        Assert.Equal("Francia", result.CountryName);
        Assert.Equal(200m, result.TaxAmount);
        Assert.Equal(1_200m, result.TotalAmount);
    }

    private sealed class FranceTaxRule : ProportionalTaxRule
    {
        protected override string CountryCode => "FR";

        protected override string CountryName => "Francia";

        protected override decimal TaxRate => 0.20m;
    }
}
