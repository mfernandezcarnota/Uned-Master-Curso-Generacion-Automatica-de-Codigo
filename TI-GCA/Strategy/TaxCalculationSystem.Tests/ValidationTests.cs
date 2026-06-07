using TaxCalculationSystem.Application;
using TaxCalculationSystem.TaxRules;

namespace TaxCalculationSystem.Tests;

public sealed class ValidationTests
{
    [Fact]
    public void Calculate_WithNegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        var calculator = new TaxCalculator(new SpainTaxRule());

        Assert.Throws<ArgumentOutOfRangeException>(() => calculator.Calculate(-1m));
    }

    [Fact]
    public void ChangeRule_WithNullRule_ThrowsArgumentNullException()
    {
        var calculator = new TaxCalculator(new SpainTaxRule());

        Assert.Throws<ArgumentNullException>(() => calculator.ChangeRule(null!));
    }
}
