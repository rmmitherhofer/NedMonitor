using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Enums;
using NedMonitor.HttpRequests;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class EfInterceptorSettingsHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given EF interceptor settings, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(EfInterceptorSettingsHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var settings = new EfInterceptorSettingsHttpRequest
        {
            Enabled = true,
            CaptureOptions = [CaptureOptions.Query]
        };

        //When
        var json = JsonSerializer.Serialize(settings);

        //Then
        json.Should().Contain("\"enabled\"");
        json.Should().Contain("\"captureOptions\"");
        await Task.CompletedTask;
    }
}
