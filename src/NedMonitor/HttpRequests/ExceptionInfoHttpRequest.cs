using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;

/// <summary>
/// Represents detailed information about an exception.
/// </summary>
public class ExceptionInfoHttpRequest
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
}
