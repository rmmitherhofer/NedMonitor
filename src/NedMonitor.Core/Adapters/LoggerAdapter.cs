using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NedMonitor.Core.Formatters;
using NedMonitor.Core.Models;
using NedMonitor.Core.Settings;
using NedMonitor.Extensions;

namespace NedMonitor.Core.Adapters;

/// <summary>
/// Custom logger that stores log entries per HTTP request context.
/// </summary>
public class LoggerAdapter : ILogger
{
    private readonly string _category;
    private readonly FormatterOptions _options;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerAdapter"/> class.
    /// </summary>
    /// <param name="options">The logger options.</param>
    /// <param name="category">The logger category.</param>
    /// <param name="httpContextAccessor">HTTP context accessor.</param>
    public LoggerAdapter(FormatterOptions options, string category, IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(FormatterOptions));

        _options = options;
        _category = category;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Begins a logical operation scope. Not implemented.
    /// </summary>
    public IDisposable BeginScope<TState>(TState state) => null;

    /// <summary>
    /// Determines whether the given log level is enabled. Always returns true.
    /// </summary>
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <summary>
    /// Logs a message to the current HTTP context.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="logLevel">The log level.</param>
    /// <param name="eventId">The event ID.</param>
    /// <param name="state">The log state.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="formatter">The message formatter.</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (formatter == null) return;

        var message = formatter(state, exception);

        if (_options.Formatter != null)
        {
            FormatterArgs formatterArgs = new(new FormatterArgs.CreateOptions
            {
                State = state,
                Exception = exception,
                DefaultValue = message,
            });

            string custom = _options.Formatter.Invoke(formatterArgs);

            message = string.IsNullOrEmpty(custom) ? message : custom;
        }

        var logEntry = new LogEntry(_category, logLevel, message).Member();

        var context = _httpContextAccessor.HttpContext;

        if (context == null) return;

        if (!context.Items.TryGetValue(NedMonitorConstants.CONTEXT_LOGS_KEY, out var obj) || obj is not List<LogEntry> logs)
        {
            logs = [];
            context.Items[NedMonitorConstants.CONTEXT_LOGS_KEY] = logs;
        }
        logs.Add(logEntry);
    }

    /// <summary>
    /// Gets the logs for the current HTTP request.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>The list of log entries.</returns>
    public static IEnumerable<LogEntry> GetLogsForCurrentRequest(HttpContext context)
    {
        if (context.Items.TryGetValue(NedMonitorConstants.CONTEXT_LOGS_KEY, out var obj) && obj is IEnumerable<LogEntry> logs)
            return logs;

        return [];
    }
}
