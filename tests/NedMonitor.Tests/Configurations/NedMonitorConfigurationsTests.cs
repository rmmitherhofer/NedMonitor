using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NedMonitor.Common.Tests;
using NedMonitor.Configurations;
using NedMonitor.Core;
using NedMonitor.Core.Settings;
using NedMonitor.Queues;
using NedMonitor.Applications;
using NedMonitor.Builders;
using Xunit.Abstractions;

namespace NedMonitor.Tests.Configurations;

public class NedMonitorConfigurationsTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given null services, " +
        "When adding NedMonitor, " +
        "Then it throws")]
    [Trait("Configurations", nameof(NedMonitorConfigurations))]
    public async Task AddNedMonitor_NullServices_Throws()
    {
        //Given
        IServiceCollection services = null!;
        var configuration = BuildConfiguration(enableNedMonitor: true);

        //When
        var act = () => services.AddNedMonitor(configuration);

        //Then
        act.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given null configuration, " +
        "When adding NedMonitor, " +
        "Then it throws")]
    [Trait("Configurations", nameof(NedMonitorConfigurations))]
    public async Task AddNedMonitor_NullConfiguration_Throws()
    {
        //Given
        var services = new ServiceCollection();
        IConfiguration configuration = null!;

        //When
        var act = () => services.AddNedMonitor(configuration);

        //Then
        act.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given monitor disabled, " +
        "When adding NedMonitor, " +
        "Then it does not register core services")]
    [Trait("Configurations", nameof(NedMonitorConfigurations))]
    public async Task AddNedMonitor_Disabled_DoesNotRegisterCoreServices()
    {
        //Given
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(enableNedMonitor: false);

        //When
        services.AddNedMonitor(configuration);

        //Then
        services.Any(s => s.ServiceType == typeof(INedMonitorApplication)).Should().BeFalse();
        services.Any(s => s.ServiceType == typeof(ILogContextBuilder)).Should().BeFalse();
        services.Any(s => s.ServiceType == typeof(INedMonitorQueue)).Should().BeFalse();
        services.Any(s => s.ServiceType == typeof(ILoggerProvider)).Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given log monitoring enabled, " +
        "When adding NedMonitor, " +
        "Then it registers logger provider")]
    [Trait("Configurations", nameof(NedMonitorConfigurations))]
    public async Task AddNedMonitor_EnableLogs_RegistersLoggerProvider()
    {
        //Given
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(enableNedMonitor: true, enableMonitorLogs: true);

        //When
        services.AddNedMonitor(configuration);

        //Then
        services.Any(s => s.ServiceType == typeof(ILoggerProvider)).Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given db query monitor enabled, " +
        "When adding options, " +
        "Then it enables interceptors and merges sensitive keys")]
    [Trait("Configurations", nameof(NedMonitorConfigurations))]
    public async Task AddOptions_DbQueriesEnabled_EnablesInterceptorsAndMergesKeys()
    {
        //Given
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(
            enableNedMonitor: true,
            enableMonitorDbQueries: true,
            sensitiveKeys: ["token", "secret"]);

        //When
        services.AddOptions(configuration);
        var provider = services.BuildServiceProvider();
        var settings = provider.GetRequiredService<IOptions<NedMonitorSettings>>().Value;

        //Then
        settings.DataInterceptors.EF.Enabled.Should().BeTrue();
        settings.DataInterceptors.Dapper.Enabled.Should().BeTrue();
        settings.SensitiveDataMasking.SensitiveKeys.Should().Contain("token");
        settings.SensitiveDataMasking.SensitiveKeys.Should().Contain("secret");
        settings.SensitiveDataMasking.SensitiveKeys.Should().Contain(NedMonitorConstants.DEFAULT_KEYS);
        await Task.CompletedTask;
    }

    private static IConfiguration BuildConfiguration(
        bool enableNedMonitor,
        bool enableMonitorLogs = false,
        bool enableMonitorDbQueries = false,
        IEnumerable<string>? sensitiveKeys = null)
    {
        var data = new Dictionary<string, string?>
        {
            [$"{NedMonitorSettings.NEDMONITOR_NODE}:ProjectId"] = "11111111-1111-1111-1111-111111111111",
            [$"{NedMonitorSettings.NEDMONITOR_NODE}:ProjectType"] = "Api",
            [$"{NedMonitorSettings.NEDMONITOR_NODE}:ExecutionMode:EnableNedMonitor"] = enableNedMonitor.ToString(),
            [$"{NedMonitorSettings.NEDMONITOR_NODE}:ExecutionMode:EnableMonitorLogs"] = enableMonitorLogs.ToString(),
            [$"{NedMonitorSettings.NEDMONITOR_NODE}:ExecutionMode:EnableMonitorDbQueries"] = enableMonitorDbQueries.ToString(),
            [$"{NedMonitorSettings.NEDMONITOR_NODE}:RemoteService:BaseAddress"] = "https://example.com/",
            [$"{NedMonitorSettings.NEDMONITOR_NODE}:RemoteService:Endpoints:NotifyLogContext"] = "/logs"
        };

        if (sensitiveKeys is not null)
        {
            int index = 0;
            foreach (var key in sensitiveKeys)
            {
                data[$"{NedMonitorSettings.NEDMONITOR_NODE}:SensitiveDataMasking:SensitiveKeys:{index}"] = key;
                index++;
            }
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(data)
            .Build();
    }
}
