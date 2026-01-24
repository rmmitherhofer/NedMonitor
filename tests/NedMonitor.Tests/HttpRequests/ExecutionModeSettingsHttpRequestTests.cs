using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpRequests;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class ExecutionModeSettingsHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given execution mode settings, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(ExecutionModeSettingsHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var settings = new ExecutionModeSettingsHttpRequest
        {
            EnableNedMonitor = true,
            EnableMonitorExceptions = true,
            EnableMonitorNotifications = true,
            EnableMonitorLogs = true,
            EnableMonitorHttpRequests = true,
            EnableMonitorDbQueries = true
        };

        //When
        var json = JsonSerializer.Serialize(settings);

        //Then
        json.Should().Contain("\"enableNedMonitor\"");
        json.Should().Contain("\"enableMonitorExceptions\"");
        json.Should().Contain("\"enableMonitorNotifications\"");
        json.Should().Contain("\"enableMonitorLogs\"");
        json.Should().Contain("\"enableMonitorHttpRequests\"");
        json.Should().Contain("\"enableMonitorDbQueries\"");
        await Task.CompletedTask;
    }
}
