using System.Data;
using System.Data.Common;

namespace NedMonitor.Core.Tests.Helpers.Fixtures
{
    public sealed class DbProviderExtractorTestsFixture
    {
        public DbConnection CreateSqlServerConnection() => new Fake.SqlClient.FakeSqlConnection();
        public DbConnection CreateNpgsqlConnection() => new Npgsql.FakeNpgsqlConnection();
        public DbConnection CreateOracleConnection() => new Oracle.ManagedDataAccess.Client.FakeOracleConnection();
    }
}

namespace Fake.SqlClient
{
    public sealed class FakeSqlConnection : DbConnection
    {
        public override string ConnectionString { get; set; } = string.Empty;
        public override string Database => "Db";
        public override string DataSource => "Source";
        public override string ServerVersion => "1.0";
        public override ConnectionState State => ConnectionState.Closed;
        public override void ChangeDatabase(string databaseName) { }
        public override void Close() { }
        public override void Open() { }
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();
        protected override DbCommand CreateDbCommand() => throw new NotSupportedException();
    }
}

namespace Npgsql
{
    public sealed class FakeNpgsqlConnection : DbConnection
    {
        public override string ConnectionString { get; set; } = string.Empty;
        public override string Database => "Db";
        public override string DataSource => "Source";
        public override string ServerVersion => "1.0";
        public override ConnectionState State => ConnectionState.Closed;
        public override void ChangeDatabase(string databaseName) { }
        public override void Close() { }
        public override void Open() { }
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();
        protected override DbCommand CreateDbCommand() => throw new NotSupportedException();
    }
}

namespace Oracle.ManagedDataAccess.Client
{
    public sealed class FakeOracleConnection : DbConnection
    {
        public override string ConnectionString { get; set; } = string.Empty;
        public override string Database => "Db";
        public override string DataSource => "Source";
        public override string ServerVersion => "1.0";
        public override ConnectionState State => ConnectionState.Closed;
        public override void ChangeDatabase(string databaseName) { }
        public override void Close() { }
        public override void Open() { }
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();
        protected override DbCommand CreateDbCommand() => throw new NotSupportedException();
    }
}