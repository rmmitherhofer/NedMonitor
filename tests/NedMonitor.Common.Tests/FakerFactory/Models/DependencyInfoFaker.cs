using Bogus;
using NedMonitor.Core.Models;

namespace NedMonitor.Common.Tests.FakerFactory.Models;

public static class DependencyInfoFaker
{
    public static IEnumerable<DependencyInfo> CreateMany(
        int count,
        string? type = null,
        string? target = null,
        bool? success = null,
        int? durationMs = null) 
        => Create(count, type, target, success, durationMs);

    public static DependencyInfo? Create(
        string? type = null,
        string? target = null,
        bool? success = null,
        int? durationMs = null)
        => Create(1, type, target, success, durationMs).FirstOrDefault();

    private static IEnumerable<DependencyInfo> Create(
        int count,
        string? type = null,
        string? target = null,
        bool? success = null,
        int? durationMs = null)
    {
        if (count == 0) return [];

        return new Faker<DependencyInfo>("pt_BR")
            .CustomInstantiator(faker => new()
            {
                Type = type ?? faker.PickRandom("HTTP", "SQL"),
                Target = target ?? faker.Internet.Url(),
                Success = success ?? true,
                DurationMs = durationMs ?? faker.Random.Int(1, 500)
            })
            .Generate(count);
    }
}
