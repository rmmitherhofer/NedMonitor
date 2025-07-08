using Microsoft.AspNetCore.Http;
using NedMonitor.Queues;
using System.Diagnostics;
using NedMonitor.Models;

namespace NedMonitor.Middleware;

/// <summary>
/// Middleware that measures request execution time and triggers NedMonitor notification
/// after the request is completed, regardless of success or exception.
/// </summary>
public class NedMonitorMiddleware
{
    /// <summary>
    /// Middleware name identifier.
    /// </summary>
    public const string Name = nameof(NedMonitorMiddleware);

    private readonly RequestDelegate _next;
    private readonly INedMonitorQueue _queue;
    private Stopwatch _diagnostic;

    /// <summary>
    /// Initializes a new instance of the <see cref="NedMonitorMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public NedMonitorMiddleware(RequestDelegate next, INedMonitorQueue queue)
    {
        ArgumentNullException.ThrowIfNull(queue, nameof(INedMonitorQueue));

        _next = next;
        _queue = queue;
    }

    /// <summary>
    /// Intercepts the HTTP request, measures its execution time, and sends data to NedMonitor.
    /// In case of an exception, the exception is stored in the HTTP context for later processing.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        _diagnostic = new();
        _diagnostic.Start();

        await _next(context);

        _diagnostic.Stop();

        _queue.Enqueue(await new Snapshot().CaptureAsync(context, _diagnostic.ElapsedMilliseconds));
    }
}
