using System.Text.Json.Serialization;

namespace Tracezilla.Models;

/// <summary>
/// Represents information about an external dependency call or operation.
/// </summary>
public class DependencyInfoRequest
{
    /// <summary>
    /// The type or category of the dependency.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// The target or endpoint of the dependency.
    /// </summary>
    [JsonPropertyName("target")]
    public string Target { get; set; }

    /// <summary>
    /// Indicates whether the dependency call was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Duration of the dependency call in milliseconds.
    /// </summary>
    [JsonPropertyName("durationMs")]
    public int DurationMs { get; set; }
}
