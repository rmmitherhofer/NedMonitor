using System.Text.Json.Serialization;
using Tracezilla.Enums;

namespace Tracezilla.Models;


/// <summary>
/// Represents the full context of a request log, including environment, request, response, diagnostics, and exceptions.
/// </summary>
public class LogContextRequest
{
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
    [JsonPropertyName("Path")]
    public string Path { get; set; }

    /// <summary>
    /// Formatted duration of the request.
    /// </summary>
    [JsonPropertyName("requestDuration")]
    public string RequestDuration { get; set; }

    /// <summary>
    /// Elapsed time of the request in milliseconds.
    /// </summary>
    [JsonPropertyName("elapsedMilliseconds")]
    public long ElapsedMilliseconds { get; set; }
    /// <summary>
    /// Unique identifier for the request trace.
    /// </summary>
    [JsonPropertyName("traceIdentifier")]
    public string TraceIdentifier { get; set; }

    /// <summary>
    /// Category of the detected error, if applicable.
    /// </summary>
    [JsonPropertyName("errorCategory")]
    public string ErrorCategory { get; set; }

    /// <summary>
    /// Information about the project generating the log.
    /// </summary>
    [JsonPropertyName("project")]
    public ProjectInfo Project { get; set; }

    /// <summary>
    /// Information about the environment where the application is running.
    /// </summary>
    [JsonPropertyName("environment")]
    public EnvironmentInfo Environment { get; set; }

    /// <summary>
    /// Authenticated user information, if available.
    /// </summary>
    [JsonPropertyName("user")]
    public UserContextRequest User { get; set; }

    /// <summary>
    /// HTTP request information.
    /// </summary>
    [JsonPropertyName("request")]
    public RequestInfoRequest Request { get; set; }

    /// <summary>
    /// HTTP response information.
    /// </summary>
    [JsonPropertyName("response")]
    public ResponseInfoRequest Response { get; set; }

    /// <summary>
    /// Diagnostic data such as memory usage, dependencies, and cache hits.
    /// </summary>
    [JsonPropertyName("diagnostic")]
    public DiagnosticRequest Diagnostic { get; set; }

    /// <summary>
    /// Notifications generated during the request processing.
    /// </summary>
    [JsonPropertyName("notifications")]
    public IEnumerable<NotificationInfoRequest> Notifications { get; set; }

    /// <summary>
    /// Detailed log entries (messages, levels, sources).
    /// </summary>
    [JsonPropertyName("logEntries")]
    public IEnumerable<LogEntryRequest> LogEntries { get; set; }

    /// <summary>
    /// Captured exception details during processing.
    /// </summary>
    [JsonPropertyName("exceptions")]
    public IEnumerable<ExceptionInfoRequest> Exceptions { get; set; }
}