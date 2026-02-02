using Microsoft.AspNetCore.Http;
using NedMonitor.Core.Interfaces;

namespace NedMonitor.Core.Middlewares;

internal class QueryCounterResetMiddleware(RequestDelegate next, IQueryCounter counter)
{
    private readonly RequestDelegate _next = next;
    private readonly IQueryCounter _counter = counter;

    public async Task InvokeAsync(HttpContext context)
    {
        _counter.Reset(context);
        await _next(context);
    }
}