using Bogus;
using Microsoft.AspNetCore.Http;

namespace NedMonitor.Common.Tests.FakerFactory;

public static class HttpContextFaker
{
    public static IEnumerable<DefaultHttpContext> CreateMany(
        int count,
        string? userAgent = null,
        string? referer = null,
        string? forwardedFor = null,
        string? correlationId = null,
        string? clientId = null)
        => Create(count, userAgent, referer, forwardedFor, correlationId, clientId);

    public static DefaultHttpContext? Create(
        string? userAgent = null,
        string? referer = null,
        string? forwardedFor = null,
        string? correlationId = null,
        string? clientId = null)
        => Create(1, userAgent, referer, forwardedFor, correlationId, clientId).FirstOrDefault();

    private static IEnumerable<DefaultHttpContext> Create(
        int count,
        string? userAgent = null,
        string? referer = null,
        string? forwardedFor = null,
        string? correlationId = null,
        string? clientId = null)
    {
        if (count == 0) return [];

        return new Faker<DefaultHttpContext>("pt_BR")
            .CustomInstantiator(f =>
            {
                DefaultHttpContext httpContext = new();
                httpContext.Request.Headers["User-Agent"] = userAgent ?? f.Internet.UserAgent();
                httpContext.Request.Headers["Referer"] = referer ?? f.Internet.Url();
                httpContext.Request.Headers["X-Forwarded-For"] = forwardedFor ?? f.Internet.Ip();
                httpContext.Request.Headers["X-Correlation-ID"] = correlationId ?? Guid.NewGuid().ToString();
                httpContext.Request.Headers["X-Client-ID"] = clientId ?? f.Company.CompanyName();

                return httpContext;
            }).Generate(count);
    }
}
