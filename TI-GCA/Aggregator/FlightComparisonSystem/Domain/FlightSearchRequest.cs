namespace FlightComparisonSystem.Domain;

public sealed record FlightSearchRequest
{
    public FlightSearchRequest(string origin, string destination, DateOnly departureDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(origin);
        ArgumentException.ThrowIfNullOrWhiteSpace(destination);

        if (string.Equals(origin, destination, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Origin and destination must be different.", nameof(destination));
        }

        Origin = origin.Trim().ToUpperInvariant();
        Destination = destination.Trim().ToUpperInvariant();
        DepartureDate = departureDate;
    }

    public string Origin { get; }

    public string Destination { get; }

    public DateOnly DepartureDate { get; }
}
