using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Enums;
using NedMonitor.HttpRequests;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class OrmSettingsHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given ORM settings, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(ORMSettingsHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var settings = new TestOrmSettingsHttpRequest
        {
            Enabled = true,
            CaptureOptions = [CaptureOptions.Query, CaptureOptions.Parameters]
        };

        //When
        var json = JsonSerializer.Serialize(settings);

        //Then
        json.Should().Contain("\"enabled\"");
        json.Should().Contain("\"captureOptions\"");
        await Task.CompletedTask;
    }

    private sealed class TestOrmSettingsHttpRequest : ORMSettingsHttpRequest
    {
    }
}
