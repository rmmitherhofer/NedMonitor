namespace NedMonitor.Core.Settings;

public class HttpLoggingSettings
{
    /// <summary>
    /// Indicates whether the request and response payload should be written to the console output.
    /// Useful for debugging during development.
    /// </summary>
    public bool WritePayloadToConsole { get; set; }
    /// <summary>
    /// Indicates whether to capture the response body for logging.
    /// </summary>
    public bool CaptureResponseBody { get; set; }
    /// <summary>
    /// Maximum size of the response body to capture, in megabytes.
    /// </summary>
    public int MaxResponseBodySizeInMb { get; set; } = 1;
    /// <summary>
    /// Determines whether the cookies from the incoming HTTP request should be captured for monitoring and logging purposes.
    /// </summary>
    public bool CaptureCookies { get; set; }

}