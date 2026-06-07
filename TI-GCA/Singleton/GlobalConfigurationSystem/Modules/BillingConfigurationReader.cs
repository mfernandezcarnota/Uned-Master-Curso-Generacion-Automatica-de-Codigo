using GlobalConfigurationSystem.Configuration;

namespace GlobalConfigurationSystem.Modules;

public sealed class BillingConfigurationReader
{
    private readonly IGlobalConfiguration _configuration;

    public BillingConfigurationReader(IGlobalConfiguration? configuration = null)
    {
        _configuration = configuration ?? GlobalConfiguration.Current;
    }

    public Uri GetServiceEndpoint() => _configuration.Snapshot.ServiceEndpoint;
}
