using NedMonitor.Core.Models;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NedMonitor.Core.Extensions;

/// <summary>
/// Extension methods to enrich <see cref="LogEntry"/> objects with caller member information.
/// </summary>
internal static class LogLevelsExtensions
{
    private static readonly string[] suffixesToRemove =
        [
            ".web.api",".webapi",".api",
            ".web.admin", ".webadmin",".admin",
            ".web.job",".webjob",".job",
            ".web",".app"
        ];

    /// <summary>
    /// Sets the member information (name, type, and source line number) on a <see cref="LogEntry"/>.
    /// It attempts to find the first stack frame belonging to the user-defined namespace to provide accurate caller info.
    /// </summary>
    /// <param name="entry">The log entry to enrich.</param>
    /// <param name="memberName">The caller member name, automatically supplied by the compiler.</param>
    /// <param name="lineNumber">The caller line number, automatically supplied by the compiler.</param>
    /// <param name="memberType">The caller file path or type, automatically supplied by the compiler.</param>
    /// <returns>The enriched <see cref="LogEntry"/> instance.</returns>
    internal static LogEntry Member(this LogEntry entry,
            [CallerMemberName] string? memberName = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string? memberType = null)
    {
        if (entry is null) return entry;

        var stackTrace = new StackTrace(true);
        StackFrame? userFrame = null;

        var userNamespace = Assembly.GetEntryAssembly()?.GetName().Name.ToLower() ?? "";

        foreach (var suffix in suffixesToRemove)
        {
            if (userNamespace.EndsWith(suffix))
            {
                userNamespace = userNamespace[..^suffix.Length];
                break;
            }
        }

        foreach (var frame in stackTrace.GetFrames() ?? [])
        {
            var method = frame.GetMethod();
            if (method?.DeclaringType?.Namespace != null &&
                method.DeclaringType.Namespace.StartsWith(userNamespace))
            {
                userFrame = frame;
                break;
            }
        }

        if (userFrame != null)
        {
            entry.MemberName = userFrame.GetMethod()?.Name ?? memberName;
            entry.MemberType = userFrame.GetMethod()?.DeclaringType?.FullName ?? memberType;
            entry.LineNumber = userFrame.GetFileLineNumber() != 0 ? userFrame.GetFileLineNumber() : lineNumber;
        }
        else
        {
            var firstFrame = stackTrace.GetFrame(0);
            entry.MemberName = memberName;
            entry.MemberType = firstFrame?.GetMethod()?.DeclaringType?.FullName ?? "Unknown";
            entry.LineNumber = lineNumber;
        }

        return entry;
    }
}
