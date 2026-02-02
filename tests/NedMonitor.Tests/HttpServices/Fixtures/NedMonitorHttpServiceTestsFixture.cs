using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NedMonitor.Core.Enums;
using NedMonitor.Core.Settings;
using NedMonitor.HttpRequests;
using NedMonitor.HttpServices;
using System.Net;
using System.Net.Http.Headers;

namespace NedMonitor.Tests.HttpServices.Fixtures;

public sealed class NedMonitorHttpServiceTestsFixture
{
    public NedMonitorSettings CreateSettings(bool writePayloadToConsole = false)
        => new()
        {
            HttpLogging = new HttpLoggingSettings
            {
                WritePayloadToConsole = writePayloadToConsole
            },
            RemoteService = new RemoteServiceSettings
            {
                BaseAddress = "https://example.local/",
                Endpoint = "/logs"
            }
        };

    internal LogContextHttpRequest CreateLog(
        string correlationId = "corr-id",
        string? clientId = "client-id",
        string? ipAddress = "127.0.0.1",
        string? userId = "user-id",
        string? userAgent = "user-agent",
        string? account = "account",
        string? accountCode = "account-code")
    {
        return new LogContextHttpRequest
        {
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow,
            LogAttentionLevel = LogAttentionLevel.Low,
            CorrelationId = correlationId,
            Uri = "/api/test",
            TotalMilliseconds = 10,
            TraceIdentifier = "trace",
            RemotePort = 10,
            LocalPort = 20,
            LocalIpAddress = "127.0.0.1",
            Project = new ProjectInfoHttp
            {
                Id = Guid.NewGuid(),
                Type = ProjectType.Api,
                Name = "NedMonitor",
                ExecutionMode = new ExecutionModeSettingsHttpRequest(),
                MinimumLogLevel = LogLevel.Information
            },
            Environment = new EnvironmentInfoHttpRequest
            {
                MachineName = "machine",
                Name = "dev",
                ApplicationVersion = "1.0.0",
                ThreadId = Environment.CurrentManagedThreadId
            },
            User = new UserInfoHttpRequest
            {
                Id = userId,
                Account = account,
                AccountCode = accountCode
            },
            Request = new RequestInfoHttpRequest
            {
                Id = "req-id",
                HttpMethod = HttpMethod.Get.Method,
                FullPath = "https://example.local/api/test",
                Scheme = "https",
                Protocol = "HTTP/1.1",
                IsHttps = true,
                QueryString = "",
                RouteValues = new Dictionary<string, string>(),
                UserAgent = userAgent ?? string.Empty,
                ClientId = clientId ?? string.Empty,
                ContentType = "application/json",
                ContentLength = 0,
                Body = null,
                BodySize = 0,
                IsAjaxRequest = false,
                IpAddress = ipAddress,
                Host = "example.local",
                PathBase = "",
                Path = "/api/test",
                Headers = new Dictionary<string, List<string>>()
            },
            Response = new ResponseInfoHttpRequest
            {
                StatusCode = HttpStatusCode.OK,
                ReasonPhrase = "OK",
                Headers = new Dictionary<string, List<string>>(),
                Body = null,
                BodySize = 0
            },
            Diagnostic = new DiagnosticHttpRequest
            {
                MemoryUsageMb = 10,
                DbQueryCount = 0,
                CacheHit = false,
                Dependencies = new List<DependencyInfoHttpRequest>()
            }
        };
    }

    internal (NedMonitorHttpService Service, TestHttpMessageHandler Handler) CreateService(
        NedMonitorSettings settings,
        Func<HttpRequestMessage, HttpResponseMessage>? responseFactory = null)
    {
        responseFactory ??= _ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        };

        var handler = new TestHttpMessageHandler(responseFactory)
        {
            ExpectContentType = "application/json"
        };

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri(settings.RemoteService.BaseAddress)
        };
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var logger = new Mock<ILogger<NedMonitorHttpService>>();

        var service = new NedMonitorHttpService(
            client,
            logger.Object,
            Options.Create(settings));

        return (service, handler);
    }

    public sealed class TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public string? ExpectContentType { get; init; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;

            if (!string.IsNullOrEmpty(ExpectContentType))
            {
                request.Content!.Headers.ContentType!.MediaType.Should().Be(ExpectContentType);
            }

            return Task.FromResult(handler(request));
        }
    }
}
