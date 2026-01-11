using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NedMonitor.Core;
using NedMonitor.Core.Models;
using NedMonitor.Core.Settings;
using Zypher.Http.Extensions;

namespace NedMonitor.Http.Handlers;

/// <summary>
/// A delegating HTTP handler that captures and logs metadata for outgoing HTTP client requests and responses,
/// including request/response headers, bodies, status codes, and exceptions (if any).
/// This logging is only activated if both <c>EnableNedMonitor</c> and <c>MonitorHttpRequests</c>
/// are enabled in <see cref="ExecutionModeSettings"/>.
/// The collected data is stored in <see cref="HttpContext.Items"/> under a predefined key,
/// allowing later access by other middleware or services.
public class NedMonitorHttpLoggingHandler : DelegatingHandler
{
    private readonly NedMonitorSettings _settings;
    private readonly IHttpContextAccessor _accessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="NedMonitorHttpLoggingHandler"/> class.
    /// </summary>
    /// <param name="accessor">Provides access to the current <see cref="HttpContext"/>.</param>
    /// <param name="options">Options wrapper containing <see cref="NedMonitorSettings"/>.</param>

    public NedMonitorHttpLoggingHandler(IHttpContextAccessor accessor, IOptions<NedMonitorSettings> options)
    {
        _settings = options.Value;
        _accessor = accessor;
    }

    /// <summary>
    /// Sends the HTTP request and captures detailed information about the request and response,
    /// including headers, body content, timestamps, status codes, and exceptions.
    /// All captured information is stored in the <see cref="HttpRequestLogContext"/> and appended
    /// to the current <see cref="HttpContext"/> for later use (e.g., diagnostics or monitoring).
    /// </summary>
    /// <param name="request">The outgoing <see cref="HttpRequestMessage"/>.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="HttpResponseMessage"/> returned by the inner handler.</returns>
    /// <exception cref="Exception">Re-throws any exceptions encountered during the request.</exception>

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? urlTemplate = request.GetHeaderRequestTemplate();

        request.Headers.Remove(HttpRequestMessageExtensions.X_REQUEST_TEMPLATE);

        if (!_settings.ExecutionMode.EnableNedMonitor || !_settings.ExecutionMode.EnableMonitorHttpRequests)
            return await base.SendAsync(request, cancellationToken);

        var context = new HttpRequestLogContext
        {
            StartTime = DateTime.Now,
            Method = request.Method.Method,
            FullUrl = request.RequestUri?.ToString() ?? string.Empty,
            UrlTemplate = urlTemplate,
            RequestHeaders = request.Headers
            .Concat(request.Content?.Headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>())
            .ToDictionary(h => h.Key, h => h.Value.ToList())
        };

        if (request.Content is not null)
            context.RequestBody = await request.Content.ReadAsStringAsync();

        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            context.StatusCode = (int)response.StatusCode;
            context.EndTime = DateTime.Now;

            context.ResponseHeaders = response.Headers
                .Concat(response.Content?.Headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>())
                .ToDictionary(h => h.Key, h => h.Value.ToList());

            if (response.Content is not null)
            {
                var rawBody = await response.Content.ReadAsStringAsync();
                long maxSize = _settings.HttpLogging.MaxResponseBodySizeInMb * 1024L * 1024L;

                if (rawBody.Length <= maxSize)
                {
                    context.ResponseBody = rawBody;
                }
                else
                {
                    context.ResponseBody = $"[Body not captured. Size exceeds limit of {_settings.HttpLogging.MaxResponseBodySizeInMb}MB]";
                }
            }

            return response;
        }
        catch (Exception ex)
        {
            context.EndTime = DateTime.Now;
            context.ExceptionType = ex.GetType().FullName;
            context.ExceptionMessage = ex.Message;
            context.StackTrace = ex.StackTrace;
            context.InnerException = ex.InnerException?.Message;
            throw;
        }
        finally
        {
            var httpContext = _accessor.HttpContext;

            if (httpContext != null)
            {
                if (!httpContext.Items.TryGetValue(NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY, out var listObj) || listObj is not List<HttpRequestLogContext> list)
                {
                    list = [];
                    httpContext.Items[NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY] = list;
                }

                list.Add(context);
            }
        }
    }
}
