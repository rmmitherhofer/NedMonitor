using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpRequests;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class LogEntryHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given log entry info, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(LogEntryHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var entry = new LogEntryHttpRequest
        {
            LogCategory = "category",
            LogSeverity = Microsoft.Extensions.Logging.LogLevel.Information,
            LogMessage = "message",
            MemberType = "Type",
            MemberName = "Method",
            SourceLineNumber = 10,
            Timestamp = DateTime.UtcNow
        };

        //When
        var json = JsonSerializer.Serialize(entry);

        //Then
        json.Should().Contain("\"logCategory\"");
        json.Should().Contain("\"logSeverity\"");
        json.Should().Contain("\"logMessage\"");
        json.Should().Contain("\"sourceLineNumber\"");
        await Task.CompletedTask;
    }
}
