using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Models;
using NedMonitor.Tests.BackgroundServices.Fixtures;
using Xunit.Abstractions;

namespace NedMonitor.Tests.BackgroundServices;

public class NedMonitorBackgroundServiceTests(
    ITestOutputHelper output,
    NedMonitorBackgroundServiceTestsFixture fixture)
    : Test(output), IClassFixture<NedMonitorBackgroundServiceTestsFixture>
{
    private readonly NedMonitorBackgroundServiceTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given a queued snapshot, " +
        "When running NedMonitorBackgroundService, " +
        "Then it notifies the application")]
    [Trait("BackgroundServices", nameof(NedMonitor.BackgroundServices.NedMonitorBackgroundService))]
    public async Task ExecuteAsync_ProcessesSnapshot_AndNotifiesApplication()
    {
        //Given
        var services = _fixture.CreateServices();
        using var serviceProvider = services.Provider;
        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        var logger = NullLogger<NedMonitor.BackgroundServices.NedMonitorBackgroundService>.Instance;
        var service = new NedMonitorBackgroundServiceTestsFixture.TestableNedMonitorBackgroundService(scopeFactory, logger);
        var queue = services.Queue;
        var appMock = services.AppMock;
        var snapshot = new Snapshot();
        var notifyTcs = new TaskCompletionSource<Snapshot>(TaskCreationOptions.RunContinuationsAsynchronously);

        appMock
            .Setup(a => a.Notify(It.IsAny<Snapshot>()))
            .Returns<Snapshot>(s =>
            {
                notifyTcs.TrySetResult(s);
                return Task.CompletedTask;
            });

        queue.Enqueue(snapshot);
        queue.Complete();

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(250));

        //When
        var runTask = service.RunAsync(cts.Token);
        var completed = await Task.WhenAny(notifyTcs.Task, Task.Delay(1000));

        //Then
        completed.Should().Be(notifyTcs.Task);
        (await notifyTcs.Task).Should().BeSameAs(snapshot);

        cts.Cancel();

        try
        {
            await runTask;
        }
        catch (OperationCanceledException)
        {
        }
    }

    [Fact(DisplayName =
        "Given notify throws, " +
        "When running NedMonitorBackgroundService, " +
        "Then it logs and continues")]
    [Trait("BackgroundServices", nameof(NedMonitor.BackgroundServices.NedMonitorBackgroundService))]
    public async Task ExecuteAsync_NotifyThrows_DoesNotCrash()
    {
        //Given
        var services = _fixture.CreateServices();
        using var serviceProvider = services.Provider;
        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        var logger = NullLogger<NedMonitor.BackgroundServices.NedMonitorBackgroundService>.Instance;
        var service = new NedMonitorBackgroundServiceTestsFixture.TestableNedMonitorBackgroundService(scopeFactory, logger);
        var queue = services.Queue;
        var appMock = services.AppMock;

        appMock
            .Setup(a => a.Notify(It.IsAny<Snapshot>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        queue.Enqueue(new Snapshot());
        queue.Complete();

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(250));

        //When
        var runTask = service.RunAsync(cts.Token);
        await Task.Delay(50, cts.Token);
        cts.Cancel();

        //Then
        try
        {
            await runTask;
        }
        catch (OperationCanceledException)
        {
        }
        appMock.Verify(a => a.Notify(It.IsAny<Snapshot>()), Times.Once);
    }

    [Fact(DisplayName =
        "Given multiple snapshots, " +
        "When running NedMonitorBackgroundService, " +
        "Then it notifies for each item")]
    [Trait("BackgroundServices", nameof(NedMonitor.BackgroundServices.NedMonitorBackgroundService))]
    public async Task ExecuteAsync_MultipleSnapshots_NotifiesForEach()
    {
        //Given
        var services = _fixture.CreateServices();
        using var serviceProvider = services.Provider;
        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        var logger = NullLogger<NedMonitor.BackgroundServices.NedMonitorBackgroundService>.Instance;
        var service = new NedMonitorBackgroundServiceTestsFixture.TestableNedMonitorBackgroundService(scopeFactory, logger);
        var queue = services.Queue;
        var appMock = services.AppMock;

        queue.Enqueue(new Snapshot());
        queue.Enqueue(new Snapshot());
        queue.Complete();

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(250));

        //When
        var runTask = service.RunAsync(cts.Token);
        await Task.Delay(50, cts.Token);
        cts.Cancel();

        //Then
        try
        {
            await runTask;
        }
        catch (OperationCanceledException)
        {
        }

        appMock.Verify(a => a.Notify(It.IsAny<Snapshot>()), Times.Exactly(2));
    }
}
