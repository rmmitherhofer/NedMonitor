using System.Data;
using System.Data.Common;

namespace NedMonitor.Core.Tests.Helpers.Fixtures;

public sealed class DbContextMetadataExtractorTestsFixture
{
    public DbConnection CreateConnection(string database, string dataSource, string connectionString) =>
        new FakeDbConnection(database, dataSource, connectionString);

    public DbConnection CreateOracleConnection(string database, string dataSource, string connectionString) =>
        new OracleFakeDbConnection(database, dataSource, connectionString);

    private class FakeDbConnection : DbConnection
    {
        private readonly string _database;
        private readonly string _dataSource;
        private string _connectionString;

        public FakeDbConnection(string database, string dataSource, string connectionString)
        {
            _database = database;
            _dataSource = dataSource;
            _connectionString = connectionString;
        }

        public override string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = value;
        }

        public override string Database => _database;
        public override string DataSource => _dataSource;
        public override string ServerVersion => "1.0";
        public override ConnectionState State => ConnectionState.Closed;
        public override void ChangeDatabase(string databaseName) { }
        public override void Close() { }
        public override void Open() { }
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();
        protected override DbCommand CreateDbCommand() => throw new NotSupportedException();
    }

    private sealed class OracleFakeDbConnection : FakeDbConnection
    {
        public OracleFakeDbConnection(string database, string dataSource, string connectionString)
            : base(database, dataSource, connectionString)
        {
        }
    }
}
