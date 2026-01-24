using Bogus;
using NedMonitor.Core.Models;

namespace NedMonitor.Common.Tests.FakerFactory.Models;

public static class HttpRequestLogContextFaker
{
    public static IEnumerable<HttpRequestLogContext> CreateMany(
        int count,
        string? method = null,
        string? fullUrl = null,
        string? urlTemplate = null,
        int? statusCode = null,
        string? requestBody = null,
        string? responseBody = null,
        Dictionary<string, List<string>>? requestHeaders = null,
        Dictionary<string, List<string>>? responseHeaders = null,
        string? exceptionType = null,
        string? exceptionMessage = null,
        string? stackTrace = null,
        string? innerException = null)
        => Create(
            count,
            method,
            fullUrl,
            urlTemplate,
            statusCode,
            requestBody,
            responseBody,
            requestHeaders,
            responseHeaders,
            exceptionType,
            exceptionMessage,
            stackTrace,
            innerException);

    public static HttpRequestLogContext? Create(
        string? method = null,
        string? fullUrl = null,
        string? urlTemplate = null,
        int? statusCode = null,
        string? requestBody = null,
        string? responseBody = null,
        Dictionary<string, List<string>>? requestHeaders = null,
        Dictionary<string, List<string>>? responseHeaders = null,
        string? exceptionType = null,
        string? exceptionMessage = null,
        string? stackTrace = null,
        string? innerException = null)
        => Create(
            1,
            method,
            fullUrl,
            urlTemplate,
            statusCode,
            requestBody,
            responseBody,
            requestHeaders,
            responseHeaders,
            exceptionType,
            exceptionMessage,
            stackTrace,
            innerException).FirstOrDefault();

    private static IEnumerable<HttpRequestLogContext> Create(
        int count,
        string? method = null,
        string? fullUrl = null,
        string? urlTemplate = null,
        int? statusCode = null,
        string? requestBody = null,
        string? responseBody = null,
        Dictionary<string, List<string>>? requestHeaders = null,
        Dictionary<string, List<string>>? responseHeaders = null,
        string? exceptionType = null,
        string? exceptionMessage = null,
        string? stackTrace = null,
        string? innerException = null)
    {
        if (count == 0) return [];

        return new Faker<HttpRequestLogContext>("pt_BR")
            .CustomInstantiator(faker => new()
            {
                StartTime = DateTime.UtcNow.AddSeconds(-1),
                EndTime = DateTime.UtcNow,
                Method = method ?? faker.PickRandom("GET", "POST"),
                FullUrl = fullUrl ?? faker.Internet.Url(),
                UrlTemplate = urlTemplate ?? "/api",
                StatusCode = statusCode ?? 200,
                RequestBody = requestBody ?? "{}",
                ResponseBody = responseBody ?? "{}",
                RequestHeaders = requestHeaders ?? new Dictionary<string, List<string>> { ["h"] = ["v"] },
                ResponseHeaders = responseHeaders ?? new Dictionary<string, List<string>> { ["rh"] = ["rv"] },
                ExceptionType = exceptionType ?? "System.Exception",
                ExceptionMessage = exceptionMessage ?? "boom",
                StackTrace = stackTrace ?? "stack",
                InnerException = innerException ?? "inner"
            })
            .Generate(count);
    }
}
