using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NedMonitor.Core.Enums;
using NedMonitor.Core.Helpers;
using NedMonitor.Core.Interfaces;
using NedMonitor.Core.Models;
using NedMonitor.Core.Settings;
using NedMonitor.EF.Cache;
using System.Data.Common;

namespace NedMonitor.EF.Interceptors;

/// <summary>
/// Entity Framework Core command interceptor used by NedMonitor to count and log executed database queries.
/// Captures execution metadata such as SQL command, parameters, duration, and exception messages during EF Core operations.
/// Stores query data in the current HTTP context for further analysis or remote transmission.
/// </summary>
public sealed class EfQueryCounter : DbCommandInterceptor
{
    private readonly EfInterceptorSettings _efSettings;
    private readonly IQueryCounter _counter;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EfQueryCounter(IQueryCounter counter, IHttpContextAccessor httpContextAccessor, IOptions<NedMonitorSettings> options)
    {
        _counter = counter;
        _httpContextAccessor = httpContextAccessor;
        _efSettings = options.Value.DataInterceptors.EF;
    }

    /// <summary>
    /// Increments the internal query counter for the current HTTP request.
    /// </summary>
    private void Increment() => _counter.Increment();
    /// <summary>
    /// Adds a query log entry to the current HTTP context, including details like SQL text, parameters, execution duration,
    /// ORM version, and exception information if applicable.
    /// </summary>
    /// <param name="command">The database command that was executed.</param>
    /// <param name="success">Indicates whether the query executed successfully.</param>
    /// <param name="exceptionMessage">The exception message, if the query failed.</param>
    /// <param name="durationMs">The duration of the query execution in milliseconds.</param>

    private void AddLog(DbCommand command, bool success, string? exceptionMessage, double? durationMs)
    {
        if (!_efSettings.Enabled.GetValueOrDefault()) return;
        if (_efSettings.CaptureOptions?.Any() != true || _efSettings.CaptureOptions.Contains(CaptureOptions.None)) return;

        var context = _httpContextAccessor.HttpContext;

        if (context is null) return;

        var capture = _efSettings.CaptureOptions;

        var entry = new DbQueryEntry
        {
            Provider = DbProviderExtractor.GetFriendlyProviderName(command.Connection),
            ExecutedAtUtc = DateTime.UtcNow,
            DurationMs = durationMs ?? 0,
            Success = success,
            ORM = $"EF Core {OrmVersionCache.EfVersion}"
        };

        if (capture.Contains(CaptureOptions.Query))
            entry.Sql = command.CommandText;

        if (capture.Contains(CaptureOptions.Parameters))
            entry.Parameters = string.Join(", ", command.Parameters.OfType<DbParameter>().Select(p => $"{p.ParameterName}={FormatValue(p.Value)}"));

        if (capture.Contains(CaptureOptions.ExceptionMessage))
            entry.ExceptionMessage = exceptionMessage;

        if (capture.Contains(CaptureOptions.Context) && command.Connection is not null)
            entry.DbContext = DbContextMetadataExtractor.Extract(command.Connection);

        QueryLogHelper.AddQueryLog(context, entry);
    }
    /// <summary>
    /// Formats a command parameter value for logging, including support for strings, nulls, DateTime, and byte arrays.
    /// </summary>
    /// <param name="value">The parameter value to format.</param>
    /// <returns>A string representation of the value.</returns>
    private static string FormatValue(object? value)
    {
        return value switch
        {
            null => "null",
            DateTime dt => dt.ToString("o"),
            string s => $"\"{s}\"",
            byte[] b => $"byte[{b.Length}]",
            _ => value?.ToString() ?? "null"
        };
    }

    #region SYNC
    /// <summary>
    /// Called before a synchronous reader command is executed. This method can be used to modify or inspect the command
    /// before it is sent to the database. In this implementation, it's overridden for completeness but does not perform any custom logic.
    /// </summary>
    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result) 
        => base.ReaderExecuting(command, eventData, result);

    /// <summary>
    /// Called after a synchronous reader command is executed. Logs query metadata and increments the query count.
    /// </summary>
    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        Increment();
        AddLog(command, true, null, eventData.Duration.TotalMilliseconds);
        return base.ReaderExecuted(command, eventData, result);
    }
    /// <summary>
    /// Called after a synchronous non-query command is executed. Logs query metadata and increments the query count.
    /// </summary>
    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        Increment();
        AddLog(command, true, null, eventData.Duration.TotalMilliseconds);
        return base.NonQueryExecuted(command, eventData, result);
    }
    /// <summary>
    /// Called after a synchronous scalar command is executed. Logs query metadata and increments the query count.
    /// </summary>
    public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
    {
        Increment();
        AddLog(command, true, null, eventData.Duration.TotalMilliseconds);
        return base.ScalarExecuted(command, eventData, result);
    }
    /// <summary>
    /// Called when a synchronous command fails. Logs error details and increments the query count.
    /// </summary>
    public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
    {
        Increment();
        AddLog(command, false, eventData.Exception?.Message, eventData.Duration.TotalMilliseconds);
        base.CommandFailed(command, eventData);
    }

    #endregion

    #region ASYNC
    /// <summary>
    /// Called after an asynchronous reader command is executed. Logs query metadata and increments the query count.
    /// </summary>
    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
    {
        Increment();
        AddLog(command, true, null, eventData.Duration.TotalMilliseconds);
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    /// <summary>
    /// Called after an asynchronous non-query command is executed. Logs query metadata and increments the query count.
    /// </summary>
    public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        Increment();
        AddLog(command, true, null, eventData.Duration.TotalMilliseconds);
        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }
    /// <summary>
    /// Called after an asynchronous scalar command is executed. Logs query metadata and increments the query count.
    /// </summary>
    public override ValueTask<object> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object result, CancellationToken cancellationToken = default)
    {
        Increment();
        AddLog(command, true, null, eventData.Duration.TotalMilliseconds);
        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }
    /// <summary>
    /// Called when an asynchronous command fails. Logs error details and increments the query count.
    /// </summary>
    public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        Increment();
        AddLog(command, false, eventData.Exception?.Message, eventData.Duration.TotalMilliseconds);
        return base.CommandFailedAsync(command, eventData, cancellationToken);
    }
    #endregion
}