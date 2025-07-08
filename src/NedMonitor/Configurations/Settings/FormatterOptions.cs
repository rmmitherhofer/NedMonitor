using NedMonitor.Formatters;

namespace NedMonitor.Configurations.Settings;

/// <summary>
/// Configuration options for NedMonitor logger.
/// </summary>
public class FormatterOptions
{
    /// <summary>
    /// Optional formatter for customizing log messages.
    /// </summary>
    public Func<FormatterArgs, string> Formatter { get; set; }
}
