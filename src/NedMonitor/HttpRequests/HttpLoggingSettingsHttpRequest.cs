using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;

/// <summary>
/// Configuration settings related to HTTP request and response logging.
/// </summary>
public class HttpLoggingSettingsHttpRequest
{
    /// <summary>
    /// Indicates whether the request and response payload should be written to the console output.
    /// Useful for debugging during development.
    /// </summary>
    [JsonPropertyName("writePayloadToConsole")]
    public bool WritePayloadToConsole { get; set; }

    /// <summary>
    /// Indicates whether to capture the response body for logging.
    /// </summary>
    [JsonPropertyName("captureResponseBody")]
    public bool CaptureResponseBody { get; set; }

    /// <summary>
    /// Maximum size of the response body to capture, in megabytes.
    /// </summary>
    [JsonPropertyName("maxResponseBodySizeInMb")]
    public int MaxResponseBodySizeInMb { get; set; } = 1;

    /// <summary>
    /// Determines whether the cookies from the incoming HTTP request should be captured for monitoring and logging purposes.
    /// </summary>
    [JsonPropertyName("captureCookies")]

    public bool CaptureCookies { get; set; }
}