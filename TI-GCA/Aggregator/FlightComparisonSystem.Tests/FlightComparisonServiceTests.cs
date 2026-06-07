using System.Collections.Concurrent;
using FlightComparisonSystem.Application;
using FlightComparisonSystem.Domain;

namespace FlightComparisonSystem.Tests;

public sealed class FlightComparisonServiceTests
{
    private static readonly FlightSearchRequest Request = new("MAD", "LHR", new DateOnly(2026, 7, 15));

    [Fact]
    public async Task SearchAsync_WithOneAirline_ReturnsItsFlights()
    {
        var expected = CreateFlight("Airline A", "AA101", 180m);
        var service = new FlightComparisonService([new StubAirline("Airline A", [expected])]);

        var result = await service.SearchAsync(Request);

        Assert.Equal(new[] { expected }, result.Flights);
        Assert.Equal(new[] { "Airline A" }, result.SuccessfulAirlines);
        Assert.Empty(result.Failures);
        Assert.False(result.IsPartial);
    }

    [Fact]
    public async Task SearchAsync_WithMultipleAirlines_QueriesAllSourcesConcurrently()
    {
        var started = new ConcurrentBag<string>();
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var airlines = new[]
        {
            new ControlledAirline("Airline A", started, release.Task),
            new ControlledAirline("Airline B", started, release.Task),
            new ControlledAirline("Airline C", started, release.Task)
        };
        var service = new FlightComparisonService(airlines);

        var search = service.SearchAsync(Request);
        await WaitUntilAsync(() => started.Count == airlines.Length);
        release.SetResult();
        var result = await search;

        Assert.Equal(3, started.Count);
        Assert.Equal(3, result.SuccessfulAirlines.Count);
    }

    [Fact]
    public async Task SearchAsync_AggregatesAndOrdersFlightsByPrice()
    {
        var service = new FlightComparisonService(
        [
            new StubAirline("Airline A", [CreateFlight("Airline A", "AA101", 220m)]),
            new StubAirline("Airline B", [CreateFlight("Airline B", "BB101", 125m), CreateFlight("Airline B", "BB102", 175m)])
        ]);

        var result = await service.SearchAsync(Request);

        Assert.Equal(new[] { 125m, 175m, 220m }, result.Flights.Select(flight => flight.Price));
        Assert.Equal(new[] { "Airline A", "Airline B" }, result.SuccessfulAirlines);
    }

    [Fact]
    public async Task SearchAsync_AcceptsANewAirlineWithoutChangingTheService()
    {
        var newAirline = new StubAirline("Airline D", [CreateFlight("Airline D", "DD101", 99m)]);
        var service = new FlightComparisonService([newAirline]);

        var result = await service.SearchAsync(Request);

        Assert.Single(result.Flights);
        Assert.Equal("Airline D", result.Flights[0].Airline);
    }

    [Fact]
    public async Task SearchAsync_WhenOneAirlineFails_ReturnsSuccessfulResultsAndFailureDetails()
    {
        var availableFlight = CreateFlight("Airline A", "AA101", 180m);
        var service = new FlightComparisonService(
        [
            new StubAirline("Airline A", [availableFlight]),
            new FailingAirline("Airline B", "Service temporarily unavailable"),
            new StubAirline("Airline C", [])
        ]);

        var result = await service.SearchAsync(Request);

        Assert.Equal(new[] { availableFlight }, result.Flights);
        Assert.Equal(new[] { "Airline A", "Airline C" }, result.SuccessfulAirlines);
        var failure = Assert.Single(result.Failures);
        Assert.Equal("Airline B", failure.Airline);
        Assert.Equal("Service temporarily unavailable", failure.Reason);
        Assert.True(result.IsPartial);
    }

    private static FlightOption CreateFlight(string airline, string flightNumber, decimal price) =>
        new(airline, flightNumber, "MAD", "LHR", new DateTimeOffset(2026, 7, 15, 8, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 7, 15, 10, 0, 0, TimeSpan.Zero), price, "EUR");

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        while (!condition())
        {
            await Task.Delay(10, timeout.Token);
        }
    }

    private sealed class StubAirline(string name, IReadOnlyList<FlightOption> flights) : IAirlineSource
    {
        public string Name => name;

        public Task<IReadOnlyList<FlightOption>> SearchAsync(FlightSearchRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(flights);
    }

    private sealed class ControlledAirline(string name, ConcurrentBag<string> started, Task release) : IAirlineSource
    {
        public string Name => name;

        public async Task<IReadOnlyList<FlightOption>> SearchAsync(FlightSearchRequest request, CancellationToken cancellationToken = default)
        {
            started.Add(Name);
            await release.WaitAsync(cancellationToken);
            return [];
        }
    }

    private sealed class FailingAirline(string name, string message) : IAirlineSource
    {
        public string Name => name;

        public Task<IReadOnlyList<FlightOption>> SearchAsync(FlightSearchRequest request, CancellationToken cancellationToken = default) =>
            Task.FromException<IReadOnlyList<FlightOption>>(new InvalidOperationException(message));
    }
}
