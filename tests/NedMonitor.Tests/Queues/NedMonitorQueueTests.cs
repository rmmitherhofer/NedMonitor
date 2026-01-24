using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Models;
using NedMonitor.Queues;
using Xunit.Abstractions;

namespace NedMonitor.Tests.Queues;

public class NedMonitorQueueTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given a snapshot, " +
        "When enqueuing, " +
        "Then it can be read from the queue")]
    [Trait("Queues", nameof(NedMonitorQueue))]
    public async Task Enqueue_ReadsSnapshotFromQueue()
    {
        //Given
        var queue = new NedMonitorQueue();
        var snapshot = new Snapshot
        {
            CorrelationId = "corr",
            RequestId = "req",
            Scheme = "https",
            Protocol = "HTTP/1.1",
            PathBase = string.Empty,
            Path = "/",
            FullPath = "https://example.com/",
            QueryString = string.Empty,
            RouteValues = new Dictionary<string, string>(),
            UserAgent = "agent",
            ClientId = "client",
            RequestHeaders = new Dictionary<string, List<string>>(),
            Host = "example.com",
            RequestContentType = "text/plain",
            Method = "GET",
            LocalIpAddress = "127.0.0.1",
            Roles = new List<string>(),
            Claims = new Dictionary<string, string>()
        };

        //When
        queue.Enqueue(snapshot);

        //Then
        queue.Reader.TryRead(out var read).Should().BeTrue();
        read.Should().BeSameAs(snapshot);
        await Task.CompletedTask;
    }
}
