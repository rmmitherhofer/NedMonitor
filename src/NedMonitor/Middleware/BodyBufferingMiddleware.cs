using Microsoft.AspNetCore.Http;

namespace NedMonitor.Middleware;

/// <summary>
/// Middleware that enables buffering on the HTTP request body,
/// allowing it to be read multiple times during the request pipeline.
/// </summary>
public class BodyBufferingMiddleware(RequestDelegate next)
{
    /// <summary>
    /// The name identifier of the middleware.
    /// </summary>
    public const string Name = nameof(BodyBufferingMiddleware);

    private readonly RequestDelegate _next = next;

    /// <summary>
    /// Enables request body buffering and invokes the next middleware in the pipeline.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        await _next(context);
    }
}
