namespace NedMonitor.Core.Settings;

/// <summary>
/// Configuration options for sensitive data masking.
/// Defines the keys that should be identified and protected when processing data.
/// </summary>
public class SensitiveDataMaskerSettings
{
    /// <summary>
    /// Indicates whether sensitive data masking is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// List of keys considered sensitive whose values should be masked.
    /// </summary>
    public List<string> SensitiveKeys { get; set; }

    /// <summary>
    /// The value that replaces the original sensitive data (e.g., "***REDACTED***").
    /// </summary>
    public string MaskValue { get; set; } = "***REDACTED***";
}