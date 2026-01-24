using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpRequests;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class RequestInfoHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given a request info payload, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(RequestInfoHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var request = new RequestInfoHttpRequest
        {
            Id = "id",
            HttpMethod = "POST",
            FullPath = "https://example.local",
            Scheme = "https",
            Protocol = "HTTP/1.1",
            IsHttps = true,
            QueryString = "q=1",
            RouteValues = new Dictionary<string, string>(),
            UserAgent = "ua",
            ClientId = "cid",
            ContentType = "application/json",
            ContentLength = 10,
            Body = null,
            BodySize = 10,
            IsAjaxRequest = true,
            IpAddress = "127.0.0.1",
            Host = "example.local",
            PathBase = "",
            Path = "/api/test",
            Referer = "https://ref.local",
            Cookies = new Dictionary<string, string>(),
            HasFormContentType = true
        };

        //When
        var json = JsonSerializer.Serialize(request);

        //Then
        json.Should().Contain("\"httpMethod\"");
        json.Should().Contain("\"fullPath\"");
        json.Should().Contain("\"contentType\"");
        json.Should().Contain("\"hasFormContentType\"");
        await Task.CompletedTask;
    }
}
