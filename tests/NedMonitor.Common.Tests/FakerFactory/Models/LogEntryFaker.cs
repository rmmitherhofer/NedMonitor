using Bogus;
using Microsoft.Extensions.Logging;
using NedMonitor.Core.Models;

namespace NedMonitor.Common.Tests.FakerFactory.Models;

internal static class LogEntryFaker
{
    public static IEnumerable<LogEntry> CreateMany(
        int count,
        string? category = null,
        LogLevel? level = null,
        string? message = null)
        => Create(count, category, level, message);

    public static LogEntry? Create(
        string? category = null,
        LogLevel? level = null,
        string? message = null)
        => Create(1, category, level, message).FirstOrDefault();

    private static IEnumerable<LogEntry> Create(
        int count,
        string? category = null,
        LogLevel? level = null,
        string? message = null)
    {
        if (count == 0) return [];

        return new Faker<LogEntry>("pt_BR")
            .CustomInstantiator(faker => new LogEntry(
                category ?? faker.Commerce.Categories(1)[0],
                level ?? faker.PickRandom<LogLevel>(),
                message ?? faker.Lorem.Sentence()))
            .Generate(count);
    }
}
