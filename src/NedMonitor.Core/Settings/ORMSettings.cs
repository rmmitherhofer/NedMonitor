using NedMonitor.Core.Enums;

namespace NedMonitor.Core.Settings;


/// <summary>
/// Base configuration for ORM (Object-Relational Mapping) data interceptors.
/// Defines common settings such as enabling the interceptor and what data to capture.
/// </summary>
public abstract class ORMSettings
{
    /// <summary>
    /// Indicates whether the ORM interceptor is enabled.
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// Specifies which elements of the query (e.g., SQL, parameters, context) should be captured.
    /// </summary>
    public List<CaptureOptions> CaptureOptions { get; set; } = [];
}