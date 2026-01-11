using NedMonitor.Core.Enums;
using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;


/// <summary>
/// Represents the full context of a request log, including environment, request, response, diagnostics, and exceptions.
/// </summary>
public class LogContextHttpRequest
{
    /// <summary>
    /// Gets or sets the timestamp indicating when the operation started.
    /// </summary>
    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets the timestamp indicating when the operation ended.
    /// </summary>
    [JsonPropertyName("endTime")]
    public DateTime EndTime { get; set; }
    /// <summary>
    /// Attention level of the log.
    /// </summary>
    [JsonPropertyName("logAttentionLevel")]
    public LogAttentionLevel LogAttentionLevel { get; set; }

    /// <summary>
    /// The correlation ID for tracking requests across services.
    /// </summary>
    [JsonPropertyName("correlationId")]
    public string CorrelationId { get; set; }

    /// <summary>
    /// The HTTP endpoint path requested.
    /// </summary>
    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    /// <summary>
    /// Gets or sets the route template of the HTTP request URI, such as "/api/customers/{id}".
    /// Useful for identifying the logical endpoint regardless of route parameter values.
    /// </summary>
    [JsonPropertyName("uriTemplate")]
    public string? UriTemplate { get; set; }

    /// <summary>
    /// Elapsed time of the request in milliseconds.
    /// </summary>
    [JsonPropertyName("totalMilliseconds")]
    public double TotalMilliseconds { get; set; }
    /// <summary>
    /// Unique identifier for the request trace.
    /// </summary>
    [JsonPropertyName("traceIdentifier")]
    public string? TraceIdentifier { get; set; }

    /// <summary>
    /// Category of the detected error, if applicable.
    /// </summary>
    [JsonPropertyName("errorCategory")]
    public string? ErrorCategory { get; set; }

    /// <summary>
    /// Information about the project generating the log.
    /// </summary>
    [JsonPropertyName("project")]
    public ProjectInfoHttp Project { get; set; }

    /// <summary>
    /// Information about the environment where the application is running.
    /// </summary>
    [JsonPropertyName("environment")]
    public EnvironmentInfoHttpRequest Environment { get; set; }

    /// <summary>
    /// Authenticated user information, if available.
    /// </summary>
    [JsonPropertyName("user")]
    public UserInfoHttpRequest User { get; set; }

    /// <summary>
    /// HTTP request information.
    /// </summary>
    [JsonPropertyName("request")]
    public RequestInfoHttpRequest Request { get; set; }

    /// <summary>
    /// HTTP response information.
    /// </summary>
    [JsonPropertyName("response")]
    public ResponseInfoHttpRequest Response { get; set; }

    /// <summary>
    /// Diagnostic data such as memory usage, dependencies, and cache hits.
    /// </summary>
    [JsonPropertyName("diagnostic")]
    public DiagnosticHttpRequest Diagnostic { get; set; }

    /// <summary>
    /// Notifications generated during the request processing.
    /// </summary>
    [JsonPropertyName("notifications")]
    public IEnumerable<NotificationInfoHttpRequest>? Notifications { get; set; }

    /// <summary>
    /// Detailed log entries (messages, levels, sources).
    /// </summary>
    [JsonPropertyName("logEntries")]
    public IEnumerable<LogEntryHttpRequest>? LogEntries { get; set; }

    /// <summary>
    /// Captured exception details during processing.
    /// </summary>
    [JsonPropertyName("exceptions")]
    public IEnumerable<ExceptionInfoHttpRequest>? Exceptions { get; set; }
    /// <summary>
    /// Logs related to outbound HTTP client calls made during the request.
    /// </summary>
    [JsonPropertyName("httpClientLogs")]
    public IEnumerable<HttpClientLogInfoHttpRequest>? HttpClientLogs { get; set; }
    /// <summary>
    /// Collection of database queries associated with the current HTTP request.
    /// </summary>
    [JsonPropertyName("dbQueryEntries")]
    public IEnumerable<DbQueryEntryHttpRequest>? DbQueryEntries { get; set; }

    /// <summary>
    /// Remote port number from which the request originated.
    /// </summary>
    [JsonPropertyName("remotePort")]
    public int RemotePort { get; set; }

    /// <summary>
    /// Local port number on which the server received the request.
    /// </summary>
    [JsonPropertyName("localPort")]
    public int LocalPort { get; set; }

    /// <summary>
    /// IP address of the server that handled the request.
    /// </summary>
    [JsonPropertyName("localIpAddress")]
    public string LocalIpAddress { get; set; }
}