using Microsoft.AspNetCore.Http;
using NedMonitor.Core.Interfaces;

namespace NedMonitor.Core.Middlewares;

public class QueryCounterResetMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IQueryCounter _counter;

    public QueryCounterResetMiddleware(RequestDelegate next, IQueryCounter counter)
    {
        _next = next;
        _counter = counter;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _counter.Reset(context);
        await _next(context);
    }
}