using Microsoft.AspNetCore.Http;

namespace NedMonitor.DataInterceptors;

/// <summary>
/// Middleware que reseta o contador de queries no início de cada requisição HTTP.
/// </summary>
public class QueryCounterResetMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IQueryCounter _queryCounter;

    public QueryCounterResetMiddleware(RequestDelegate next, IQueryCounter queryCounter)
    {
        _next = next;
        _queryCounter = queryCounter;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _queryCounter.Reset();
        await _next(context);
    }
}