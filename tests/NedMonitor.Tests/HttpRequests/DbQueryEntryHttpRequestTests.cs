using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpRequests;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class DbQueryEntryHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given db query entry info, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(DbQueryEntryHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var entry = new DbQueryEntryHttpRequest
        {
            Provider = "Provider",
            Sql = "select 1",
            Parameters = "{}",
            ExecutedAt = DateTime.UtcNow,
            DurationMs = 10,
            Success = true,
            ExceptionMessage = "error",
            DbContext = new Dictionary<string, string>
            {
                ["db"] = "test"
            },
            ORM = "EF"
        };

        //When
        var json = JsonSerializer.Serialize(entry);

        //Then
        json.Should().Contain("\"provider\"");
        json.Should().Contain("\"sql\"");
        json.Should().Contain("\"parameters\"");
        json.Should().Contain("\"executedAt\"");
        json.Should().Contain("\"durationMs\"");
        json.Should().Contain("\"dbContext\"");
        json.Should().Contain("\"orm\"");
        await Task.CompletedTask;
    }
}
