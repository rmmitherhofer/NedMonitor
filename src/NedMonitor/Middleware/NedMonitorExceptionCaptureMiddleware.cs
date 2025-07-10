using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NedMonitor.Applications;
using NedMonitor.Core;
using NedMonitor.Core.Settings;

namespace NedMonitor.Middleware;

public class NedMonitorExceptionCaptureMiddleware
{
    private readonly RequestDelegate _next;
    private readonly INedMonitorApplication _nedMonitor;
    private readonly NedMonitorSettings _settings;
    private readonly ILogger<NedMonitorExceptionCaptureMiddleware> _logger;

    public NedMonitorExceptionCaptureMiddleware(
        RequestDelegate next,
        INedMonitorApplication nedMonitor,
        IOptions<NedMonitorSettings> settings,
        ILogger<NedMonitorExceptionCaptureMiddleware> logger)
    {
        _next = next;
        _nedMonitor = nedMonitor;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            bool isExpected = _settings.ExpectedExceptions?.Any(expected =>
                ex.GetType().Name?.Equals(expected, StringComparison.OrdinalIgnoreCase) == true) ?? false;

            if (!isExpected)
                context.Items[NedMonitorConstants.CONTEXT_EXCEPTION_KEY] = ex;

            throw;
        }
    }
}