using FluentAssertions;
using Moq;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Models;
using NedMonitor.Middleware;
using NedMonitor.Tests.Middleware.Fixtures;
using Xunit.Abstractions;

namespace NedMonitor.Tests.Middleware;

public class NedMonitorMiddlewareTests(
    ITestOutputHelper output,
    NedMonitorMiddlewareTestsFixture fixture)
    : Test(output), IClassFixture<NedMonitorMiddlewareTestsFixture>
{
    private readonly NedMonitorMiddlewareTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given a request pipeline, " +
        "When invoking NedMonitorMiddleware, " +
        "Then it enqueues a snapshot")]
    [Trait("Middleware", nameof(NedMonitorMiddleware))]
    public async Task InvokeAsync_Success_EnqueuesSnapshot()
    {
        //Given
        var context = _fixture.CreateContext();
        var queueMock = _fixture.CreateQueueMock();
        var middleware = new NedMonitorMiddleware(_ => Task.CompletedTask);
        Snapshot? captured = null;
        queueMock.Setup(q => q.Enqueue(It.IsAny<Snapshot>()))
            .Callback<Snapshot>(snapshot => captured = snapshot);

        //When
        await middleware.InvokeAsync(context, queueMock.Object);

        //Then
        queueMock.Verify(q => q.Enqueue(It.IsAny<Snapshot>()), Times.Once);
        captured.Should().NotBeNull();
    }

    [Fact(DisplayName =
        "Given a request that throws, " +
        "When invoking NedMonitorMiddleware, " +
        "Then it rethrows and still enqueues a snapshot")]
    [Trait("Middleware", nameof(NedMonitorMiddleware))]
    public async Task InvokeAsync_Throws_EnqueuesSnapshot_AndRethrows()
    {
        //Given
        var context = _fixture.CreateContext();
        var queueMock = _fixture.CreateQueueMock();
        var middleware = new NedMonitorMiddleware(_ => throw new InvalidOperationException("boom"));

        //When
        var act = () => middleware.InvokeAsync(context, queueMock.Object);

        //Then
        await act.Should().ThrowAsync<InvalidOperationException>();
        queueMock.Verify(q => q.Enqueue(It.IsAny<Snapshot>()), Times.Once);
    }
}
