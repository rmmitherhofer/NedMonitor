using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Middlewares;
using NedMonitor.Core.Tests.Middlewares.Fixtures;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Middlewares;

public class QueryCounterResetMiddlewareTests(ITestOutputHelper output, QueryCounterResetMiddlewareTestsFixture fixture)
    : Test(output), IClassFixture<QueryCounterResetMiddlewareTestsFixture>
{
    private readonly QueryCounterResetMiddlewareTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given request pipeline, " +
        "When invoking middleware, " +
        "Then it resets counter before next")]
    [Trait("Middlewares", nameof(QueryCounterResetMiddleware))]
    public async Task InvokeAsync_ResetsCounter_ThenCallsNext()
    {
        //Given
        var counter = _fixture.CreateCounter();
        var next = _fixture.CreateNext();
        var middleware = new QueryCounterResetMiddleware(next, counter);

        //When
        await middleware.InvokeAsync(_fixture.Context);

        //Then
        _fixture.Calls.Should().Equal("reset", "next");
    }
}
