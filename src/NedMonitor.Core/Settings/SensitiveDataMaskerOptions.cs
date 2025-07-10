namespace NedMonitor.Core.Settings;

/// <summary>
/// Configuration options for sensitive data masking.
/// Defines the keys that should be identified and protected when processing data.
/// </summary>
public class SensitiveDataMaskerOptions
{
    /// <summary>
    /// List of keys considered sensitive whose values should be masked.
    /// </summary>
    public List<string> SensitiveKeys { get; set; }
}
