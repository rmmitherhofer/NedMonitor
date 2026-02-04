using Microsoft.Extensions.Logging;
using NedMonitor.Core.Enums;
using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;


/// <summary>
/// Contains basic information about a project.
/// </summary>
internal class ProjectInfoHttp
{
    /// <summary>
    /// Unique identifier for the project.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Type of the project (defined by the <see cref="ProjectType"/> enum).
    /// </summary>
    [JsonPropertyName("type")]
    public ProjectType Type { get; set; }

    /// <summary>
    /// Name of the project, automatically obtained from the entry assembly.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Defines the execution mode settings that control how NedMonitor behaves during runtime,
    /// such as which features to enable (e.g., logging, notifications, exceptions).
    /// </summary>
    [JsonPropertyName("executionMode")]
    public ExecutionModeSettingsHttpRequest ExecutionMode { get; set; }

    /// <summary>
    /// Configuration settings related to HTTP request and response logging.
    /// </summary>
    [JsonPropertyName("httpLogging")]
    public HttpLoggingSettingsHttpRequest? HttpLogging { get; set; }

    /// <summary>
    /// Configuration options for masking sensitive data in logs, such as passwords or tokens.
    /// </summary>
    [JsonPropertyName("sensitiveDataMasking")]
    public SensitiveDataMaskerSettingsHttpRequest? SensitiveDataMasking { get; set; }

    /// <summary>
    /// Settings for capturing and handling exceptions during request processing.
    /// </summary>
    [JsonPropertyName("exceptions")]
    public ExceptionsSettingsHttpRequest? Exceptions { get; set; }

    /// <summary>
    /// Settings related to database interceptors for logging EF and Dapper queries.
    /// </summary>
    [JsonPropertyName("dataInterceptors")]
    public DataInterceptorsSettingsHttpRequest? DataInterceptors { get; set; }

    /// <summary>
    /// Defines the minimum log level to be captured and stored during a request lifecycle.
    /// Log entries below this level will be ignored.
    /// </summary>
    [JsonPropertyName("minimumLogLevel")]
    public LogLevel MinimumLogLevel { get; set; }
}