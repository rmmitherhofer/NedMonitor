using Microsoft.AspNetCore.Http;
using NedMonitor.Core.Models;
using NedMonitor.Queues;
using System.Diagnostics;

namespace NedMonitor.Middleware;

/// <summary>
/// Middleware that measures request execution time and triggers NedMonitor notification
/// after the request is completed, regardless of success or exception.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NedMonitorMiddleware"/> class.
/// </remarks>
/// <param name="next">The next middleware in the pipeline.</param>
internal class NedMonitorMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Middleware name identifier.
    /// </summary>
    public const string Name = nameof(NedMonitorMiddleware);

    private readonly RequestDelegate _next = next;
    private Stopwatch _diagnostic;

    /// <summary>
    /// Intercepts the HTTP request, measures its execution time, and sends data to NedMonitor.
    /// In case of an exception, the exception is stored in the HTTP context for later processing.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context, INedMonitorQueue queue)
    {
        _diagnostic = new();
        DateTime startAt = DateTime.Now;
        try
        {
            _diagnostic.Start();

            await _next(context);
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            _diagnostic.Stop();
            queue.Enqueue(await new Snapshot().CaptureAsync(context, _diagnostic.Elapsed.TotalMilliseconds, startAt, DateTime.Now));
        }
    }
}
