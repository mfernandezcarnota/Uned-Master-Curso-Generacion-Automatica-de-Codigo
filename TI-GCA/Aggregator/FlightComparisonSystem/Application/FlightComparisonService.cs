using FlightComparisonSystem.Domain;

namespace FlightComparisonSystem.Application;

public sealed class FlightComparisonService
{
    private readonly IReadOnlyList<IAirlineSource> _airlines;

    public FlightComparisonService(IEnumerable<IAirlineSource> airlines)
    {
        ArgumentNullException.ThrowIfNull(airlines);
        _airlines = airlines.ToArray();

        if (_airlines.Count == 0)
        {
            throw new ArgumentException("At least one airline source is required.", nameof(airlines));
        }

        if (_airlines.Any(airline => airline is null))
        {
            throw new ArgumentException("Airline sources cannot contain null values.", nameof(airlines));
        }

        if (_airlines.GroupBy(airline => airline.Name, StringComparer.OrdinalIgnoreCase).Any(group => group.Count() > 1))
        {
            throw new ArgumentException("Airline source names must be unique.", nameof(airlines));
        }
    }

    public async Task<FlightComparisonResult> SearchAsync(
        FlightSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var searches = _airlines.Select(airline => SearchAirlineAsync(airline, request, cancellationToken));
        var responses = await Task.WhenAll(searches);

        var flights = responses
            .SelectMany(response => response.Flights)
            .OrderBy(flight => flight.Price)
            .ThenBy(flight => flight.Departure)
            .ToArray();

        var successfulAirlines = responses
            .Where(response => response.Failure is null)
            .Select(response => response.Airline)
            .ToArray();

        var failures = responses
            .Where(response => response.Failure is not null)
            .Select(response => response.Failure!)
            .ToArray();

        return new FlightComparisonResult(flights, successfulAirlines, failures);
    }

    private static async Task<AirlineResponse> SearchAirlineAsync(
        IAirlineSource airline,
        FlightSearchRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var flights = await airline.SearchAsync(request, cancellationToken)
                ?? throw new InvalidOperationException("The airline returned a null response.");
            return new AirlineResponse(airline.Name, flights, null);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            return new AirlineResponse(
                airline.Name,
                Array.Empty<FlightOption>(),
                new AirlineFailure(airline.Name, exception.Message));
        }
    }

    private sealed record AirlineResponse(
        string Airline,
        IReadOnlyList<FlightOption> Flights,
        AirlineFailure? Failure);
}
