using Bogus;
using NedMonitor.Core.Models;

namespace NedMonitor.Common.Tests.FakerFactory.Models;

public static class DbQueryEntryFaker
{
    public static IEnumerable<DbQueryEntry> CreateMany(
        int count,
        string? provider = null,
        string? orm = null,
        string? sql = null,
        string? parameters = null,
        string? exceptionMessage = null,
        IDictionary<string, string>? dbContext = null)
        => Create(count, provider, orm, sql, parameters, exceptionMessage, dbContext);

    public static DbQueryEntry? Create(
        string? provider = null,
        string? orm = null,
        string? sql = null,
        string? parameters = null,
        string? exceptionMessage = null,
        IDictionary<string, string>? dbContext = null)
        => Create(1, provider, orm, sql, parameters, exceptionMessage, dbContext).FirstOrDefault();

    private static IEnumerable<DbQueryEntry> Create(
        int count,
        string? provider = null,
        string? orm = null,
        string? sql = null,
        string? parameters = null,
        string? exceptionMessage = null,
        IDictionary<string, string>? dbContext = null)
    {
        if (count == 0) return [];

        return new Faker<DbQueryEntry>("pt_BR")
            .CustomInstantiator(faker => new DbQueryEntry
            {
                Provider = provider ?? faker.Company.CompanyName(),
                ORM = orm ?? faker.PickRandom("EF", "Dapper"),
                Sql = sql ?? "select 1",
                Parameters = parameters ?? "{}",
                ExecutedAt = DateTime.UtcNow,
                DurationMs = faker.Random.Double(1, 500),
                ExceptionMessage = exceptionMessage ?? "none",
                DbContext = dbContext ?? new Dictionary<string, string> { ["Database"] = "Db" }
            })
            .Generate(count);
    }
}
