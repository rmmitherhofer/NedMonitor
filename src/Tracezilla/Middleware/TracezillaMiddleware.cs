using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Tracezilla.Models;
using Tracezilla.Queues;

namespace Tracezilla.Middleware;

/// <summary>
/// Middleware that measures request execution time and triggers Tracezilla notification
/// after the request is completed, regardless of success or exception.
/// </summary>
public class TracezillaMiddleware
{
    /// <summary>
    /// Middleware name identifier.
    /// </summary>
    public const string Name = nameof(TracezillaMiddleware);

    private readonly RequestDelegate _next;
    private readonly ITracezillaQueue _queue;
    private Stopwatch _diagnostic;

    /// <summary>
    /// Initializes a new instance of the <see cref="TracezillaMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public TracezillaMiddleware(RequestDelegate next, ITracezillaQueue queue)
    {
        ArgumentNullException.ThrowIfNull(queue, nameof(ITracezillaQueue));

        _next = next;
        _queue = queue;
    }

    /// <summary>
    /// Intercepts the HTTP request, measures its execution time, and sends data to Tracezilla.
    /// In case of an exception, the exception is stored in the HTTP context for later processing.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        _diagnostic = new();
        _diagnostic.Start();

        await _next(context);

        _diagnostic.Stop();

        if (!context.Request.Path.Value.Contains("swagger"))
            _queue.Enqueue(await new Snapshot().CaptureAsync(context, _diagnostic.ElapsedMilliseconds));
    }
}
