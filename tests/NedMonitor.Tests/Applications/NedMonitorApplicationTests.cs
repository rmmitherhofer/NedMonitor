using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NedMonitor.Applications;
using NedMonitor.Builders;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Models;
using NedMonitor.Core.Settings;
using NedMonitor.HttpRequests;
using NedMonitor.HttpServices;
using Xunit.Abstractions;

namespace NedMonitor.Tests.Applications;

public class NedMonitorApplicationTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given monitoring disabled, " +
        "When notifying, " +
        "Then it does not build or flush")]
    [Trait("Applications", nameof(NedMonitorApplication))]
    public async Task Notify_Disabled_DoesNotBuildOrFlush()
    {
        //Given
        var settings = new NedMonitorSettings
        {
            ExecutionMode = new ExecutionModeSettings
            {
                EnableNedMonitor = false
            }
        };
        var httpService = new Mock<INedMonitorHttpService>();
        var builder = new Mock<ILogContextBuilder>();
        var app = new NedMonitorApplication(httpService.Object, Options.Create(settings), builder.Object);

        //When
        await app.Notify(CreateSnapshot());

        //Then
        builder.Verify(b => b.WithSnapshot(It.IsAny<Snapshot>()), Times.Never);
        httpService.Verify(s => s.Flush(It.IsAny<LogContextHttpRequest>()), Times.Never);
    }

    [Fact(DisplayName =
        "Given monitoring enabled, " +
        "When notifying, " +
        "Then it applies strategies and flushes")]
    [Trait("Applications", nameof(NedMonitorApplication))]
    public async Task Notify_Enabled_InvokesStrategiesAndFlushes()
    {
        //Given
        var settings = new NedMonitorSettings
        {
            ExecutionMode = new ExecutionModeSettings
            {
                EnableNedMonitor = true,
                EnableMonitorExceptions = true,
                EnableMonitorHttpRequests = true,
                EnableMonitorNotifications = true,
                EnableMonitorLogs = true,
                EnableMonitorDbQueries = true
            }
        };
        var httpService = new Mock<INedMonitorHttpService>();
        var builder = new Mock<ILogContextBuilder>();
        var built = new LogContextHttpRequest();

        builder.Setup(b => b.WithSnapshot(It.IsAny<Snapshot>())).Returns(builder.Object);
        builder.Setup(b => b.WithException()).Returns(builder.Object);
        builder.Setup(b => b.WithHttpClientLogs()).Returns(builder.Object);
        builder.Setup(b => b.WithNotifications()).Returns(builder.Object);
        builder.Setup(b => b.WithLogEntries()).Returns(builder.Object);
        builder.Setup(b => b.WithDbQueryLogs()).Returns(builder.Object);
        builder.Setup(b => b.Build()).Returns(built);

        var app = new NedMonitorApplication(httpService.Object, Options.Create(settings), builder.Object);

        //When
        await app.Notify(CreateSnapshot());

        //Then
        builder.Verify(b => b.WithSnapshot(It.IsAny<Snapshot>()), Times.Once);
        builder.Verify(b => b.WithException(), Times.Once);
        builder.Verify(b => b.WithHttpClientLogs(), Times.Once);
        builder.Verify(b => b.WithNotifications(), Times.Once);
        builder.Verify(b => b.WithLogEntries(), Times.Once);
        builder.Verify(b => b.WithDbQueryLogs(), Times.Once);
        builder.Verify(b => b.Build(), Times.Once);
        httpService.Verify(s => s.Flush(built), Times.Once);
    }

    private static Snapshot CreateSnapshot()
    {
        return new Snapshot
        {
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow,
            CorrelationId = "corr",
            Scheme = "https",
            Host = "example.com",
            Path = "/api/test",
            PathTemplate = "api/test",
            TotalMilliseconds = 10,
            TraceId = "trace",
            RemotePort = 1,
            LocalPort = 2,
            LocalIpAddress = "127.0.0.1",
            RequestId = "req",
            Method = "GET",
            PathBase = string.Empty,
            FullPath = "https://example.com/api/test",
            QueryString = string.Empty,
            RouteValues = new Dictionary<string, string>(),
            UserAgent = "agent",
            ClientId = "client",
            RequestHeaders = new Dictionary<string, List<string>>(),
            RequestContentType = "application/json",
            RequestContentLength = 0,
            IsAjaxRequest = false,
            IpAddress = "127.0.0.1",
            StatusCode = 200,
            ResponseHeaders = new Dictionary<string, List<string>>(),
            ResponseBody = null,
            ResponseBodySize = 0,
            ThreadId = 1,
            Roles = new List<string>(),
            Claims = new Dictionary<string, string>()
        };
    }
}
