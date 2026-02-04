namespace NedMonitor.Core.Models;

/// <summary>
/// Represents information about an external dependency call or operation.
/// </summary>
internal class DependencyInfo
{
    /// <summary>
    /// The type or category of the dependency.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The target or endpoint of the dependency.
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// Indicates whether the dependency call was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Duration of the dependency call in milliseconds.
    /// </summary>
    public int DurationMs { get; set; }
}
