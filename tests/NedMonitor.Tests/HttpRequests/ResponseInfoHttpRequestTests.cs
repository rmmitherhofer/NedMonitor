using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpRequests;
using System.Net;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class ResponseInfoHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given a response info payload, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(ResponseInfoHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var response = new ResponseInfoHttpRequest
        {
            StatusCode = HttpStatusCode.OK,
            ReasonPhrase = "OK",
            Headers = new Dictionary<string, List<string>>(),
            Body = null,
            BodySize = 0
        };

        //When
        var json = JsonSerializer.Serialize(response);

        //Then
        json.Should().Contain("\"statusCode\"");
        json.Should().Contain("\"reasonPhrase\"");
        json.Should().Contain("\"bodySize\"");
        await Task.CompletedTask;
    }
}
