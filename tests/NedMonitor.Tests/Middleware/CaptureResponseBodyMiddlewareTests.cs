using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core;
using NedMonitor.Middleware;
using NedMonitor.Tests.Middleware.Fixtures;
using System.Text;
using Xunit.Abstractions;

namespace NedMonitor.Tests.Middleware;

public class CaptureResponseBodyMiddlewareTests(
    ITestOutputHelper output,
    CaptureResponseBodyMiddlewareTestsFixture fixture)
    : Test(output), IClassFixture<CaptureResponseBodyMiddlewareTestsFixture>
{
    private readonly CaptureResponseBodyMiddlewareTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given capture enabled and body within limit, " +
        "When invoking CaptureResponseBodyMiddleware, " +
        "Then it stores the response body and size")]
    [Trait("Middleware", nameof(CaptureResponseBodyMiddleware))]
    public async Task InvokeAsync_CaptureEnabled_StoresBodyAndSize()
    {
        //Given
        var context = _fixture.CreateContext();
        var originalBody = context.Response.Body;
        var options = _fixture.CreateOptions(capture: true, maxResponseBodySizeInMb: 1);
        var middleware = new CaptureResponseBodyMiddleware(async ctx =>
        {
            var payload = Encoding.UTF8.GetBytes("ok");
            await ctx.Response.Body.WriteAsync(payload);
        }, options);

        //When
        await middleware.InvokeAsync(context);

        //Then
        context.Items[NedMonitorConstants.CONTEXT_REPONSE_BODY_KEY].Should().Be("ok");
        context.Items[NedMonitorConstants.CONTEXT_REPONSE_BODY_SIZE_KEY].Should().BeOfType<long>();

        originalBody.Position = 0;
        using var reader = new StreamReader(originalBody, leaveOpen: true);
        (await reader.ReadToEndAsync()).Should().Be("ok");
    }

    [Fact(DisplayName =
        "Given capture enabled and body exceeds limit, " +
        "When invoking CaptureResponseBodyMiddleware, " +
        "Then it stores the truncation message")]
    [Trait("Middleware", nameof(CaptureResponseBodyMiddleware))]
    public async Task InvokeAsync_BodyTooLarge_StoresMessage()
    {
        //Given
        var context = _fixture.CreateContext();
        var options = _fixture.CreateOptions(capture: true, maxResponseBodySizeInMb: 0);
        var middleware = new CaptureResponseBodyMiddleware(async ctx =>
        {
            var payload = Encoding.UTF8.GetBytes("abc");
            await ctx.Response.Body.WriteAsync(payload);
        }, options);

        //When
        await middleware.InvokeAsync(context);

        //Then
        context.Items[NedMonitorConstants.CONTEXT_REPONSE_BODY_KEY]
            .Should()
            .BeOfType<string>()
            .Which.Should().Contain("exceeds limit of 0MB");
    }

    [Fact(DisplayName =
        "Given capture disabled, " +
        "When invoking CaptureResponseBodyMiddleware, " +
        "Then it stores only the response size")]
    [Trait("Middleware", nameof(CaptureResponseBodyMiddleware))]
    public async Task InvokeAsync_CaptureDisabled_StoresOnlySize()
    {
        //Given
        var context = _fixture.CreateContext();
        var options = _fixture.CreateOptions(capture: false, maxResponseBodySizeInMb: 1);
        var middleware = new CaptureResponseBodyMiddleware(async ctx =>
        {
            var payload = Encoding.UTF8.GetBytes("ok");
            await ctx.Response.Body.WriteAsync(payload);
        }, options);

        //When
        await middleware.InvokeAsync(context);

        //Then
        context.Items.ContainsKey(NedMonitorConstants.CONTEXT_REPONSE_BODY_KEY).Should().BeFalse();
        context.Items[NedMonitorConstants.CONTEXT_REPONSE_BODY_SIZE_KEY].Should().BeOfType<long>();
    }

    [Fact(DisplayName =
        "Given capture enabled and empty response body, " +
        "When invoking CaptureResponseBodyMiddleware, " +
        "Then it stores empty body and size zero")]
    [Trait("Middleware", nameof(CaptureResponseBodyMiddleware))]
    public async Task InvokeAsync_CaptureEnabled_EmptyBody_StoresEmptyString()
    {
        //Given
        var context = _fixture.CreateContext();
        var options = _fixture.CreateOptions(capture: true, maxResponseBodySizeInMb: 1);
        var middleware = new CaptureResponseBodyMiddleware(_ => Task.CompletedTask, options);

        //When
        await middleware.InvokeAsync(context);

        //Then
        context.Items[NedMonitorConstants.CONTEXT_REPONSE_BODY_KEY].Should().Be(string.Empty);
        context.Items[NedMonitorConstants.CONTEXT_REPONSE_BODY_SIZE_KEY].Should().Be(0L);
    }
}
