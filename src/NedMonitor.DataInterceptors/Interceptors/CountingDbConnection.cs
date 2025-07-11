using Dapper;
using System.Data;

namespace NedMonitor.DataInterceptors.Interceptors;

/// <summary>
/// Wrapper para IDbConnection que intercepta chamadas Dapper para contagem de queries.
/// </summary>
public class CountingDbConnection : IDbConnection
{
    private readonly IDbConnection _innerConnection;
    private readonly DapperQueryCounter _counter;

    public CountingDbConnection(IDbConnection innerConnection, DapperQueryCounter counter)
    {
        _innerConnection = innerConnection ?? throw new ArgumentNullException(nameof(innerConnection));
        _counter = counter ?? throw new ArgumentNullException(nameof(counter));
    }

    public string ConnectionString
    {
        get => _innerConnection.ConnectionString;
        set => _innerConnection.ConnectionString = value;
    }

    public int ConnectionTimeout => _innerConnection.ConnectionTimeout;

    public string Database => _innerConnection.Database;

    public ConnectionState State => _innerConnection.State;

    public IDbTransaction BeginTransaction() => _innerConnection.BeginTransaction();

    public IDbTransaction BeginTransaction(IsolationLevel il) => _innerConnection.BeginTransaction(il);

    public void ChangeDatabase(string databaseName) => _innerConnection.ChangeDatabase(databaseName);

    public void Close() => _innerConnection.Close();

    public void Open() => _innerConnection.Open();

    public void Dispose() => _innerConnection.Dispose();

    public int Execute(string sql, object param = null, IDbTransaction transaction = null,
        int? commandTimeout = null, CommandType? commandType = null)
    {
        _counter.Increment();
        return _innerConnection.Execute(sql, param, transaction, commandTimeout, commandType);
    }

    public IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null,
        bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        _counter.Increment();
        return _innerConnection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
    }


    #region Métodos não Dapper (delegar para _innerConnection)

    public IDbCommand CreateCommand() => _innerConnection.CreateCommand();

    #endregion
}
