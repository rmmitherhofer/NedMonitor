using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;

/// <summary>
/// Represents feature flags for enabling or disabling individual NedMonitor features.
/// Allows granular control over what should be logged or monitored.
/// </summary>
internal class ExecutionModeSettingsHttpRequest
{
    /// <summary>
    /// Enables or disables the NedMonitor entirely.
    /// </summary>
    [JsonPropertyName("enableNedMonitor")]
    public bool EnableNedMonitor { get; set; } = true;

    /// <summary>
    /// Enables capturing of unhandled exceptions.
    /// </summary>
    [JsonPropertyName("enableMonitorExceptions")]
    public bool EnableMonitorExceptions { get; set; } = true;

    /// <summary>
    /// Enables capturing of domain or validation notifications.
    /// </summary>
    [JsonPropertyName("enableMonitorNotifications")]
    public bool EnableMonitorNotifications { get; set; }

    /// <summary>
    /// Enables capturing of application logs (e.g., console logs).
    /// </summary>
    [JsonPropertyName("enableMonitorLogs")]
    public bool EnableMonitorLogs { get; set; }

    /// <summary>
    /// Enables logging of outgoing HttpClient requests.
    /// </summary>
    [JsonPropertyName("enableMonitorHttpRequests")]
    public bool EnableMonitorHttpRequests { get; set; }

    /// <summary>
    /// Enables logging and monitoring of database queries (Entity Framework, Dapper, etc.).
    /// </summary>
    [JsonPropertyName("enableMonitorDbQueries")]
    public bool EnableMonitorDbQueries { get; set; }
}