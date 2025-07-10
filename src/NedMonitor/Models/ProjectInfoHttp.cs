using NedMonitor.Core.Enums;
using System.Text.Json.Serialization;

namespace NedMonitor.Models;


/// <summary>
/// Contains basic information about a project.
/// </summary>
public class ProjectInfoHttp
{
    /// <summary>
    /// Unique identifier of the project.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the project.
    /// </summary>
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    /// <summary>
    /// Type of the project.
    /// </summary>
    [JsonPropertyName("type")]
    public ProjectType Type { get; set; }

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
    /// Maximum size of the response body to capture, in megabytes.
    /// </summary>
    [JsonPropertyName("maxResponseBodySizeInMb")]
    public int MaxResponseBodySizeInMb { get; set; }

    /// <summary>
    /// Indicates whether to capture the response body for logging.
    /// </summary>
    [JsonPropertyName("captureResponseBody")]
    public bool CaptureResponseBody { get; set; }

    /// <summary>
    /// Indicates whether the request and response payload should be written to the console output.
    /// Useful for debugging during development.
    /// </summary>
    [JsonPropertyName("writePayloadToConsole")]
    public bool WritePayloadToConsole { get; set; }

    /// <summary>
    /// List of keywords used to identify and mask sensitive data (e.g., passwords, tokens, secrets) in logs.
    /// </summary>
    [JsonPropertyName("sensitiveKeys")]
    public List<string> SensitiveKeys { get; set; }
}


