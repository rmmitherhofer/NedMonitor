using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Dapper.Cache;
using Xunit.Abstractions;

namespace NedMonitor.Dapper.Tests.Cache;

public class OrmVersionCacheTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given Dapper version cache, " +
        "When accessed, " +
        "Then it returns a non-empty value")]
    [Trait("Cache", nameof(OrmVersionCache))]
    public async Task DapperVersionCached_ReturnsValue()
    {
        //Given
        var version = OrmVersionCache.DapperVersionCached;

        //When
        var result = version;

        //Then
        result.Should().NotBeNullOrWhiteSpace();
        await Task.CompletedTask;
    }
}
