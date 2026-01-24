using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpRequests;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class DependencyInfoHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given dependency info, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(DependencyInfoHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var dependency = new DependencyInfoHttpRequest
        {
            Type = "HTTP",
            Target = "https://example.local",
            Success = true,
            DurationMs = 10
        };

        //When
        var json = JsonSerializer.Serialize(dependency);

        //Then
        json.Should().Contain("\"type\"");
        json.Should().Contain("\"target\"");
        json.Should().Contain("\"success\"");
        json.Should().Contain("\"durationMs\"");
        await Task.CompletedTask;
    }
}
