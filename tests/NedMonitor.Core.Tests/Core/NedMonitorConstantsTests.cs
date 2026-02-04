using FluentAssertions;
using NedMonitor.Common.Tests;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Core;

public class NedMonitorConstantsTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given default sensitive keys, " +
        "When accessed, " +
        "Then they include common entries")]
    [Trait("Core", nameof(NedMonitorConstants))]
    public async Task DefaultKeys_IncludeCommonEntries()
    {
        //Given
        var keys = NedMonitorConstants.DEFAULT_KEYS;

        //When
        var count = keys.Count;

        //Then
        count.Should().BeGreaterThan(0);
        keys.Should().Contain("password");
        keys.Should().Contain("token");
        keys.Should().Contain("senha");
        keys.Should().OnlyHaveUniqueItems();
        await Task.CompletedTask;
    }
}
