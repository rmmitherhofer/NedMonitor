using Dapper;
using System.Data;
using System.Data.Common;
using static Dapper.SqlMapper;

namespace Solucao.RH.Customers.Data.TesteDapper;

public interface INedDapperWrapper
{


    public dynamic QueryFirst(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public dynamic? QueryFirstOrDefault(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public dynamic QuerySingle(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public dynamic? QuerySingleOrDefault(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public IEnumerable<T> Query<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
    public T QueryFirst<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);

    public T? QueryFirstOrDefault<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public T QuerySingle<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public T? QuerySingleOrDefault<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public IEnumerable<object> Query(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
    public object QueryFirst(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public object? QueryFirstOrDefault(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public object QuerySingle(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public object? QuerySingleOrDefault(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public IEnumerable<T> Query<T>(IDbConnection cnn, CommandDefinition command);
    public T QueryFirst<T>(IDbConnection cnn, CommandDefinition command);
    public T? QueryFirstOrDefault<T>(IDbConnection cnn, CommandDefinition command);
    public T QuerySingle<T>(IDbConnection cnn, CommandDefinition command);
    public T? QuerySingleOrDefault<T>(IDbConnection cnn, CommandDefinition command);
    public GridReader QueryMultiple(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public GridReader QueryMultiple(IDbConnection cnn, CommandDefinition command);


    #region QueryAsync (dynamic)

    Task<IEnumerable<dynamic>> QueryAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<IEnumerable<dynamic>> QueryAsync(IDbConnection cnn, CommandDefinition command);

    #endregion

    #region Query (dynamic)
    public IEnumerable<dynamic> Query(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);

    #endregion

    #region QueryAsync<T>

    Task<IEnumerable<T>> QueryAsync<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<IEnumerable<T>> QueryAsync<T>(IDbConnection cnn, CommandDefinition command);

    #endregion

    #region QueryAsync (Type)

    Task<IEnumerable<object>> QueryAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<IEnumerable<object>> QueryAsync(IDbConnection cnn, Type type, CommandDefinition command);

    #endregion

    #region Query (First/FirstOrDefault/Single/SingleOrDefault)

    Task<T> QueryFirstAsync<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<T?> QueryFirstOrDefaultAsync<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<T> QueryFirstAsync<T>(IDbConnection cnn, CommandDefinition command);
    Task<T?> QueryFirstOrDefaultAsync<T>(IDbConnection cnn, CommandDefinition command);

    Task<T> QuerySingleAsync<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<T> QuerySingleAsync<T>(IDbConnection cnn, CommandDefinition command);
    Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection cnn, CommandDefinition command);

    #endregion

    #region Query (Type version)

    Task<object> QueryFirstAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<object?> QueryFirstOrDefaultAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<object> QueryFirstAsync(IDbConnection cnn, Type type, CommandDefinition command);
    Task<object?> QueryFirstOrDefaultAsync(IDbConnection cnn, Type type, CommandDefinition command);

    Task<object> QuerySingleAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<object?> QuerySingleOrDefaultAsync(IDbConnection cnn, Type type, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<object> QuerySingleAsync(IDbConnection cnn, Type type, CommandDefinition command);
    Task<object?> QuerySingleOrDefaultAsync(IDbConnection cnn, Type type, CommandDefinition command);

    #endregion

    #region Query (dynamic CommandDefinition)

    Task<dynamic> QueryFirstAsync(IDbConnection cnn, CommandDefinition command);
    Task<dynamic?> QueryFirstOrDefaultAsync(IDbConnection cnn, CommandDefinition command);
    Task<dynamic> QuerySingleAsync(IDbConnection cnn, CommandDefinition command);
    Task<dynamic?> QuerySingleOrDefaultAsync(IDbConnection cnn, CommandDefinition command);

    #endregion

    #region QueryMultipleAsync

    Task<GridReader> QueryMultipleAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<GridReader> QueryMultipleAsync(IDbConnection cnn, CommandDefinition command);

    #endregion

    #region QueryAsync with Mapping
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);



    #endregion

    #region Query with Mapping
    public IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string? splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string? splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string? splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string? splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string? splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
    public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string? splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
    public IEnumerable<TReturn> Query<TReturn>(IDbConnection cnn, string sql, Type[] types, Func<object[], TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string? splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

    #endregion

    #region ExecuteAsync

    Task<int> ExecuteAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<int> ExecuteAsync(IDbConnection cnn, CommandDefinition command);

    #endregion

    #region Execute
    public int Execute(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public int Execute(IDbConnection cnn, CommandDefinition command);

    #endregion

    #region ExecuteReaderAsync

    Task<IDataReader> ExecuteReaderAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<DbDataReader> ExecuteReaderAsync(DbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<IDataReader> ExecuteReaderAsync(IDbConnection cnn, CommandDefinition command);
    Task<DbDataReader> ExecuteReaderAsync(DbConnection cnn, CommandDefinition command);
    Task<IDataReader> ExecuteReaderAsync(IDbConnection cnn, CommandDefinition command, CommandBehavior commandBehavior);
    Task<DbDataReader> ExecuteReaderAsync(DbConnection cnn, CommandDefinition command, CommandBehavior commandBehavior);

    #endregion

    #region ExecuteReader
    public IDataReader ExecuteReader(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public IDataReader ExecuteReader(IDbConnection cnn, CommandDefinition command);
    public IDataReader ExecuteReader(IDbConnection cnn, CommandDefinition command, CommandBehavior commandBehavior);
    #endregion

    #region ExecuteScalarAsync

    Task<object?> ExecuteScalarAsync(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<T?> ExecuteScalarAsync<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    Task<object?> ExecuteScalarAsync(IDbConnection cnn, CommandDefinition command);
    Task<T?> ExecuteScalarAsync<T>(IDbConnection cnn, CommandDefinition command);

    #endregion

    #region ExecuteScalar
    public object? ExecuteScalar(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public T? ExecuteScalar<T>(IDbConnection cnn, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    public object? ExecuteScalar(IDbConnection cnn, CommandDefinition command);
    public T? ExecuteScalar<T>(IDbConnection cnn, CommandDefinition command);

    #endregion

    #region QueryUnbufferedAsync

    IAsyncEnumerable<dynamic> QueryUnbufferedAsync(DbConnection cnn, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    IAsyncEnumerable<T> QueryUnbufferedAsync<T>(DbConnection cnn, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);

    #endregion

    #region QueryUnbuffered

    #endregion

}
