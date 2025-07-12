using Microsoft.Extensions.Logging;
using NedMonitor.Core.Enums;
using System.Reflection;
using System.Text.Json.Serialization;

namespace NedMonitor.Core.Settings;

/// <summary>
/// Main configuration settings for NedMonitor, including project identification,
/// type, name, and logging parameters.
/// </summary>
public class NedMonitorSettings
{
    /// <summary>
    /// Configuration section node name used in appsettings.json or environment configuration.
    /// </summary>
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
    /// Defines the execution mode settings that control how NedMonitor behaves during runtime,
    /// such as which features to enable (e.g., logging, notifications, exceptions).
    /// </summary>
    public ExecutionModeSettings ExecutionMode { get; set; } = new();

    /// <summary>
    /// HTTP logging behavior, including options for capturing and limiting request/response data.
    /// </summary>
    public HttpLoggingSettings HttpLogging { get; set; } = new();

    /// <summary>
    /// Configuration options for masking sensitive data in logs, such as passwords or tokens.
    /// </summary>
    public SensitiveDataMaskerSettings? SensitiveDataMasking { get; set; }

    /// <summary>
    /// Settings for expected exceptions that should not be treated as errors.
    /// </summary>
    public ExceptionsSettings Exceptions { get; set; } = new();

    /// <summary>
    /// Settings for intercepting and logging database operations (EF and Dapper).
    /// </summary>
    public DataInterceptorsSettings DataInterceptors { get; set; }

    /// <summary>
    /// HTTP service-specific settings for NedMonitor (e.g., base address, endpoints).
    /// </summary>
    public RemoteServiceSettings RemoteService { get; set; }

    /// <summary>
    /// Defines the minimum log level to be captured and stored during a request lifecycle.
    /// Log entries below this level will be ignored.
    /// </summary>
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;
}