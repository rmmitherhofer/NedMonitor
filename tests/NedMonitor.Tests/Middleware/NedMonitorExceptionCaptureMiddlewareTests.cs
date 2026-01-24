using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core;
using NedMonitor.Core.Models;
using NedMonitor.Middleware;
using NedMonitor.Tests.Middleware.Fixtures;
using Xunit.Abstractions;

namespace NedMonitor.Tests.Middleware;

public class NedMonitorExceptionCaptureMiddlewareTests(
    ITestOutputHelper output,
    NedMonitorExceptionCaptureMiddlewareTestsFixture fixture)
    : Test(output), IClassFixture<NedMonitorExceptionCaptureMiddlewareTestsFixture>
{
    private readonly NedMonitorExceptionCaptureMiddlewareTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given an unexpected exception, " +
        "When invoking NedMonitorExceptionCaptureMiddleware, " +
        "Then it stores exception info and rethrows")]
    [Trait("Middleware", nameof(NedMonitorExceptionCaptureMiddleware))]
    public async Task Invoke_UnexpectedException_StoresExceptionInfo_AndRethrows()
    {
        //Given
        var context = _fixture.CreateContext();
        var options = _fixture.CreateOptions();
        var middleware = new NedMonitorExceptionCaptureMiddleware(_ => throw new InvalidOperationException("boom"), options);

        //When
        var act = () => middleware.Invoke(context);

        //Then
        await act.Should().ThrowAsync<InvalidOperationException>();
        context.Items[NedMonitorConstants.CONTEXT_EXCEPTION_KEY]
            .Should()
            .BeOfType<ExceptionInfo>();
    }

    [Fact(DisplayName =
        "Given an expected exception, " +
        "When invoking NedMonitorExceptionCaptureMiddleware, " +
        "Then it does not store exception info")]
    [Trait("Middleware", nameof(NedMonitorExceptionCaptureMiddleware))]
    public async Task Invoke_ExpectedException_DoesNotStoreExceptionInfo()
    {
        //Given
        var context = _fixture.CreateContext();
        var options = _fixture.CreateOptions(typeof(InvalidOperationException).FullName!);
        var middleware = new NedMonitorExceptionCaptureMiddleware(_ => throw new InvalidOperationException("boom"), options);

        //When
        var act = () => middleware.Invoke(context);

        //Then
        await act.Should().ThrowAsync<InvalidOperationException>();
        context.Items.ContainsKey(NedMonitorConstants.CONTEXT_EXCEPTION_KEY).Should().BeFalse();
    }

    [Fact(DisplayName =
        "Given an expected exception with different casing, " +
        "When invoking NedMonitorExceptionCaptureMiddleware, " +
        "Then it does not store exception info")]
    [Trait("Middleware", nameof(NedMonitorExceptionCaptureMiddleware))]
    public async Task Invoke_ExpectedException_IgnoresCase()
    {
        //Given
        var context = _fixture.CreateContext();
        var expected = typeof(InvalidOperationException).FullName!.ToLowerInvariant();
        var options = _fixture.CreateOptions(expected);
        var middleware = new NedMonitorExceptionCaptureMiddleware(_ => throw new InvalidOperationException("boom"), options);

        //When
        var act = () => middleware.Invoke(context);

        //Then
        await act.Should().ThrowAsync<InvalidOperationException>();
        context.Items.ContainsKey(NedMonitorConstants.CONTEXT_EXCEPTION_KEY).Should().BeFalse();
    }
}
