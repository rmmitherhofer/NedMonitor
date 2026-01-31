using Bogus;
using NedMonitor.Core.Models;

namespace NedMonitor.Common.Tests.FakerFactory.Models;

internal static class ExceptionInfoFaker
{
    public static IEnumerable<ExceptionInfo> CreateMany(
        int count,
        string? type = null,
        string? message = null,
        string? stackTrace = null,
        string? innerException = null,
        DateTime? timestamp = null,
        string? source = null)
        => Create(count, type, message, stackTrace, innerException, timestamp, source);

    public static ExceptionInfo? Create(
        string? type = null,
        string? message = null,
        string? stackTrace = null,
        string? innerException = null,
        DateTime? timestamp = null,
        string? source = null)
        => Create(1, type, message, stackTrace, innerException, timestamp, source).FirstOrDefault();

    private static IEnumerable<ExceptionInfo> Create(
        int count,
        string? type = null,
        string? message = null,
        string? stackTrace = null,
        string? innerException = null,
        DateTime? timestamp = null,
        string? source = null)
    {
        if (count == 0) return [];

        return new Faker<ExceptionInfo>("pt_BR")
            .CustomInstantiator(faker => new()
            {
                Type = type ?? "System.Exception",
                Message = message ?? faker.Lorem.Sentence(),
                StackTrace = stackTrace ?? "stack",
                InnerException = innerException ?? "inner",
                Timestamp = timestamp ?? DateTime.UtcNow,
                Source = source ?? faker.System.FileName()
            })
            .Generate(count);
    }
}
