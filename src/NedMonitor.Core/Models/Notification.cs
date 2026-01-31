using Microsoft.Extensions.Logging;

namespace NedMonitor.Core.Models;

/// <summary>
/// Represents a domain notification with metadata such as timestamp, log level, and content.
/// </summary>
public class Notification
{
    /// <summary>
    /// Type of the message.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Aggregation Id.
    /// </summary>
    public Guid AgregationId { get; set; }
    /// <summary>
    /// Unique identifier of the notification.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Timestamp when the notification was created.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Logging level associated with the notification.
    /// </summary>
    public LogLevel LogLevel { get; set; }

    /// <summary>
    /// Optional key that identifies the context of the notification.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Main content or value of the notification.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Optional detailed message.
    /// </summary>
    public string Detail { get; set; }
}
