using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Middleware;
using NedMonitor.Tests.Middleware.Fixtures;
using System.IO;
using Xunit.Abstractions;

namespace NedMonitor.Tests.Middleware;

public class BodyBufferingMiddlewareTests(
    ITestOutputHelper output,
    BodyBufferingMiddlewareTestsFixture fixture)
    : Test(output), IClassFixture<BodyBufferingMiddlewareTestsFixture>
{
    private readonly BodyBufferingMiddlewareTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given a request with non-seekable body, " +
        "When invoking BodyBufferingMiddleware, " +
        "Then it enables buffering and calls next")]
    [Trait("Middleware", nameof(BodyBufferingMiddleware))]
    public async Task InvokeAsync_EnablesBuffering_AndCallsNext()
    {
        //Given
        var context = _fixture.CreateContextWithNonSeekableBody("payload");
        var called = false;
        var middleware = new BodyBufferingMiddleware(ctx =>
        {
            called = true;
            return Task.CompletedTask;
        });

        //When
        await middleware.InvokeAsync(context);

        //Then
        called.Should().BeTrue();
        context.Request.Body.CanSeek.Should().BeTrue();
    }

    [Fact(DisplayName =
        "Given a buffered request body, " +
        "When invoking BodyBufferingMiddleware, " +
        "Then it can be read more than once")]
    [Trait("Middleware", nameof(BodyBufferingMiddleware))]
    public async Task InvokeAsync_AllowsMultipleReads()
    {
        //Given
        var context = _fixture.CreateContextWithNonSeekableBody("payload");
        string firstRead = string.Empty;
        string secondRead = string.Empty;

        var middleware = new BodyBufferingMiddleware(async ctx =>
        {
            using (var reader = new StreamReader(ctx.Request.Body, leaveOpen: true))
            {
                firstRead = await reader.ReadToEndAsync();
                ctx.Request.Body.Position = 0;
                secondRead = await reader.ReadToEndAsync();
            }
        });

        //When
        await middleware.InvokeAsync(context);

        //Then
        firstRead.Should().Be("payload");
        secondRead.Should().Be("payload");
    }

    [Fact(DisplayName =
        "Given next throws, " +
        "When invoking BodyBufferingMiddleware, " +
        "Then it rethrows the exception")]
    [Trait("Middleware", nameof(BodyBufferingMiddleware))]
    public async Task InvokeAsync_NextThrows_Rethrows()
    {
        //Given
        var context = _fixture.CreateContextWithNonSeekableBody("payload");
        var middleware = new BodyBufferingMiddleware(_ => throw new InvalidOperationException("boom"));

        //When
        var act = () => middleware.InvokeAsync(context);

        //Then
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
