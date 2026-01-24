using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NedMonitor.Common.Tests;
using NedMonitor.Http.Configurations;
using NedMonitor.Http.Handlers;
using Xunit.Abstractions;

namespace NedMonitor.Http.Tests.Configurations;

public class NedMonitorHttpConfigurationTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given null services, " +
        "When adding NedMonitor Http, " +
        "Then it throws")]
    [Trait("Configurations", nameof(NedMonitorHttpConfiguration))]
    public async Task AddNedMonitorHttp_NullServices_Throws()
    {
        //Given
        IServiceCollection services = null!;

        //When
        var act = () => services.AddNedMonitorHttp();

        //Then
        act.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given services, " +
        "When adding NedMonitor Http twice, " +
        "Then it registers a single handler")]
    [Trait("Configurations", nameof(NedMonitorHttpConfiguration))]
    public async Task AddNedMonitorHttp_Twice_RegistersOnce()
    {
        //Given
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        services.AddOptions();

        //When
        services.AddNedMonitorHttp();
        services.AddNedMonitorHttp();

        //Then
        var provider = services.BuildServiceProvider();
        var handlers = provider.GetServices<NedMonitorHttpLoggingHandler>();
        handlers.Should().HaveCount(1);
        await Task.CompletedTask;
    }
}
