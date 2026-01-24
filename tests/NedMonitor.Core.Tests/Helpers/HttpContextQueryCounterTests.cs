using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core;
using NedMonitor.Core.Helpers;
using NedMonitor.Core.Tests.Helpers.Fixtures;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Helpers;

public class HttpContextQueryCounterTests(ITestOutputHelper output, HttpContextQueryCounterTestsFixture fixture) : Test(output), IClassFixture<HttpContextQueryCounterTestsFixture>
{
    private readonly HttpContextQueryCounterTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given empty HttpContext, " +
        "When incrementing twice, " +
        "Then count is 2")]
    [Trait("Helpers", nameof(HttpContextQueryCounter))]
    public async Task Increment_Twice_IncrementsCount()
    {
        //Given
        var counter = _fixture.CreateCounter();

        //When
        counter.Increment();
        counter.Increment();

        //Then
        var count = counter.GetCount(_fixture.Context);
        count.Should().Be(2);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given existing count, " +
        "When reset is called, " +
        "Then count becomes zero")]
    [Trait("Helpers", nameof(HttpContextQueryCounter))]
    public async Task Reset_SetsCountToZero()
    {
        //Given
        var counter = _fixture.CreateCounter();
        counter.Increment();

        //When
        counter.Reset(_fixture.Context);

        //Then
        counter.GetCount(_fixture.Context).Should().Be(0);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given null HttpContext, " +
        "When incrementing, " +
        "Then it does not throw and count remains zero")]
    [Trait("Helpers", nameof(HttpContextQueryCounter))]
    public async Task Increment_WithNullContext_DoesNothing()
    {
        //Given
        var counter = _fixture.CreateCounterWithNullContext();
        _fixture.Context.Items.Clear();

        //When
        counter.Increment();

        //Then
        _fixture.Context.Items.Should().NotContainKey(NedMonitorConstants.CONTEXT_QUERY_COUNT_KEY);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given non-integer count in context, " +
        "When getting count, " +
        "Then it returns zero")]
    [Trait("Helpers", nameof(HttpContextQueryCounter))]
    public async Task GetCount_WithNonIntegerValue_ReturnsZero()
    {
        //Given
        _fixture.Context.Items[NedMonitorConstants.CONTEXT_QUERY_COUNT_KEY] = "one";
        var counter = _fixture.CreateCounter();

        //When
        var count = counter.GetCount(_fixture.Context);

        //Then
        count.Should().Be(0);
        await Task.CompletedTask;
    }
}
