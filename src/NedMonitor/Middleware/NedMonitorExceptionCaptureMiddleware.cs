using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NedMonitor.Applications;
using NedMonitor.Core.Settings;

namespace NedMonitor.Middleware;

public class NedMonitorExceptionCaptureMiddleware
{
    private readonly RequestDelegate _next;
    private readonly NedMonitorSettings _settings;

    public NedMonitorExceptionCaptureMiddleware(
        RequestDelegate next,
        INedMonitorApplication nedMonitor,
        IOptions<NedMonitorSettings> settings)
    {
        _next = next;
        _settings = settings.Value;
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
                ex.GetType().FullName?.Equals(expected, StringComparison.OrdinalIgnoreCase) == true) ?? false;

            if (!isExpected)
                context.Items[NedMonitorConstants.CONTEXT_EXCEPTION_KEY] = ex;

            throw;
        }
    }
}