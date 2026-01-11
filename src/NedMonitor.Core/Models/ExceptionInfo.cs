namespace NedMonitor.Core.Models;

/// <summary>
/// Represents a single exception captured by NedMonitor.
/// </summary>
public class ExceptionInfo
{
    /// <summary>
    /// The full type name of the exception (e.g., System.NullReferenceException).
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The exception message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// The full stack trace if available.
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Serialized inner exception, if any.
    /// </summary>
    public string? InnerException { get; set; }

    /// <summary>
    /// Timestamp when the exception was captured.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Optional context or source where the exception was thrown (e.g., class/method name).
    /// </summary>
    public string? Source { get; set; }
}
