using Dapper;
using Microsoft.Extensions.Logging;
using NedMonitor.Dapper.Interceptors;
using System.Data;

namespace NedMonitor.Dapper.Wrappers;

internal class NedDapperWrapper : INedDapperWrapper
{
    private readonly ILogger<NedDapperWrapper> _logger;
    public NedDapperWrapper(ILogger<NedDapperWrapper> logger) => _logger = logger;

    #region ExecuteAsync
    public Task<int> ExecuteAsync(IDbConnection conn, string sql, object? param = null, IDbTransaction? trans = null, int? timeout = null, CommandType? type = null) =>
        UseConnAsync(conn, c => c.ExecuteAsync(sql, param, trans, timeout, type), () => conn.ExecuteAsync(sql, param, trans, timeout, type));
    public int Execute(IDbConnection conn, string sql, object? param = null, IDbTransaction? trans = null, int? timeout = null, CommandType? type = null) =>
        UseConn(conn, c => c.Execute(sql, param, trans, timeout, type), () => conn.Execute(sql, param, trans, timeout, type));

    public int Execute(IDbConnection conn, CommandDefinition command) =>
        UseConn(conn, c => c.Execute(command), () => conn.Execute(command));

    public Task<int> ExecuteAsync(IDbConnection conn, CommandDefinition command) =>
        UseConnAsync(conn, c => c.ExecuteAsync(command), () => conn.ExecuteAsync(command));

    #endregion

    #region ExecuteReaderAsync
    public Task<IDataReader> ExecuteReaderAsync(IDbConnection conn, CommandDefinition command)
        => UseConnAsync(conn, c => c.ExecuteReaderAsync(command), () => conn.ExecuteReaderAsync(command));
    public Task<IDataReader> ExecuteReaderAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.ExecuteReaderAsync(sql, param, transaction, commandTimeout, commandType), () => cnn.ExecuteReaderAsync(sql, param, transaction, commandTimeout, commandType));
    public Task<IDataReader> ExecuteReaderAsync(IDbConnection cnn, CommandDefinition command, CommandBehavior commandBehavior)
        => UseConnAsync(cnn, c => c.ExecuteReaderAsync(command, commandBehavior), () => cnn.ExecuteReaderAsync(command, commandBehavior));
    #endregion

    #region MyRegion
    public IDataReader ExecuteReader(IDbConnection conn, CommandDefinition command)
        => UseConn(conn, c => c.ExecuteReader(command), () => conn.ExecuteReader(command));
    public IDataReader ExecuteReader(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
       => UseConn(cnn, c => c.ExecuteReader(sql, param, transaction, commandTimeout, commandType), () => cnn.ExecuteReader(sql, param, transaction, commandTimeout, commandType));
    public IDataReader ExecuteReader(IDbConnection cnn, CommandDefinition command, CommandBehavior commandBehavior)
        => UseConn(cnn, c => c.ExecuteReader(command, commandBehavior), () => cnn.ExecuteReader(command, commandBehavior));
    #endregion

    #region ExecuteScalarAsync

    public Task<object?> ExecuteScalarAsync(IDbConnection conn, CommandDefinition command)
        => UseConnAsync(conn, c => c.ExecuteScalarAsync(command), () => conn.ExecuteScalarAsync(command));
    public Task<T?> ExecuteScalarAsync<T>(IDbConnection conn, CommandDefinition command)
        => UseConnAsync(conn, c => c.ExecuteScalarAsync<T>(command), () => conn.ExecuteScalarAsync<T>(command));
    public Task<object?> ExecuteScalarAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.ExecuteScalarAsync(sql, param, transaction, commandTimeout, commandType), () => cnn.ExecuteScalarAsync(sql, param, transaction, commandTimeout, commandType));
    public Task<T?> ExecuteScalarAsync<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType), () => cnn.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType));

    #endregion

    #region ExecuteScalar
    public object? ExecuteScalar(IDbConnection conn, string sql, object? param = null, IDbTransaction? trans = null, int? timeout = null, CommandType? type = null)
        => UseConn(conn, c => c.ExecuteScalar(sql, param, trans, timeout, type), () => conn.ExecuteScalar(sql, param, trans, timeout, type));

    public T? ExecuteScalar<T>(IDbConnection conn, string sql, object? param = null, IDbTransaction? trans = null, int? timeout = null, CommandType? type = null)
        => UseConn(conn, c => c.ExecuteScalar<T>(sql, param, trans, timeout, type), () => conn.ExecuteScalar<T>(sql, param, trans, timeout, type));
    public object? ExecuteScalar(IDbConnection conn, CommandDefinition command)
        => UseConn(conn, c => c.ExecuteScalar(command), () => conn.ExecuteScalar(command));
    public T? ExecuteScalar<T>(IDbConnection conn, CommandDefinition command)
        => UseConn(conn, c => c.ExecuteScalar<T>(command), () => conn.ExecuteScalar<T>(command));

    #endregion

    #region QueryFirstAsync

    public Task<T> QueryFirstAsync<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.QueryFirstAsync<T>(sql, param, transaction, commandTimeout, commandType), () => cnn.QueryFirstAsync<T>(sql, param, transaction, commandTimeout, commandType));
    public Task<dynamic> QueryFirstAsync(IDbConnection cnn, CommandDefinition command)
        => UseConnAsync(cnn, c => c.QueryFirstAsync(command), () => cnn.QueryFirstAsync(command));
    public Task<T> QueryFirstAsync<T>(IDbConnection cnn, CommandDefinition command)
        => UseConnAsync(cnn, c => c.QueryFirstAsync<T>(command), () => cnn.QueryFirstAsync<T>(command));
    public Task<object> QueryFirstAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.QueryFirstAsync(type, sql, param, transaction, commandTimeout, commandType), () => cnn.QueryFirstAsync(type, sql, param, transaction, commandTimeout, commandType));
    public Task<object> QueryFirstAsync(IDbConnection cnn, Type type, CommandDefinition command)
        => UseConnAsync(cnn, c => c.QueryFirstAsync(type, command), () => cnn.QueryFirstAsync(type, command));


    #endregion

    #region QueryFirst
    public dynamic QueryFirst(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.QueryFirst(sql, param, transaction, commandTimeout, commandType), () => cnn.QueryFirst(sql, param, transaction, commandTimeout, commandType));
    public T QueryFirst<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.QueryFirst<T>(sql, param, transaction, commandTimeout, commandType), () => cnn.QueryFirst<T>(sql, param, transaction, commandTimeout, commandType));
    public object QueryFirst(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.QueryFirst(type, sql, param, transaction, commandTimeout, commandType), () => cnn.QueryFirst(type, sql, param, transaction, commandTimeout, commandType));
    public T QueryFirst<T>(IDbConnection cnn, CommandDefinition command)
        => UseConn(cnn, c => c.QueryFirst<T>(command), () => cnn.QueryFirst<T>(command));
    #endregion

    #region QueryFirstOrDefaultAsync

    public Task<T?> QueryFirstOrDefaultAsync<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType), () => cnn.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType));
    public Task<dynamic?> QueryFirstOrDefaultAsync(IDbConnection cnn, CommandDefinition command)
        => UseConnAsync(cnn, c => c.QueryFirstOrDefaultAsync(command), () => cnn.QueryFirstOrDefaultAsync(command));
    public Task<T?> QueryFirstOrDefaultAsync<T>(IDbConnection cnn, CommandDefinition command)
        => UseConnAsync(cnn, c => c.QueryFirstOrDefaultAsync<T>(command), () => cnn.QueryFirstOrDefaultAsync<T>(command));
    public Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection cnn, CommandDefinition command)
        => UseConnAsync(cnn, c => c.QuerySingleOrDefaultAsync<T>(command), () => cnn.QuerySingleOrDefaultAsync<T>(command));
    public Task<object?> QueryFirstOrDefaultAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.QueryFirstOrDefaultAsync(type, sql, param, transaction, commandTimeout, commandType), () => cnn.QueryFirstOrDefaultAsync(type, sql, param, transaction, commandTimeout, commandType));
    public Task<object?> QueryFirstOrDefaultAsync(IDbConnection cnn, Type type, CommandDefinition command)
        => UseConnAsync(cnn, c => c.QueryFirstOrDefaultAsync(type, command), () => cnn.QueryFirstOrDefaultAsync(type, command));

    #endregion

    #region QueryFirstOrDefault
    public dynamic? QueryFirstOrDefault(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.QueryFirstOrDefault(sql, param, transaction, commandTimeout, commandType), () => cnn.QueryFirstOrDefault(sql, param, transaction, commandTimeout, commandType));
    public T? QueryFirstOrDefault<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType), () => cnn.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType));
    public object? QueryFirstOrDefault(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.QueryFirstOrDefault(type, sql, param, transaction, commandTimeout, commandType), () => cnn.QueryFirstOrDefault(type, sql, param, transaction, commandTimeout, commandType));
    public T? QueryFirstOrDefault<T>(IDbConnection cnn, CommandDefinition command)
        => UseConn(cnn, c => c.QueryFirstOrDefault<T>(command), () => cnn.QueryFirstOrDefault<T>(command));
    #endregion

    #region QuerySingleOrDefaultAsync

    public Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection conn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(conn, c => c.QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType), () => conn.QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType));
    public Task<dynamic?> QuerySingleOrDefaultAsync(IDbConnection conn, CommandDefinition command)
        => UseConnAsync(conn, c => c.QuerySingleOrDefaultAsync(command), () => conn.QuerySingleOrDefaultAsync(command));
    public Task<object?> QuerySingleOrDefaultAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.QuerySingleOrDefaultAsync(type, sql, param, transaction, commandTimeout, commandType), () => cnn.QuerySingleOrDefaultAsync(type, sql, param, transaction, commandTimeout, commandType));
    public Task<object?> QuerySingleOrDefaultAsync(IDbConnection cnn, Type type, CommandDefinition command)
        => UseConnAsync(cnn, c => c.QuerySingleOrDefaultAsync(type, command), () => cnn.QuerySingleOrDefaultAsync(type, command));

    #endregion

    #region QuerySingleOrDefault
    public dynamic? QuerySingleOrDefault(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.QuerySingleOrDefault(sql, param, transaction, commandTimeout, commandType), () => cnn.QuerySingleOrDefault(sql, param, transaction, commandTimeout, commandType));
    public T? QuerySingleOrDefault<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.QuerySingleOrDefault<T>(sql, param, transaction, commandTimeout, commandType), () => cnn.QuerySingleOrDefault<T>(sql, param, transaction, commandTimeout, commandType));
    public object? QuerySingleOrDefault(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.QuerySingleOrDefault(type, sql, param, transaction, commandTimeout, commandType), () => cnn.QuerySingleOrDefault(type, sql, param, transaction, commandTimeout, commandType));
    public T? QuerySingleOrDefault<T>(IDbConnection cnn, CommandDefinition command)
        => UseConn(cnn, c => c.QuerySingleOrDefault<T>(command), () => cnn.QuerySingleOrDefault<T>(command));



    #endregion

    #region QuerySingleAsync
    public Task<T> QuerySingleAsync<T>(IDbConnection cnn, CommandDefinition command)
        => UseConnAsync(cnn, c => c.QuerySingleAsync<T>(command), () => cnn.QuerySingleAsync<T>(command));
    public Task<T> QuerySingleAsync<T>(IDbConnection conn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(conn, c => c.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, commandType), () => conn.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, commandType));
    public Task<dynamic> QuerySingleAsync(IDbConnection conn, CommandDefinition command)
        => UseConnAsync(conn, c => c.QuerySingleAsync(command), () => conn.QuerySingleAsync(command));
    public Task<object> QuerySingleAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.QuerySingleAsync(type, sql, param, transaction, commandTimeout, commandType), () => cnn.QuerySingleAsync(type, sql, param, transaction, commandTimeout, commandType));
    public Task<object> QuerySingleAsync(IDbConnection cnn, Type type, CommandDefinition command)
        => UseConnAsync(cnn, c => c.QuerySingleAsync(type, command), () => cnn.QuerySingleAsync(type, command));

    #endregion

    #region QuerySingle
    public dynamic QuerySingle(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.QuerySingle(sql, param, transaction, commandTimeout, commandType), () => cnn.QuerySingle(sql, param, transaction, commandTimeout, commandType));
    public T QuerySingle<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.QuerySingle<T>(sql, param, transaction, commandTimeout, commandType), () => cnn.QuerySingle<T>(sql, param, transaction, commandTimeout, commandType));
    public object QuerySingle(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.QuerySingle(type, sql, param, transaction, commandTimeout, commandType), () => cnn.QuerySingle(type, sql, param, transaction, commandTimeout, commandType));
    public T QuerySingle<T>(IDbConnection cnn, CommandDefinition command)
        => UseConn(cnn, c => c.QuerySingle<T>(command), () => cnn.QuerySingle<T>(command));
    #endregion

    #region QueryAsync
    public Task<IEnumerable<T>> QueryAsync<T>(IDbConnection conn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(conn, c => c.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType), () => conn.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType));
    public Task<IEnumerable<object>> QueryAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.QueryAsync(type, sql, param, transaction, commandTimeout, commandType), () => cnn.QueryAsync(type, sql, param, transaction, commandTimeout, commandType));
    public Task<IEnumerable<dynamic>> QueryAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.QueryAsync(sql, param, transaction, commandTimeout, commandType), () => cnn.QueryAsync(sql, param, transaction, commandTimeout, commandType));
    public Task<IEnumerable<dynamic>> QueryAsync(IDbConnection conn, CommandDefinition command)
        => UseConnAsync(conn, c => c.QueryAsync(command), () => conn.QueryAsync(command));
    public Task<IEnumerable<T>> QueryAsync<T>(IDbConnection cnn, CommandDefinition command)
        => UseConnAsync(cnn, c => c.QueryAsync<T>(command), () => cnn.QueryAsync<T>(command));
    public Task<IEnumerable<object>> QueryAsync(IDbConnection cnn, Type type, CommandDefinition command)
        => UseConnAsync(cnn, c => c.QueryAsync(type, command), () => cnn.QueryAsync(type, command));
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), () => cnn.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), () => cnn.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), () => cnn.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), () => cnn.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), () => cnn.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), () => cnn.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));

    #endregion

    #region Query

    public IEnumerable<dynamic> Query(IDbConnection conn, string sql, object? param = null, IDbTransaction? trans = null, bool buffered = true, int? timeout = null, CommandType? type = null)
        => UseConn(conn, c => c.Query(sql, param, trans, buffered, timeout, type), () => conn.Query(sql, param, trans, buffered, timeout, type));
    public IEnumerable<T> Query<T>(IDbConnection conn, string sql, object? param = null, IDbTransaction? trans = null, bool buffered = true, int? timeout = null, CommandType? type = null)
        => UseConn(conn, c => c.Query<T>(sql, param, trans, buffered, timeout, type), () => conn.Query<T>(sql, param, trans, buffered, timeout, type));
    public IEnumerable<object> Query(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.Query(type, sql, param, transaction, buffered, commandTimeout, commandType), () => cnn.Query(type, sql, param, transaction, buffered, commandTimeout, commandType));
    public IEnumerable<T> Query<T>(IDbConnection cnn, CommandDefinition command)
        => UseConn(cnn, c => c.Query<T>(command), () => cnn.Query<T>(command));
    public IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), () => cnn.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), () => cnn.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), () => cnn.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), () => cnn.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), () => cnn.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), () => cnn.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
    public IEnumerable<TReturn> Query<TReturn>(IDbConnection cnn, string sql, Type[] types, Func<object[], TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.Query(sql, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType), () => cnn.Query(sql, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType));

    #endregion

    #region QueryMultipleAsync

    public Task<SqlMapper.GridReader> QueryMultipleAsync(IDbConnection conn, CommandDefinition command)
        => UseConnAsync(conn, c => c.QueryMultipleAsync(command), () => conn.QueryMultipleAsync(command));

    public Task<SqlMapper.GridReader> QueryMultipleAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConnAsync(cnn, c => c.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType), () => cnn.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType));

    #endregion

    #region QueryMultiple
    public SqlMapper.GridReader QueryMultiple(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => UseConn(cnn, c => c.QueryMultiple(sql, param, transaction, commandTimeout, commandType), () => cnn.QueryMultiple(sql, param, transaction, commandTimeout, commandType));
    public SqlMapper.GridReader QueryMultiple(IDbConnection cnn, CommandDefinition command)
        => UseConn(cnn, c => c.QueryMultiple(command), () => cnn.QueryMultiple(command));
    #endregion

    #region private
    private void LogUnmonitored() => _logger.LogWarning("Query not monitored by NedMonitor (connection is not CountingDbConnection)");

    private T UseConn<T>(IDbConnection conn, Func<CountingDbConnection, T> useCounting, Func<T> useDapper)
    {
        ArgumentNullException.ThrowIfNull(conn, nameof(conn));
        return conn is CountingDbConnection cd ? useCounting(cd) : LogUnmonitoredAnd(useDapper);
    }

    private Task<T> UseConnAsync<T>(IDbConnection conn, Func<CountingDbConnection, Task<T>> useCounting, Func<Task<T>> useDapper)
    {
        ArgumentNullException.ThrowIfNull(conn, nameof(conn));
        return conn is CountingDbConnection cd ? useCounting(cd) : LogUnmonitoredAnd(useDapper);
    }

    private T LogUnmonitoredAnd<T>(Func<T> fallback)
    {
        LogUnmonitored();
        return fallback();
    }

    private Task<T> LogUnmonitoredAnd<T>(Func<Task<T>> fallback)
    {
        LogUnmonitored();
        return fallback();
    }

    #endregion
}
