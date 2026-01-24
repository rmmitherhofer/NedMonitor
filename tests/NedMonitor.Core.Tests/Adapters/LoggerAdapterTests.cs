using FluentAssertions;
using Microsoft.Extensions.Logging;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Adapters;
using NedMonitor.Core.Settings;
using NedMonitor.Core.Tests.Adapters.Fixtures;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Adapters;

public class LoggerAdapterTests(ITestOutputHelper output, LoggerAdapterTestsFixture fixture)
    : Test(output), IClassFixture<LoggerAdapterTestsFixture>
{
    private readonly LoggerAdapterTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given a minimum log level, " +
        "When checking IsEnabled, " +
        "Then it respects the configured minimum")]
    [Trait("Adapters", nameof(LoggerAdapter))]
    public async Task IsEnabled_RespectsMinimumLogLevel()
    {
        //Given
        var logger = _fixture.CreateLogger(LogLevel.Warning, new FormatterOptions());

        //When
        var enabled = logger.IsEnabled(LogLevel.Information);

        //Then
        enabled.Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given enabled log level, " +
        "When logging, " +
        "Then it stores entry in HttpContext")]
    [Trait("Adapters", nameof(LoggerAdapter))]
    public async Task Log_Enabled_AddsEntryToContext()
    {
        //Given
        var logger = _fixture.CreateLogger(LogLevel.Information, new FormatterOptions());
        var exception = new Exception("boom");

        //When
        logger.Log(LogLevel.Information, new EventId(1), "state", exception, (state, ex) => state.ToString());

        //Then
        var logs = LoggerAdapter.GetLogsForCurrentRequest(_fixture.Context).ToList();
        logs.Should().HaveCount(1);
        logs[0].Message.Should().Be("state");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given enabled log level, " +
        "When logging, " +
        "Then it populates member info")]
    [Trait("Adapters", nameof(LoggerAdapter))]
    public async Task Log_Enabled_PopulatesMemberInfo()
    {
        //Given
        var logger = _fixture.CreateLogger(LogLevel.Information, new FormatterOptions());

        //When
        logger.Log(LogLevel.Information, new EventId(4), "state", null, (state, ex) => state.ToString());

        //Then
        var logs = LoggerAdapter.GetLogsForCurrentRequest(_fixture.Context).ToList();
        logs.Should().HaveCount(1);
        logs[0].MemberName.Should().NotBeNullOrWhiteSpace();
        logs[0].MemberType.Should().NotBeNullOrWhiteSpace();
        logs[0].LineNumber.Should().BeGreaterThan(0);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given custom formatter, " +
        "When logging, " +
        "Then it uses the formatted message")]
    [Trait("Adapters", nameof(LoggerAdapter))]
    public async Task Log_WithFormatter_UsesCustomMessage()
    {
        //Given
        var options = new FormatterOptions
        {
            Formatter = _ => "custom"
        };
        var logger = _fixture.CreateLogger(LogLevel.Information, options);
        var exception = new Exception("boom");

        //When
        logger.Log(LogLevel.Information, new EventId(2), "state", exception, (state, ex) => "default");

        //Then
        var logs = LoggerAdapter.GetLogsForCurrentRequest(_fixture.Context).ToList();
        logs.Should().HaveCount(1);
        logs[0].Message.Should().Be("custom");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given null HttpContext, " +
        "When logging, " +
        "Then it does not add entries")]
    [Trait("Adapters", nameof(LoggerAdapter))]
    public async Task Log_WhenContextIsNull_DoesNotAddEntries()
    {
        //Given
        var logger = _fixture.CreateLoggerWithNullContext(LogLevel.Information, new FormatterOptions());
        var exception = new Exception("boom");
        _fixture.Context.Items.Clear();

        //When
        logger.Log(LogLevel.Information, new EventId(3), "state", exception, (state, ex) => "message");

        //Then
        var logs = LoggerAdapter.GetLogsForCurrentRequest(_fixture.Context).ToList();
        logs.Should().BeEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given no logs in context, " +
        "When retrieving logs, " +
        "Then it returns empty list")]
    [Trait("Adapters", nameof(LoggerAdapter))]
    public async Task GetLogsForCurrentRequest_WhenEmpty_ReturnsEmpty()
    {
        //Given
        _fixture.Context.Items.Clear();

        //When
        var logs = LoggerAdapter.GetLogsForCurrentRequest(_fixture.Context).ToList();

        //Then
        logs.Should().BeEmpty();
        await Task.CompletedTask;
    }
}
