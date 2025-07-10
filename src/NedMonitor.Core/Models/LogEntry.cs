using Microsoft.Extensions.Logging;

namespace NedMonitor.Core.Models;

/// <summary>
/// Represents a single log entry containing metadata about the log event,
/// including severity, category, origin, and timestamp.
/// </summary>
public class LogEntry
{
    /// <summary>
    /// The category of the log, typically the source class or context.
    /// </summary>
    public string Category { get; private set; }

    /// <summary>
    /// The severity level of the log (e.g., Information, Warning, Error).
    /// </summary>
    public LogLevel LogLevel { get; private set; }

    /// <summary>
    /// The message or content of the log.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// The type name of the member where the log was generated (e.g., class or interface name).
    /// </summary>
    public string? MemberType { get; set; }

    /// <summary>
    /// The name of the method or property where the log originated.
    /// </summary>
    public string? MemberName { get; set; }

    /// <summary>
    /// The line number in the source file where the log was written.
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// The UTC timestamp indicating when the log was created.
    /// </summary>
    public DateTime DateTime { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class.
    /// </summary>
    /// <param name="category">The category or context of the log.</param>
    /// <param name="logLevel">The severity level of the log.</param>
    /// <param name="message">The log message.</param>
    public LogEntry(string category, LogLevel logLevel, string message)
    {
        Category = category;
        LogLevel = logLevel;
        Message = message;
        DateTime = DateTime.UtcNow;
    }
}
