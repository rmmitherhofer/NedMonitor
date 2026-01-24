using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NedMonitor.Applications;
using NedMonitor.BackgroundServices;
using NedMonitor.Core.Models;
using NedMonitor.Queues;
using System.Threading.Channels;
using Moq;

namespace NedMonitor.Tests.BackgroundServices.Fixtures;

public sealed class NedMonitorBackgroundServiceTestsFixture
{
    public (ServiceProvider Provider, FakeNedMonitorQueue Queue, Mock<INedMonitorApplication> AppMock) CreateServices()
    {
        var services = new ServiceCollection();
        var queue = new FakeNedMonitorQueue();
        var appMock = new Mock<INedMonitorApplication>();

        services.AddSingleton<INedMonitorQueue>(queue);
        services.AddSingleton(appMock.Object);

        return (services.BuildServiceProvider(), queue, appMock);
    }

    public sealed class FakeNedMonitorQueue : INedMonitorQueue
    {
        private readonly Channel<Snapshot> _channel = Channel.CreateUnbounded<Snapshot>();

        public ChannelReader<Snapshot> Reader => _channel.Reader;

        public void Enqueue(Snapshot snapshot)
            => _channel.Writer.TryWrite(snapshot);

        public void Complete()
            => _channel.Writer.TryComplete();
    }

    public sealed class TestableNedMonitorBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<NedMonitorBackgroundService> logger)
        : NedMonitorBackgroundService(scopeFactory, logger)
    {
        public Task RunAsync(CancellationToken token) => ExecuteAsync(token);
    }
}
