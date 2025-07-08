using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NedMonitor.Configurations.Settings;

namespace NedMonitor.Middleware;

/// <summary>
/// Middleware that captures the HTTP response body for logging purposes,
/// storing it in the current context if its size is within the configured limit.
/// </summary>
public class CaptureResponseBodyMiddleware(RequestDelegate next, IOptions<NedMonitorSettings> options)
{
    /// <summary>
    /// Middleware name identifier.
    /// </summary>
    public const string Name = nameof(CaptureResponseBodyMiddleware);

    private readonly NedMonitorSettings _settings = options.Value;
    private readonly RequestDelegate _next = next;

    /// <summary>
    /// Captures the response body and stores it in <c>HttpContext.Items["CapturedResponseBody"]</c>
    /// if the content length is within the allowed size.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var originalBody = context.Response.Body;

        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await _next(context);

        string capturedBody;

        long maxSize = _settings.MaxResponseBodySizeInMb * 1024L * 1024L;

        if (_settings.CaptureResponseBody)
        {
            if (memoryStream.Length <= maxSize)
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                capturedBody = await new StreamReader(memoryStream).ReadToEndAsync();
            }
            else
            {
                capturedBody = $"[Body not captured. Size exceeds limit of {_settings.MaxResponseBodySizeInMb}MB]";
            }
            context.Items["CapturedResponseBody"] = capturedBody;
        }
        context.Items["CapturedResponseBodySize"] = memoryStream.Length;

        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBody);
        context.Response.Body = originalBody;
    }
}
