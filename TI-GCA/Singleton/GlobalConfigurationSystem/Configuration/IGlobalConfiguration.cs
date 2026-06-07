namespace GlobalConfigurationSystem.Configuration;

/// <summary>
/// Provides read and atomic update access to the application-wide configuration.
/// </summary>
public interface IGlobalConfiguration
{
    ConfigurationSnapshot Snapshot { get; }

    ConfigurationSnapshot Update(Func<ConfigurationSnapshot, ConfigurationSnapshot> updateFactory);
}
