using NedMonitor.Core.Formatters;

namespace NedMonitor.Core.Settings;

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