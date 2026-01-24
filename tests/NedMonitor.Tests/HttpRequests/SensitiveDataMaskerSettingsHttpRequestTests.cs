using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpRequests;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class SensitiveDataMaskerSettingsHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given sensitive data masking settings, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(SensitiveDataMaskerSettingsHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var settings = new SensitiveDataMaskerSettingsHttpRequest
        {
            Enabled = true,
            SensitiveKeys = ["token"],
            SensitivePatterns = ["\\d+"],
            MaskValue = "***"
        };

        //When
        var json = JsonSerializer.Serialize(settings);

        //Then
        json.Should().Contain("\"enabled\"");
        json.Should().Contain("\"sensitiveKeys\"");
        json.Should().Contain("\"sensitivePatterns\"");
        json.Should().Contain("\"maskValue\"");
        await Task.CompletedTask;
    }
}
