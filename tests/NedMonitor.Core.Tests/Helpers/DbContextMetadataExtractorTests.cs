using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Helpers;
using NedMonitor.Core.Tests.Helpers.Fixtures;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Helpers;

public class DbContextMetadataExtractorTests(ITestOutputHelper output, DbContextMetadataExtractorTestsFixture fixture)
    : Test(output), IClassFixture<DbContextMetadataExtractorTestsFixture>
{
    private readonly DbContextMetadataExtractorTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given connection with User ID, " +
        "When extracting metadata, " +
        "Then it includes database, data source and user id")]
    [Trait("Helpers", nameof(DbContextMetadataExtractor))]
    public async Task Extract_WithUserId_ReturnsExpectedMetadata()
    {
        //Given
        var connection = _fixture.CreateConnection(
            "MainDb",
            "Server01",
            "User ID=alice;Password=secret;");

        //When
        var info = DbContextMetadataExtractor.Extract(connection);

        //Then
        info["Database"].Should().Be("MainDb");
        info["DataSource"].Should().Be("Server01");
        info["UserId"].Should().Be("alice");
        info.ContainsKey("ServiceName").Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given connection with User, " +
        "When extracting metadata, " +
        "Then it includes user id")]
    [Trait("Helpers", nameof(DbContextMetadataExtractor))]
    public async Task Extract_WithUser_ReturnsUserId()
    {
        //Given
        var connection = _fixture.CreateConnection(
            "MainDb",
            "Server01",
            "User=service;Password=secret;");

        //When
        var info = DbContextMetadataExtractor.Extract(connection);

        //Then
        info["UserId"].Should().Be("service");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given oracle connection with service name, " +
        "When extracting metadata, " +
        "Then it includes service name")]
    [Trait("Helpers", nameof(DbContextMetadataExtractor))]
    public async Task Extract_OracleConnection_IncludesServiceName()
    {
        //Given
        var connection = _fixture.CreateOracleConnection(
            "OracleDb",
            "OracleSource",
            "User ID=oracle;SERVICE_NAME=ORCL");

        //When
        var info = DbContextMetadataExtractor.Extract(connection);

        //Then
        info["ServiceName"].Should().Be("ORCL");
        await Task.CompletedTask;
    }
}
