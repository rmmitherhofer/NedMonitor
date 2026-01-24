using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpRequests;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class DiagnosticHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given diagnostic info, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(DiagnosticHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var info = new DiagnosticHttpRequest
        {
            MemoryUsageMb = 10,
            DbQueryCount = 2,
            CacheHit = true,
            Dependencies = new List<DependencyInfoHttpRequest>
            {
                new()
                {
                    Type = "HTTP",
                    Target = "https://example.local",
                    Success = true,
                    DurationMs = 10
                }
            }
        };

        //When
        var json = JsonSerializer.Serialize(info);

        //Then
        json.Should().Contain("\"memoryUsageMb\"");
        json.Should().Contain("\"dbQueryCount\"");
        json.Should().Contain("\"cacheHit\"");
        json.Should().Contain("\"dependencies\"");
        await Task.CompletedTask;
    }
}
