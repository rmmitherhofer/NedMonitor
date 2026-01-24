using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Helpers;
using NedMonitor.Core.Models;
using NedMonitor.Core.Tests.Helpers.Fixtures;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Helpers;

public class QueryLogHelperTests(ITestOutputHelper output, QueryLogHelperTestsFixture fixture) : Test(output), IClassFixture<QueryLogHelperTestsFixture>
{
    private readonly QueryLogHelperTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given empty context, " +
        "When adding query log, " +
        "Then it creates list and stores entry")]
    [Trait("Helpers", nameof(QueryLogHelper))]
    public async Task AddQueryLog_CreatesList_WhenMissing()
    {
        //Given
        var entry = _fixture.CreateEntry();

        //When
        QueryLogHelper.AddQueryLog(_fixture.Context, entry);

        //Then
        var list = _fixture.Context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY]
            .Should().BeOfType<List<DbQueryEntry>>().Which;
        list.Should().HaveCount(1);
        list[0].Should().BeSameAs(entry);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given existing list, " +
        "When adding query log, " +
        "Then it appends to existing list")]
    [Trait("Helpers", nameof(QueryLogHelper))]
    public async Task AddQueryLog_Appends_WhenListExists()
    {
        //Given
        var list = new List<DbQueryEntry>();
        _fixture.Context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY] = list;
        var entry = _fixture.CreateEntry();

        //When
        QueryLogHelper.AddQueryLog(_fixture.Context, entry);

        //Then
        list.Should().HaveCount(1);
        list[0].Should().BeSameAs(entry);
        await Task.CompletedTask;
    }
}
