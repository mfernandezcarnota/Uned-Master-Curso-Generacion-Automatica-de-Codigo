using GlobalConfigurationSystem.Configuration;

namespace GlobalConfigurationSystem.Modules;

public sealed class AuditConfigurationReader
{
    private readonly IGlobalConfiguration _configuration;

    public AuditConfigurationReader(IGlobalConfiguration? configuration = null)
    {
        _configuration = configuration ?? GlobalConfiguration.Current;
    }

    public string GetEnvironmentName() => _configuration.Snapshot.EnvironmentName;
}
