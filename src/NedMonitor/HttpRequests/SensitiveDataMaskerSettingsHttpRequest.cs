using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;

/// <summary>
/// Represents the configuration settings for sensitive data masking in HTTP requests.
/// </summary>
public class SensitiveDataMaskerSettingsHttpRequest
{
    /// <summary>
    /// Indicates whether sensitive data masking is enabled.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    /// <summary>
    /// A list of sensitive keys that should be masked in logs or outputs.
    /// </summary>
    [JsonPropertyName("sensitiveKeys")]
    public List<string>? SensitiveKeys { get; set; }

    /// <summary>
    /// The value used to replace the sensitive data (e.g., "***").
    /// </summary>
    [JsonPropertyName("maskValue")]
    public string? MaskValue { get; set; }
}