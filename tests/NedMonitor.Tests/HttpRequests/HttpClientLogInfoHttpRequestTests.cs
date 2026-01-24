using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpRequests;
using System.Net;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class HttpClientLogInfoHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given http client log info, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(HttpClientLogInfoHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var info = new HttpClientLogInfoHttpRequest
        {
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow,
            Method = "GET",
            Url = "https://example.local",
            TemplateUrl = "/template",
            StatusCode = HttpStatusCode.OK,
            RequestBody = null,
            ResponseBody = null,
            RequestHeaders = new Dictionary<string, List<string>>(),
            ResponseHeaders = new Dictionary<string, List<string>>(),
            ExceptionType = "Exception",
            ExceptionMessage = "message",
            StackTrace = "trace",
            InnerException = "inner"
        };

        //When
        var json = JsonSerializer.Serialize(info);

        //Then
        json.Should().Contain("\"startTime\"");
        json.Should().Contain("\"endTime\"");
        json.Should().Contain("\"requestHeaders\"");
        json.Should().Contain("\"responseHeaders\"");
        json.Should().Contain("\"exceptionType\"");
        json.Should().Contain("\"innerException\"");
        await Task.CompletedTask;
    }
}
