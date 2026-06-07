using System.Reflection;
using GlobalConfigurationSystem.Configuration;
using GlobalConfigurationSystem.Modules;

namespace GlobalConfigurationSystem.Tests;

public sealed class GlobalConfigurationTests
{
    [Fact]
    public void MultipleModules_ReadTheSameSharedConfiguration()
    {
        var expectedEnvironment = $"Integration-{Guid.NewGuid():N}";
        var expectedEndpoint = new Uri($"https://{Guid.NewGuid():N}.example.com/");
        GlobalConfiguration.Current.Update(current => current with
        {
            EnvironmentName = expectedEnvironment,
            ServiceEndpoint = expectedEndpoint,
            Revision = current.Revision + 1
        });

        var auditModule = new AuditConfigurationReader();
        var billingModule = new BillingConfigurationReader();

        Assert.Equal(expectedEnvironment, auditModule.GetEnvironmentName());
        Assert.Equal(expectedEndpoint, billingModule.GetServiceEndpoint());
    }

    [Fact]
    public void Current_AlwaysReturnsTheSameInstance()
    {
        var first = GlobalConfiguration.Current;
        var second = GlobalConfiguration.Current;

        Assert.Same(first, second);
    }

    [Fact]
    public void Type_CannotBeCreatedThroughItsPublicApi()
    {
        var publicConstructors = typeof(GlobalConfiguration)
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance);

        Assert.Empty(publicConstructors);
        Assert.Throws<MissingMethodException>(() => Activator.CreateInstance(typeof(GlobalConfiguration)));
        Assert.True(typeof(GlobalConfiguration).IsSealed);
    }

    [Fact]
    public async Task ConcurrentAccess_ReturnsOneInstance()
    {
        var accesses = Enumerable.Range(0, 1_000)
            .Select(_ => Task.Run(() => GlobalConfiguration.Current));

        var instances = await Task.WhenAll(accesses);

        Assert.All(instances, instance => Assert.Same(instances[0], instance));
    }

    [Fact]
    public async Task ConcurrentUpdates_PreserveEveryChangeAndSharedConsistency()
    {
        const int updateCount = 500;
        var configuration = GlobalConfiguration.Current;
        var baseline = configuration.Snapshot.Revision;

        var updates = Enumerable.Range(0, updateCount)
            .Select(_ => Task.Run(() => configuration.Update(current => current with
            {
                Revision = current.Revision + 1
            })));

        await Task.WhenAll(updates);

        Assert.Equal(baseline + updateCount, configuration.Snapshot.Revision);
        Assert.Same(configuration.Snapshot, GlobalConfiguration.Current.Snapshot);
    }
}
