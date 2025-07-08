using Common.Extensions;
using Common.Json;
using Common.Notifications.Messages;
using Common.User.Extensions;
using Microsoft.AspNetCore.Http;
using NedMonitor.Adapters;

namespace NedMonitor.Models;

public class Snapshot
{
    #region Request
    /// <summary>
    /// The unique identifier for the current request.
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// The URL scheme (http, https).
    /// </summary>
    public string Scheme { get; set; }

    /// <summary>
    /// The HTTP protocol version.
    /// </summary>
    public string Protocol { get; set; }

    /// <summary>
    /// Indicates if the request uses HTTPS.
    /// </summary>
    public bool IsHttps { get; set; }

    /// <summary>
    /// The query string parameters.
    /// </summary>
    public string QueryString { get; set; }

    /// <summary>
    /// The route values extracted from the request URL.
    /// </summary>
    public IDictionary<string, string> RouteValues { get; set; }

    /// <summary>
    /// The User-Agent header from the request.
    /// </summary>
    public string UserAgent { get; set; }

    /// <summary>
    /// The client identifier making the request.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// The HTTP headers included in the request.
    /// </summary>
    public IDictionary<string, List<string>> RequestHeaders { get; set; }

    /// <summary>
    /// The content type of the request body.
    /// </summary>
    public string RequestContentType { get; set; }

    /// <summary>
    /// The length of the content body, if available.
    /// </summary>
    public long? RequestContentLength { get; set; }

    /// <summary>
    /// The request body content.
    /// </summary>
    public object? RequestBody { get; set; }

    /// <summary>
    /// Indicates if the request was made via AJAX.
    /// </summary>
    public bool IsAjaxRequest { get; set; }
    /// <summary>
    /// Indicates whether the request has a form content type (e.g., multipart/form-data).
    /// </summary>
    public bool HasFormContentType { get; set; }
    /// <summary>
    /// The IP address of the client making the request.
    /// </summary>
    public string? IpAddress { get; set; }
    #endregion

    #region Environment
    /// <summary>
    /// The thread ID associated with the current request execution.
    /// </summary>
    public int ThreadId { get; set; }
    #endregion

    #region User
    /// <summary>
    /// The authenticated user ID (if available).
    /// </summary>
    public string? UserId { get; set; }
    /// <summary>
    /// The authenticated user's name (if available).
    /// </summary>
    public string? UserName { get; set; }
    /// <summary>
    /// The authenticated user's document (if available).
    /// </summary>
    public string? UserDocument { get; set; }
    /// <summary>
    /// The authenticated user's account (if available).
    /// </summary>
    public string? UserAccount { get; set; }
    /// <summary>
    /// The authenticated user's e-mail (if available).
    /// </summary>
    public string? UserEmail { get; set; }
    /// <summary>
    /// Indicates whether the user is authenticated.
    /// </summary>
    public bool IsAuthenticated { get; set; }
    /// <summary>
    /// Authentication mechanism used (e.g., JWT, Cookie).
    /// </summary>
    public string? AuthenticationType { get; set; }
    /// <summary>
    /// The tenant ID, useful in multi-tenant applications.
    /// </summary>
    public string? TenantId { get; set; }
    /// <summary>
    /// Roles assigned to the current user.
    /// </summary>
    public IEnumerable<string> Roles { get; set; }
    /// <summary>
    /// Claims attached to the user identity.
    /// </summary>
    public IDictionary<string, string> Claims { get; set; }
    #endregion

    #region Response

    /// <summary>
    /// The HTTP status code returned by the response.
    /// </summary>
    public int StatusCode { get; set; }
    /// <summary>
    /// The HTTP headers included in the response.
    /// </summary>
    public IDictionary<string, List<string>>? ResponseHeaders { get; set; }

    /// <summary>
    /// The body content of the response.
    /// </summary>
    public object? ResponseBody { get; set; }

    /// <summary>
    /// The size of the body content in bytes.
    /// </summary>
    public long ResponseBodySize { get; set; }
    #endregion

    #region Diagnostics
    /// <summary>
    /// Total memory usage (in MB) at the time of snapshot.
    /// </summary>
    public double MemoryUsageMb { get; set; }
    #endregion

    /// <summary>
    /// Correlation ID for distributed tracing.
    /// </summary>
    public string CorrelationId { get; set; }
    /// <summary>
    /// Trace ID for request correlation (used by ASP.NET Core diagnostics).
    /// </summary>
    public string? TraceId { get; set; }
    /// <summary>
    /// The raw path of the request.
    /// </summary>
    public string Path { get; set; }
    /// <summary>
    /// The HTTP method used (duplicate of HttpMethod).
    /// </summary>
    public string Method { get; set; }
    /// <summary>
    /// The full request URL (duplicate of RequestUrl).
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// The total duration of the request in milliseconds.
    /// </summary>
    public long ElapsedMilliseconds { get; set; }
    /// <summary>
    /// List of log entries captured during the request.
    /// </summary>
    public IEnumerable<LogEntry>? LogEntries { get; set; }
    /// <summary>
    /// Domain notifications collected during the request.
    /// </summary>
    public IEnumerable<Notification>? Notifications { get; set; }
    /// <summary>
    /// Exception thrown during the request (if any).
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// Captures the full snapshot of the request lifecycle including request, response, user, diagnostics, and logs.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="elapsedMs">Elapsed time for the request, in milliseconds.</param>
    /// <returns>The completed <see cref="Snapshot"/> instance.</returns>
    public async Task<Snapshot> CaptureAsync(HttpContext context, long elapsedMs)
    {
        var request = context.Request;
        var response = context.Response;
        var user = context.User;

        context.Items.TryGetValue("CapturedResponseBody", out var responseObj);
        context.Items.TryGetValue("Exception", out var exceptionObj);
        context.Items.TryGetValue("__Notifications__", out var notificationsObj);
        context.Items.TryGetValue("CapturedResponseBodySize", out var responseBodySizeObj);

        return new Snapshot
        {
            CorrelationId = request.GetCorrelationId(),
            TraceId = context.TraceIdentifier,
            Path = request.Path,
            ElapsedMilliseconds = elapsedMs,

            #region Request
            RequestId = request.GetRequestId(),
            Method = request.Method,
            Url = request.GetFullUrl(),
            Scheme = request.Scheme,
            Protocol = request.Protocol,
            IsHttps = request.IsHttps,
            QueryString = request.QueryString.ToString(),
            RouteValues = request.RouteValues.ToDictionary(k => k.Key, v => v.Value?.ToString()),
            UserAgent = request.GetUserAgent(),
            ClientId = request.GetClientId(),
            RequestHeaders = request.Headers.ToDictionary(k => k.Key, v => v.Value.ToList()),
            RequestContentType = request.GetContentType(),
            RequestContentLength = request.ContentLength ?? 0,
            RequestBody = GetRequestBodyAsync(context),
            IsAjaxRequest = request.IsAjaxRequest(),
            IpAddress = request.GetIpAddress(),
            #endregion

            #region Response
            StatusCode = response.StatusCode,
            ResponseHeaders = response.Headers.ToDictionary(k => k.Key, v => v.Value.ToList()),
            ResponseBody = responseObj,
            ResponseBodySize = responseBodySizeObj is long responseBodySize ? responseBodySize : 0,
            #endregion

            #region Diagnostic
            MemoryUsageMb = GC.GetTotalMemory(false) / (1024.0 * 1024),
            #endregion

            #region User
            UserId = user.GetId() ?? request.GetUserId(),
            UserName = user.GetName() ?? request.GetUserName(),
            UserAccount = user.GetAccount() ?? request.GetUserAccount(),
            UserDocument = user.GetDocument() ?? request.GetUserDocument(),
            UserEmail = user.GetEmail(),
            TenantId = user.GetTenantId(),
            IsAuthenticated = user.IsAuthenticated(),
            AuthenticationType = user.GetAuthenticationType(),
            Roles = user.GetRoles(),
            Claims = user.Claims.ToDictionary(c => c.Type, c => c.Value),
            #endregion

            LogEntries = LoggerAdapter.GetLogsForCurrentRequest(context),
            Notifications = notificationsObj is IEnumerable<Notification> notifications ? notifications : null,
            Exception = exceptionObj is Exception exception ? exception : null
        };
    }
    /// <summary>
    /// Extracts and parses the request body based on its content type.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>The deserialized request body or raw content.</returns>
    private async Task<object?> GetRequestBodyAsync(HttpContext context)
    {
        if (context.Request.HasFormContentType)
        {
            HasFormContentType = true;

            var form = await context.Request.ReadFormAsync();
            var dict = form.ToDictionary(x => x.Key, x => (object)x.Value.ToString());

            for (int i = 0; i < form.Files.Count; i++)
            {
                var file = form.Files[i];
                dict.Add($"{nameof(file.Name)}_{i}", file.Name);
                dict.Add($"{nameof(file.ContentType)}_{i}", file.ContentType);
                dict.Add($"{nameof(file.FileName)}_{i}", file.FileName);
                dict.Add($"{nameof(file.Length)}_{i}", file.Length);
            }

            return dict;
        }
        else
        {
            context.Request.EnableBuffering();

            context.Request.Body.Position = 0;
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (string.IsNullOrEmpty(body))
                return null;

            try
            {
                return JsonExtensions.Deserialize<object>(body);
            }
            catch
            {
                return body;
            }
        }
    }
}

