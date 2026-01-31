using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NedMonitor.Core;
using NedMonitor.Core.Models;
using NedMonitor.Core.Settings;

namespace NedMonitor.Middleware;

internal class NedMonitorExceptionCaptureMiddleware(RequestDelegate next, IOptions<NedMonitorSettings> settings)
{
    private readonly RequestDelegate _next = next;
    private readonly NedMonitorSettings _settings = settings.Value;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            bool isExpected = _settings.Exceptions.Expected?.Any(expected =>
                ex.GetType().FullName?.Equals(expected, StringComparison.OrdinalIgnoreCase) == true) ?? false;

            if (!isExpected)
            {
                context.Items[NedMonitorConstants.CONTEXT_EXCEPTION_KEY] = new ExceptionInfo()
                {
                    Type = ex.GetType().FullName!,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    Source = ex.Source,
                    Timestamp = DateTime.Now
                };
            }
            throw;
        }
    }
}