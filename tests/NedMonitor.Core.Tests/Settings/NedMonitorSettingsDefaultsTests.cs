using FluentAssertions;
using Microsoft.Extensions.Logging;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Settings;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Settings;

public class NedMonitorSettingsDefaultsTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given default settings, " +
        "When created, " +
        "Then it initializes nested defaults")]
    [Trait("Settings", nameof(NedMonitorSettings))]
    public async Task NedMonitorSettings_Defaults_AreExpected()
    {
        //Given
        var settings = new NedMonitorSettings();

        //When
        var name = settings.Name;

        //Then
        name.Should().NotBeNullOrWhiteSpace();
        settings.MinimumLogLevel.Should().Be(LogLevel.Information);
        settings.ExecutionMode.Should().NotBeNull();
        settings.HttpLogging.Should().NotBeNull();
        settings.SensitiveDataMasking.Should().NotBeNull();
        settings.Exceptions.Should().NotBeNull();
        settings.DataInterceptors.Should().NotBeNull();
        settings.RemoteService.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given remote service settings, " +
        "When setting properties, " +
        "Then it stores the values")]
    [Trait("Settings", nameof(RemoteServiceSettings))]
    public async Task RemoteServiceSettings_SetAndGet()
    {
        //Given
        var settings = new RemoteServiceSettings
        {
            BaseAddress = "https://api.example.local",
            Endpoint = "/notify"
        };

        //When
        var baseAddress = settings.BaseAddress;

        //Then
        baseAddress.Should().Be("https://api.example.local");
        settings.Endpoint.Should().NotBeNull();
        settings.Endpoint.Should().Be("/notify");
        await Task.CompletedTask;
    }
}
