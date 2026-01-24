using FluentAssertions;
using Microsoft.Extensions.Logging;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Adapters;
using NedMonitor.Providers;
using NedMonitor.Tests.Providers.Fixtures;
using Xunit.Abstractions;

namespace NedMonitor.Tests.Providers;

public class LoggerProviderTests(
    ITestOutputHelper output,
    LoggerProviderTestsFixture fixture)
    : Test(output), IClassFixture<LoggerProviderTestsFixture>
{
    private readonly LoggerProviderTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given logger provider, " +
        "When creating a logger, " +
        "Then it returns a LoggerAdapter instance")]
    [Trait("Providers", nameof(LoggerProvider))]
    public async Task CreateLogger_ReturnsLoggerAdapter()
    {
        //Given
        var provider = new LoggerProvider(
            _fixture.CreateFormatterOptions(),
            _fixture.CreateHttpContextAccessor(),
            _fixture.CreateOptions());

        //When
        var logger = provider.CreateLogger("category");

        //Then
        logger.Should().BeOfType<LoggerAdapter>();
        logger.Should().BeAssignableTo<ILogger>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given logger provider, " +
        "When disposing, " +
        "Then it does not throw")]
    [Trait("Providers", nameof(LoggerProvider))]
    public async Task Dispose_DoesNotThrow()
    {
        //Given
        var provider = new LoggerProvider(
            _fixture.CreateFormatterOptions(),
            _fixture.CreateHttpContextAccessor(),
            _fixture.CreateOptions());

        //When
        var act = () => provider.Dispose();

        //Then
        act.Should().NotThrow();
        await Task.CompletedTask;
    }
}
