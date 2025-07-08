using NedMonitor.Enums;
using System.Reflection;

namespace NedMonitor.Configurations.Settings;

/// <summary>
/// Main configuration settings for NedMonitor, including project identification,
/// type, name, and logging parameters.
/// </summary>
public class NedMonitorSettings
{
    internal const string NEDMONITOR_NODE = "NedMonitor";
    /// <summary>
    /// Unique identifier for the project.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Type of the project (defined by the <see cref="ProjectType"/> enum).
    /// </summary>
    public ProjectType ProjectType { get; set; }

    /// <summary>
    /// Name of the project, automatically obtained from the entry assembly.
    /// </summary>
    public string Name { get; } = Assembly.GetEntryAssembly().GetName().Name;

    /// <summary>
    /// Defines the current logging behavior of NedMonitor.
    /// </summary>
    public ExecutionMode ExecutionMode { get; set; }

    /// <summary>
    /// Maximum size of the response body to capture, in megabytes.
    /// </summary>
    public int MaxResponseBodySizeInMb { get; set; } = 1;

    /// <summary>
    /// Indicates whether to capture the response body for logging.
    /// </summary>
    public bool CaptureResponseBody { get; set; } = true;

    /// <summary>
    /// HTTP service specific settings for NedMonitor.
    /// </summary>
    public HttpServiceSettings Service { get; set; }

    /// <summary>
    /// Indicates whether the request and response payload should be written to the console output.
    /// Useful for debugging during development.
    /// </summary>
    public bool WritePayloadToConsole { get; set; } = false;

    /// <summary>
    /// Configuration options for masking sensitive data in logs, such as passwords or tokens.
    /// </summary>
    public SensitiveDataMaskerOptions? SensitiveDataMasker { get; set; }
}

