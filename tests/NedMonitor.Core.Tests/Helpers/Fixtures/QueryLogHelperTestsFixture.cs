using Microsoft.AspNetCore.Http;
using NedMonitor.Core.Models;

namespace NedMonitor.Core.Tests.Helpers.Fixtures;

public sealed class QueryLogHelperTestsFixture
{
    public DefaultHttpContext Context { get; } = new();

    public DbQueryEntry CreateEntry() => new()
    {
        Provider = "Test",
        ORM = "TestOrm",
        ExecutedAt = DateTime.UtcNow
    };
}
