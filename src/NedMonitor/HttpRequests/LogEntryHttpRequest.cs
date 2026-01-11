using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;

/// <summary>
/// Represents a single log entry with detailed information.
/// </summary>
public class LogEntryHttpRequest
{
    /// <summary>
    /// Category or source of the log entry.
    /// </summary>
    [JsonPropertyName("logCategory")]
    public string LogCategory { get; set; }

    /// <summary>
    /// Severity level of the log entry.
    /// </summary>
    [JsonPropertyName("logSeverity")]
    public LogLevel LogSeverity { get; set; }

    /// <summary>
    /// The log message content.
    /// </summary>
    [JsonPropertyName("logMessage")]
    public string LogMessage { get; set; }

    /// <summary>
    /// The fully qualified type name where the log was generated.
    /// </summary>
    [JsonPropertyName("memberType")]
    public string? MemberType { get; set; }

    /// <summary>
    /// The name of the method or member generating the log.
    /// </summary>
    [JsonPropertyName("memberName")]
    public string? MemberName { get; set; }

    /// <summary>
    /// The source code line number where the log was generated.
    /// </summary>
    [JsonPropertyName("sourceLineNumber")]
    public int SourceLineNumber { get; set; }

    /// <summary>
    /// Timestamp when the log entry was created.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}
