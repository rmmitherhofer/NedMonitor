using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace NedMonitor.HttpResponses;

/// <summary>
/// Represents a notification message returned by the API, typically used for logging or audit purposes.
/// </summary>
internal class NotificationHttpResponse : MessageHttpResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the notification.
    /// </summary>

    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the notification was generated.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the log level that describes the severity of the notification.
    /// </summary>

    [JsonPropertyName("logLevel")]
    public LogLevel? LogLevel { get; set; }

    /// <summary>
    /// Gets or sets an optional key used to categorize or identify the message.
    /// </summary>

    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the main message value or content.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets additional detailed information associated with the notification.
    /// </summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
}
