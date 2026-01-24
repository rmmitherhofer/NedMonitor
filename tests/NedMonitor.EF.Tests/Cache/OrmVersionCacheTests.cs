using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.EF.Cache;
using Xunit.Abstractions;

namespace NedMonitor.EF.Tests.Cache;

public class OrmVersionCacheTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given EF version cache, " +
        "When accessed, " +
        "Then it returns a non-empty value")]
    [Trait("Cache", nameof(OrmVersionCache))]
    public async Task EfVersion_ReturnsValue()
    {
        //Given
        var version = OrmVersionCache.EfVersion;

        //When
        var result = version;

        //Then
        result.Should().NotBeNullOrWhiteSpace();
        await Task.CompletedTask;
    }
}
