using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Common.Tests.FakerFactory.Models;
using NedMonitor.Core.Models;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Models;

public class DependencyInfoTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given dependency info, " +
        "When setting properties, " +
        "Then it stores values")]
    [Trait("Models", nameof(DependencyInfo))]
    public async Task Properties_SetAndGet()
    {
        //Given
        var info = DependencyInfoFaker.Create(
            type: "HTTP",
            target: "https://example.local",
            success: true,
            durationMs: 120);

        //When
        var type = info.Type;

        //Then
        type.Should().Be("HTTP");
        info.Target.Should().Be("https://example.local");
        info.Success.Should().BeTrue();
        info.DurationMs.Should().Be(120);
        await Task.CompletedTask;
    }
}
