using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;

/// <summary>
/// Configuration settings related to handling and logging exceptions.
/// </summary>
internal class ExceptionsSettingsHttpRequest
{
    /// <summary>
    /// A list of fully qualified exception type names that should be treated as expected exceptions
    /// (i.e., not considered errors and may not trigger error logging).
    /// </summary>
    [JsonPropertyName("expected")]
    public List<string> Expected { get; set; }
}