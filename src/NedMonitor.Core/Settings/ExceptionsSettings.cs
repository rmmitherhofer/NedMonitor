namespace NedMonitor.Core.Settings;

public class ExceptionsSettings
{
    /// <summary>
    /// A list of fully qualified exception type names that should be treated as expected exceptions
    /// (i.e., not considered errors and may not trigger error logging).
    /// </summary>
    public List<string> Expected { get; set; } = [];
}