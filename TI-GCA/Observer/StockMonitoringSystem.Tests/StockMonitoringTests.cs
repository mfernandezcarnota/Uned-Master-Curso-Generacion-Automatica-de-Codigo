using StockMonitoringSystem.Investors;
using StockMonitoringSystem.Monitoring;

namespace StockMonitoringSystem.Tests;

public sealed class StockMonitoringTests
{
    [Fact]
    public void RegisteredInvestorReceivesPriceChanges()
    {
        var stock = new Stock("ACME", 100m);
        var investor = new Investor("Ana");

        stock.Register(investor);
        stock.UpdatePrice(105m);

        var notification = Assert.Single(investor.Notifications);
        Assert.Equal("ACME", notification.Symbol);
        Assert.Equal(100m, notification.PreviousPrice);
        Assert.Equal(105m, notification.CurrentPrice);
    }

    [Fact]
    public void MultipleRegisteredInvestorsReceiveTheSamePriceChange()
    {
        var stock = new Stock("ACME", 100m);
        var firstInvestor = new Investor("Ana");
        var secondInvestor = new Investor("Bruno");
        var thirdInvestor = new Investor("Carla");

        stock.Register(firstInvestor);
        stock.Register(secondInvestor);
        stock.Register(thirdInvestor);
        stock.UpdatePrice(102.50m);

        Assert.Equal(102.50m, Assert.Single(firstInvestor.Notifications).CurrentPrice);
        Assert.Equal(102.50m, Assert.Single(secondInvestor.Notifications).CurrentPrice);
        Assert.Equal(102.50m, Assert.Single(thirdInvestor.Notifications).CurrentPrice);
    }

    [Fact]
    public void RemovedInvestorStopsReceivingPriceChanges()
    {
        var stock = new Stock("ACME", 100m);
        var investor = new Investor("Ana");
        stock.Register(investor);

        var removed = stock.Remove(investor);
        stock.UpdatePrice(110m);

        Assert.True(removed);
        Assert.Empty(investor.Notifications);
    }

    [Fact]
    public void PriceChangeAutomaticallyCreatesANotification()
    {
        var stock = new Stock("ACME", 100m);
        var investor = new Investor("Ana");
        stock.Register(investor);

        stock.UpdatePrice(99m);

        Assert.Equal(99m, stock.Price);
        Assert.Single(investor.Notifications);
    }

    [Fact]
    public void ConsecutiveUpdatesAreDeliveredInOrder()
    {
        var stock = new Stock("ACME", 100m);
        var investor = new Investor("Ana");
        stock.Register(investor);

        stock.UpdatePrice(101m);
        stock.UpdatePrice(98m);
        stock.UpdatePrice(103m);

        Assert.Collection(
            investor.Notifications,
            update => Assert.Equal((100m, 101m), (update.PreviousPrice, update.CurrentPrice)),
            update => Assert.Equal((101m, 98m), (update.PreviousPrice, update.CurrentPrice)),
            update => Assert.Equal((98m, 103m), (update.PreviousPrice, update.CurrentPrice)));
    }

    [Fact]
    public void RegisteringTheSameInvestorTwiceDoesNotDuplicateNotifications()
    {
        var stock = new Stock("ACME", 100m);
        var investor = new Investor("Ana");

        stock.Register(investor);
        stock.Register(investor);
        stock.UpdatePrice(101m);

        Assert.Single(investor.Notifications);
    }

    [Fact]
    public void UnchangedPriceDoesNotCreateANotification()
    {
        var stock = new Stock("ACME", 100m);
        var investor = new Investor("Ana");
        stock.Register(investor);

        stock.UpdatePrice(100m);

        Assert.Empty(investor.Notifications);
    }
}
