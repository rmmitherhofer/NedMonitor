using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace NedMonitor.Models;

/// <summary>
/// Represents a notification log entry with details such as level, key, value, and timestamp.
/// </summary>
public class NotificationInfoRequest
    {
        /// <summary>
        /// Unique identifier of the notification.
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Timestamp when the notification was created.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Severity level of the log.
        /// </summary>
        [JsonPropertyName("logLevel")]
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Optional key or category related to the notification.
        /// </summary>
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        /// <summary>
        /// The notification message or value.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }

        /// <summary>
        /// Optional detailed information about the notification.
        /// </summary>
        [JsonPropertyName("detail")]
        public string? Detail { get; set; }
   }

