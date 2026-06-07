using System.Threading;

namespace GlobalConfigurationSystem.Configuration;

/// <summary>
/// Provides the single application-wide entry point for configuration.
/// </summary>
public sealed class GlobalConfiguration : IGlobalConfiguration
{
    private static readonly Lazy<GlobalConfiguration> Shared = new(
        static () => new GlobalConfiguration(),
        LazyThreadSafetyMode.ExecutionAndPublication);

    private ConfigurationSnapshot _snapshot = ConfigurationSnapshot.Default;

    private GlobalConfiguration()
    {
    }

    public static IGlobalConfiguration Current => Shared.Value;

    public ConfigurationSnapshot Snapshot => Volatile.Read(ref _snapshot);

    public ConfigurationSnapshot Update(Func<ConfigurationSnapshot, ConfigurationSnapshot> updateFactory)
    {
        ArgumentNullException.ThrowIfNull(updateFactory);

        while (true)
        {
            var current = Snapshot;
            var candidate = updateFactory(current)
                ?? throw new InvalidOperationException("An update cannot produce a null configuration.");

            Validate(candidate);

            if (ReferenceEquals(
                    Interlocked.CompareExchange(ref _snapshot, candidate, current),
                    current))
            {
                return candidate;
            }
        }
    }

    private static void Validate(ConfigurationSnapshot snapshot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(snapshot.EnvironmentName);
        ArgumentException.ThrowIfNullOrWhiteSpace(snapshot.LogLevel);

        if (!snapshot.ServiceEndpoint.IsAbsoluteUri)
        {
            throw new ArgumentException("The service endpoint must be an absolute URI.", nameof(snapshot));
        }

        if (snapshot.Revision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(snapshot), "The revision cannot be negative.");
        }
    }
}
