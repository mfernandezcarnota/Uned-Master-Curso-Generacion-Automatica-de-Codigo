namespace FlightComparisonSystem.Domain;

public sealed record FlightOption(
    string Airline,
    string FlightNumber,
    string Origin,
    string Destination,
    DateTimeOffset Departure,
    DateTimeOffset Arrival,
    decimal Price,
    string Currency);
