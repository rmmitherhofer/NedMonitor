using Microsoft.Extensions.Logging;
using NedMonitor.Core.Enums;

namespace NedMonitor.Core.Extensions;

internal static class LogAttentionMapper
{
    /// <summary>
    /// Maps a Microsoft LogLevel to the corresponding attention level.
    /// </summary>
    internal static LogAttentionLevel Map(this LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Trace => LogAttentionLevel.None,
            LogLevel.Debug => LogAttentionLevel.None,
            LogLevel.Information => LogAttentionLevel.Low,
            LogLevel.Warning => LogAttentionLevel.Medium,
            LogLevel.Error => LogAttentionLevel.High,
            LogLevel.Critical => LogAttentionLevel.Critical,
            _ => LogAttentionLevel.None
        };
}