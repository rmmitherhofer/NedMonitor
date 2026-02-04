using NedMonitor.Core.Enums;
using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;

/// <summary>
/// Base configuration settings for ORM (Object-Relational Mapping) data logging.
/// Used for both EF Core and Dapper interceptors.
/// </summary>
internal abstract class ORMSettingsHttpRequest
{
    /// <summary>
    /// Indicates whether the interceptor is enabled.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    /// <summary>
    /// Specifies which parts of the query should be captured (e.g., SQL, parameters, context).
    /// </summary>
    [JsonPropertyName("captureOptions")]
    public List<CaptureOptions>? CaptureOptions { get; set; }
}