using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpRequests;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class NotificationInfoHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given notification info, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(NotificationInfoHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var notification = new NotificationInfoHttpRequest
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            LogLevel = Microsoft.Extensions.Logging.LogLevel.Warning,
            Key = "key",
            Value = "value",
            Detail = "detail"
        };

        //When
        var json = JsonSerializer.Serialize(notification);

        //Then
        json.Should().Contain("\"logLevel\"");
        json.Should().Contain("\"key\"");
        json.Should().Contain("\"value\"");
        json.Should().Contain("\"detail\"");
        await Task.CompletedTask;
    }
}
