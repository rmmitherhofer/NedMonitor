using NedMonitor.Core.Enums;
using System.Reflection;

namespace NedMonitor.Core.Settings;

/// <summary>
/// Main configuration settings for NedMonitor, including project identification,
/// type, name, and logging parameters.
/// </summary>
public class NedMonitorSettings
{
    public const string NEDMONITOR_NODE = "NedMonitor";
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
    /// <summary>
    /// Defines the execution mode settings that control how NedMonitor behaves during runtime,
    /// such as which features to enable (e.g., logging, notifications, exceptions).
    /// </summary>
    public ExecutionModeSettings ExecutionMode { get; set; } = new();
    /// <summary>
    /// A list of fully qualified exception type names that should be treated as expected exceptions
    /// (i.e., not considered errors and may not trigger error logging).
    /// </summary>
    public List<string> ExpectedExceptions { get; set; } = new();
}

