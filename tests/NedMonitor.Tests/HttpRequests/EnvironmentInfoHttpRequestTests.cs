using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpRequests;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class EnvironmentInfoHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given environment info, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(EnvironmentInfoHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var info = new EnvironmentInfoHttpRequest
        {
            MachineName = "machine",
            Name = "dev",
            ApplicationVersion = "1.0.0",
            ThreadId = 1
        };

        //When
        var json = JsonSerializer.Serialize(info);

        //Then
        json.Should().Contain("\"machineName\"");
        json.Should().Contain("\"name\"");
        json.Should().Contain("\"applicationVersion\"");
        json.Should().Contain("\"threadId\"");
        await Task.CompletedTask;
    }
}
