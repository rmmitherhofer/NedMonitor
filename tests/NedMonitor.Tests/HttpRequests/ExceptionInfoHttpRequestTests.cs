using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpRequests;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class ExceptionInfoHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given exception info, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(ExceptionInfoHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var info = new ExceptionInfoHttpRequest
        {
            Type = "Exception",
            Message = "message",
            Tracer = "trace",
            InnerException = "inner",
            Timestamp = DateTime.UtcNow,
            Source = "source"
        };

        //When
        var json = JsonSerializer.Serialize(info);

        //Then
        json.Should().Contain("\"type\"");
        json.Should().Contain("\"message\"");
        json.Should().Contain("\"tracer\"");
        json.Should().Contain("\"innerException\"");
        json.Should().Contain("\"timestamp\"");
        await Task.CompletedTask;
    }
}
