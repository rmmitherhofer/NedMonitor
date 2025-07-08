namespace NedMonitor.Enums;

/// <summary>
/// Represents the severity or attention level assigned to a log entry,
/// helping consumers prioritize or filter log data based on importance.
/// </summary>
public enum LogAttentionLevel
{
    /// <summary>
    /// No attention is required. Used when severity is undefined or not applicable.
    /// </summary>
    None = 0,

    /// <summary>
    /// Low priority. Typically informational messages or minor issues.
    /// </summary>
    Low = 1,

    /// <summary>
    /// Medium priority. Issues that may require monitoring but do not impact critical functionality.
    /// </summary>
    Medium = 2,

    /// <summary>
    /// High priority. Errors or behaviors that should be addressed promptly.
    /// </summary>
    High = 3,

    /// <summary>
    /// Critical priority. Represents severe failures, system outages, or security breaches.
    /// </summary>
    Critical = 4
}
