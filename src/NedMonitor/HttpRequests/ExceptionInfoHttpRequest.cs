using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;

/// <summary>
/// Represents detailed information about an exception.
/// </summary>
internal class ExceptionInfoHttpRequest
{
    /// <summary>
    /// The type or class name of the exception.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// The exception message.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }

    /// <summary>
    /// Optional stack trace or tracer information.
    /// </summary>
    [JsonPropertyName("tracer")]
    public string? Tracer { get; set; }

    /// <summary>
    /// Optional detailed additional information about the exception.
    /// </summary>
    [JsonPropertyName("innerException")]
    public string? InnerException { get; set; }

    /// <summary>
    /// Timestamp when the exception was captured.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Optional context or source where the exception was thrown (e.g., class/method name).
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }
}
