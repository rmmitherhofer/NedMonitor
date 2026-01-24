using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Enums;
using NedMonitor.HttpRequests;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class LogContextHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given a log context request, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(LogContextHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var request = new LogContextHttpRequest
        {
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow,
            LogAttentionLevel = LogAttentionLevel.Low,
            CorrelationId = "corr",
            Uri = "/api/test",
            TotalMilliseconds = 12,
            Project = new ProjectInfoHttp
            {
                Id = Guid.NewGuid(),
                Type = ProjectType.Api,
                Name = "NedMonitor",
                ExecutionMode = new ExecutionModeSettingsHttpRequest(),
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information
            },
            Environment = new EnvironmentInfoHttpRequest
            {
                MachineName = "machine",
                Name = "dev",
                ApplicationVersion = "1.0.0",
                ThreadId = 1
            },
            User = new UserInfoHttpRequest(),
            Request = new RequestInfoHttpRequest
            {
                Id = "id",
                HttpMethod = "GET",
                FullPath = "https://example.local",
                Scheme = "https",
                Protocol = "HTTP/1.1",
                IsHttps = true,
                QueryString = "",
                RouteValues = new Dictionary<string, string>(),
                UserAgent = "ua",
                ClientId = "cid",
                ContentType = "application/json",
                ContentLength = 0,
                Body = null,
                BodySize = 0,
                IsAjaxRequest = false,
                Host = "example.local",
                PathBase = "",
                Path = "/api/test"
            },
            Response = new ResponseInfoHttpRequest
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                ReasonPhrase = "OK",
                Headers = new Dictionary<string, List<string>>(),
                Body = null,
                BodySize = 0
            },
            Diagnostic = new DiagnosticHttpRequest
            {
                MemoryUsageMb = 1,
                DbQueryCount = 0,
                CacheHit = false,
                Dependencies = new List<DependencyInfoHttpRequest>()
            }
        };

        //When
        var json = JsonSerializer.Serialize(request);

        //Then
        json.Should().Contain("\"startTime\"");
        json.Should().Contain("\"endTime\"");
        json.Should().Contain("\"correlationId\"");
        json.Should().Contain("\"uriTemplate\"");
        json.Should().Contain("\"totalMilliseconds\"");
        await Task.CompletedTask;
    }
}
