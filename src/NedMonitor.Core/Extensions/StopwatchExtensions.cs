using System.Diagnostics;

namespace NedMonitor.Core.Extensions;

/// <summary>
/// Extension methods for <see cref="Stopwatch"/> and time formatting utilities.
/// </summary>
public static class StopwatchExtensions
{
    /// <summary>
    /// Returns a formatted string of the stopwatch's elapsed time in the format "HH:mm:ss.fff".
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>A formatted time string.</returns>
    public static string GetFormattedTime(this Stopwatch stopwatch)
    {
        ArgumentNullException.ThrowIfNull(stopwatch);
        return stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
    }
}