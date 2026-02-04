using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NedMonitor.Core.Enums;
using NedMonitor.Core.Extensions;
using NedMonitor.Core.Helpers;
using NedMonitor.Core.Interfaces;
using NedMonitor.Core.Models;
using NedMonitor.Core.Settings;
using NedMonitor.Dapper.Cache;
using NedMonitor.Dapper.Extensions;
using System.Data;
using System.Diagnostics;

namespace NedMonitor.Dapper.Interceptors;

/// <summary>
/// Wraps an existing <see cref="IDbConnection"/> instance to intercept Dapper operations.
/// It increments the query counter during execution and logs query metadata to the current HTTP context when enabled.
/// Designed for use with NedMonitor to capture diagnostic and performance data.
/// </summary>
public class CountingDbConnection : IDbConnection
{
    private readonly DapperInterceptorSettings _settings;
    private readonly IDbConnection _inner;
    private readonly IQueryCounter _counter;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CountingDbConnection"/> class.
    /// </summary>
    /// <param name="inner">The underlying <see cref="IDbConnection"/> being wrapped.</param>
    /// <param name="counter">The query counter used to track database operations.</param>
    /// <param name="httpContextAccessor">The accessor to retrieve the current HTTP context.</param>
    /// <param name="options">Configuration settings for NedMonitor.</param>

    public CountingDbConnection(IDbConnection inner, IQueryCounter counter, IHttpContextAccessor httpContextAccessor, IOptions<NedMonitorSettings> options)
    {
        _inner = inner;
        _counter = counter;
        _httpContextAccessor = httpContextAccessor;
        _settings = options.Value.DataInterceptors.Dapper;
    }

    #region IDbConnection Members
    public string ConnectionString { get => _inner.ConnectionString; set => _inner.ConnectionString = value; }
    public int ConnectionTimeout => _inner.ConnectionTimeout;
    public string Database => _inner.Database;
    public ConnectionState State => _inner.State;
    public IDbTransaction BeginTransaction() => _inner.BeginTransaction();
    public IDbTransaction BeginTransaction(IsolationLevel il) => _inner.BeginTransaction(il);
    public void ChangeDatabase(string databaseName) => _inner.ChangeDatabase(databaseName);
    public void Close() => _inner.Close();
    public IDbCommand CreateCommand() => _inner.CreateCommand();
    public void Open() => _inner.Open();
    public void Dispose() => _inner.Dispose();
    #endregion

    #region Dapper Intercepted Methods

    #region Execute
    public int Execute(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.Execute(sql, param, transaction, commandTimeout, commandType), sql, param);
    #endregion

    #region ExecuteAsync
    public Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLoggingAsync(() => _inner.ExecuteAsync(sql, param, transaction, commandTimeout, commandType), sql, param);
    #endregion

    #region Query
    public IEnumerable<T> Query<T>(string sql, object? param = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType), sql, param);
    public IEnumerable<dynamic> Query(string sql, object? param = null, IDbTransaction? trans = null, bool buffered = true, int? timeout = null, CommandType? type = null)
        => ExecuteWithLogging(() => _inner.Query(sql, param, trans, buffered, timeout, type), sql, param);
    public IEnumerable<object> Query(Type type, string sql, object? param = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.Query(type, sql, param, transaction, buffered, commandTimeout, commandType), sql, param);
    public IEnumerable<T> Query<T>(CommandDefinition command)
        => ExecuteWithLogging(() => _inner.Query<T>(command), command.CommandText, command.Parameters);
    public IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);
    public IEnumerable<TReturn> Query<TReturn>(string sql, Type[] types, Func<object[], TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.Query<TReturn>(sql, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);

    #endregion

    #region QueryAsync
    public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLoggingAsync(() => _inner.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLoggingAsync(() => _inner.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLoggingAsync(() => _inner.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLoggingAsync(() => _inner.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLoggingAsync(() => _inner.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) => ExecuteWithLoggingAsync(()
        => _inner.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) => ExecuteWithLoggingAsync(()
        => _inner.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);

    #endregion

    #region QueryFirst
    public T QueryFirst<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.QueryFirst<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
    #endregion

    #region QueryFirstAsync
    public Task<T> QueryFirstAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLoggingAsync(() => _inner.QueryFirstAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
    #endregion

    #region QueryFirstOrDefault
    public T? QueryFirstOrDefault<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
    #endregion

    #region QueryFirstOrDefaultAsync
    public Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLoggingAsync(() => _inner.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
    #endregion

    #region QuerySingle
    public T QuerySingle<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.QuerySingle<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
    #endregion

    #region QuerySingleAsync
    public Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLoggingAsync(() => _inner.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
    #endregion

    #region QuerySingleOrDefault
    public T? QuerySingleOrDefault<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.QuerySingleOrDefault<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
    #endregion

    #region QuerySingleOrDefaultAsync
    public Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLoggingAsync(() => _inner.QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);

    #endregion

    #region ExecuteScalar
    public object? ExecuteScalar(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.ExecuteScalar(sql, param, transaction, commandTimeout, commandType), sql, param);
    #endregion

    #region ExecuteScalarAsync
    public Task<object?> ExecuteScalarAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLoggingAsync(() => _inner.ExecuteScalarAsync(sql, param, transaction, commandTimeout, commandType), sql, param);
    #endregion

    #region QueryMultiple
    public SqlMapper.GridReader QueryMultiple(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLogging(() => _inner.QueryMultiple(sql, param, transaction, commandTimeout, commandType), sql, param);
    #endregion

    #region QueryMultipleAsync
    public Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => ExecuteWithLoggingAsync(() => _inner.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType), sql, param);
    #endregion






    #endregion

    private T ExecuteWithLogging<T>(Func<T> func, string sql, object? param)
    {
        _counter.Increment();

        var stopwatch = Stopwatch.StartNew();
        Exception? error = null;
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            error = ex;
            throw;
        }
        finally
        {
            stopwatch.Stop();
            LogQuery(sql, param, stopwatch.ElapsedMilliseconds, error == null, error?.Message);
        }
    }

    private async Task<T> ExecuteWithLoggingAsync<T>(Func<Task<T>> func, string sql, object? param)
    {
        _counter.Increment();

        var stopwatch = Stopwatch.StartNew();
        Exception? error = null;
        try
        {
            return await func();
        }
        catch (Exception ex)
        {
            error = ex;
            throw;
        }
        finally
        {
            stopwatch.Stop();
            LogQuery(sql, param, stopwatch.ElapsedMilliseconds, error == null, error?.Message);
        }
    }

    private void LogQuery(string sql, object param, long durationMs, bool success, string exceptionMessage)
    {
        if (!_settings.Enabled.GetValueOrDefault()) return;
        if (_settings.CaptureOptions?.Any() != true || _settings.CaptureOptions.Contains(CaptureOptions.None)) return;

        var context = _httpContextAccessor.HttpContext;

        if (context is null) return;

        var capture = _settings.CaptureOptions;

        var entry = new DbQueryEntry
        {
            Provider = DbProviderExtractor.GetFriendlyProviderName(_inner),
            ExecutedAt = DateTime.Now,
            DurationMs = (int)durationMs,
            Success = success,
            ORM = $"Dapper {OrmVersionCache.DapperVersionCached}"
        };

        if (capture.Contains(CaptureOptions.Query))
            entry.Sql = sql;

        if (capture.Contains(CaptureOptions.Parameters))
            entry.Parameters = param is null ? string.Empty : JsonExtensions.Serialize(DynamicParametersExtensions.GetDeclaredParameters(param));

        if (capture.Contains(CaptureOptions.ExceptionMessage))
            entry.ExceptionMessage = exceptionMessage;

        if (capture.Contains(CaptureOptions.Context))
            entry.DbContext = DbContextMetadataExtractor.Extract(_inner);

        QueryLogHelper.AddQueryLog(context, entry);
    }
}

