using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Common.Tests.FakerFactory.Models;
using NedMonitor.Core.Models;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Models;

public class DbQueryEntryTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given new query entry, " +
        "When created, " +
        "Then Success defaults to true")]
    [Trait("Models", nameof(DbQueryEntry))]
    public async Task Constructor_DefaultsSuccessTrue()
    {
        //Given
        var entry = new NedMonitor.Core.Models.DbQueryEntry
        {
            Provider = "Provider",
            ORM = "Orm"
        };

        //When
        var success = entry.Success;

        //Then
        success.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given query entry data, " +
        "When setting properties, " +
        "Then it stores values")]
    [Trait("Models", nameof(DbQueryEntry))]
    public async Task Properties_SetAndGet()
    {
        //Given
        var entry = DbQueryEntryFaker.Create(provider: "Provider", orm: "Orm");

        //When
        var provider = entry.Provider;

        //Then
        provider.Should().Be("Provider");
        entry.ORM.Should().Be("Orm");
        entry.Sql.Should().Be("select 1");
        entry.Parameters.Should().Be("{}");
        entry.ExceptionMessage.Should().Be("none");
        entry.DbContext.Should().ContainKey("Database");
        await Task.CompletedTask;
    }
}
