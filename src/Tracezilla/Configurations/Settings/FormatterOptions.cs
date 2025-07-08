using Tracezilla.Formatters;

namespace Tracezilla.Configurations.Settings;

/// <summary>
/// Configuration options for Tracezilla logger.
/// </summary>
public class FormatterOptions
{
    /// <summary>
    /// Optional formatter for customizing log messages.
    /// </summary>
    public Func<FormatterArgs, string> Formatter { get; set; }
}
