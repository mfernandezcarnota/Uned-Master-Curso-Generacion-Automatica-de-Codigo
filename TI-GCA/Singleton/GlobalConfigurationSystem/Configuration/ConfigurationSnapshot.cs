namespace GlobalConfigurationSystem.Configuration;

/// <summary>
/// Immutable view of the application-wide configuration at a point in time.
/// </summary>
public sealed record ConfigurationSnapshot(
    string EnvironmentName,
    Uri ServiceEndpoint,
    string LogLevel,
    bool IsDiagnosticsEnabled,
    long Revision)
{
    public static ConfigurationSnapshot Default { get; } = new(
        EnvironmentName: "Production",
        ServiceEndpoint: new Uri("https://api.example.com/"),
        LogLevel: "Information",
        IsDiagnosticsEnabled: false,
        Revision: 0);
}
