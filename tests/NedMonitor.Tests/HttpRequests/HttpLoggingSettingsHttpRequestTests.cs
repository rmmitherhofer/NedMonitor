using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpRequests;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class HttpLoggingSettingsHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given http logging settings, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(HttpLoggingSettingsHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var settings = new HttpLoggingSettingsHttpRequest
        {
            WritePayloadToConsole = true,
            CaptureResponseBody = true,
            MaxResponseBodySizeInMb = 5,
            CaptureCookies = true
        };

        //When
        var json = JsonSerializer.Serialize(settings);

        //Then
        json.Should().Contain("\"writePayloadToConsole\"");
        json.Should().Contain("\"captureResponseBody\"");
        json.Should().Contain("\"maxResponseBodySizeInMb\"");
        json.Should().Contain("\"captureCookies\"");
        await Task.CompletedTask;
    }
}
