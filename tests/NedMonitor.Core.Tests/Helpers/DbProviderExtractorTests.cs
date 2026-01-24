using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Helpers;
using NedMonitor.Core.Tests.Helpers.Fixtures;
using System.Data;
using System.Data.Common;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Helpers;

public class DbProviderExtractorTests(ITestOutputHelper output, DbProviderExtractorTestsFixture fixture) : Test(output), IClassFixture<DbProviderExtractorTestsFixture>
{
    private readonly DbProviderExtractorTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given a SQL Server connection, " +
        "When getting friendly provider name, " +
        "Then it returns SQL Server")]
    [Trait("Helpers", nameof(DbProviderExtractor))]
    public void GetFriendlyProviderName_FromSqlClient_ReturnsSqlServer()
    {
        //Given
        var connection = _fixture.CreateSqlServerConnection();

        //When
        var friendly = DbProviderExtractor.GetFriendlyProviderName(connection);

        //Then
        friendly.Should().Be("SQL Server");
    }

    [Fact(DisplayName =
        "Given a Npgsql connection, " +
        "When getting friendly provider name, " +
        "Then it returns PostgreSQL")]
    [Trait("Helpers", nameof(DbProviderExtractor))]
    public void GetFriendlyProviderName_FromNpgsql_ReturnsPostgreSql()
    {
        //Given
        var connection = _fixture.CreateNpgsqlConnection();

        //When
        var friendly = DbProviderExtractor.GetFriendlyProviderName(connection);

        //Then
        friendly.Should().Be("PostgreSQL");
    }

    [Fact(DisplayName =
        "Given an Oracle connection, " +
        "When getting friendly provider name, " +
        "Then it returns Oracle")]
    [Trait("Helpers", nameof(DbProviderExtractor))]
    public void GetFriendlyProviderName_FromOracle_ReturnsOracle()
    {
        //Given
        var connection = _fixture.CreateOracleConnection();

        //When
        var friendly = DbProviderExtractor.GetFriendlyProviderName(connection);

        //Then
        friendly.Should().Be("Oracle");
    }

    [Fact(DisplayName =
        "Given a null connection, " +
        "When getting provider name, " +
        "Then it returns Unknown")]
    [Trait("Helpers", nameof(DbProviderExtractor))]
    public void GetProviderName_Null_ReturnsUnknown()
    {
        //Given
        DbConnection? connection = null;

        //When
        var provider = DbProviderExtractor.GetProviderName(connection);

        //Then
        provider.Should().Be("Unknown");
    }

    [Fact(DisplayName =
        "Given an unknown provider connection, " +
        "When getting friendly provider name, " +
        "Then it returns the namespace")]
    [Trait("Helpers", nameof(DbProviderExtractor))]
    public void GetFriendlyProviderName_Unknown_ReturnsNamespace()
    {
        //Given
        var connection = new FakeUnknownConnection();

        //When
        var friendly = DbProviderExtractor.GetFriendlyProviderName(connection);

        //Then
        friendly.Should().Be(typeof(FakeUnknownConnection).Namespace);
    }


    private sealed class FakeUnknownConnection : DbConnection
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
