using FluentAssertions;
using Microsoft.Extensions.Logging;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Enums;
using NedMonitor.Core.Extensions;
using NedMonitor.Core.Tests.Extensions.Fixtures;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Extensions;

public class LogAttentionMapperTests(ITestOutputHelper output, LogAttentionMapperTestsFixture fixture)
    : Test(output), IClassFixture<LogAttentionMapperTestsFixture>
{
    [Fact(DisplayName =
        "Given information log level, " +
        "When mapping attention, " +
        "Then it returns Low")]
    [Trait("Extensions", nameof(LogAttentionMapper))]
    public async Task Map_FromInformation_ReturnsLow()
    {
        //Given
        var logLevel = LogLevel.Information;

        //When
        var attention = logLevel.Map();

        //Then
        attention.Should().Be(LogAttentionLevel.Low);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given warning log level, " +
        "When mapping attention, " +
        "Then it returns Medium")]
    [Trait("Extensions", nameof(LogAttentionMapper))]
    public async Task Map_FromWarning_ReturnsMedium()
    {
        //Given
        var logLevel = LogLevel.Warning;

        //When
        var attention = logLevel.Map();

        //Then
        attention.Should().Be(LogAttentionLevel.Medium);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given error log level, " +
        "When mapping attention, " +
        "Then it returns High")]
    [Trait("Extensions", nameof(LogAttentionMapper))]
    public async Task Map_FromError_ReturnsHigh()
    {
        //Given
        var logLevel = LogLevel.Error;

        //When
        var attention = logLevel.Map();

        //Then
        attention.Should().Be(LogAttentionLevel.High);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given debug log level, " +
        "When mapping attention, " +
        "Then it returns None")]
    [Trait("Extensions", nameof(LogAttentionMapper))]
    public async Task Map_FromDebug_ReturnsNone()
    {
        //Given
        var logLevel = LogLevel.Debug;

        //When
        var attention = logLevel.Map();

        //Then
        attention.Should().Be(LogAttentionLevel.None);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given trace log level, " +
        "When mapping attention, " +
        "Then it returns None")]
    [Trait("Extensions", nameof(LogAttentionMapper))]
    public async Task Map_FromTrace_ReturnsNone()
    {
        //Given
        var logLevel = LogLevel.Trace;

        //When
        var attention = logLevel.Map();

        //Then
        attention.Should().Be(LogAttentionLevel.None);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given critical log level, " +
        "When mapping attention, " +
        "Then it returns Critical")]
    [Trait("Extensions", nameof(LogAttentionMapper))]
    public async Task Map_FromCritical_ReturnsCritical()
    {
        //Given
        var logLevel = LogLevel.Critical;

        //When
        var attention = logLevel.Map();

        //Then
        attention.Should().Be(LogAttentionLevel.Critical);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given unknown log level, " +
        "When mapping attention, " +
        "Then it returns None")]
    [Trait("Extensions", nameof(LogAttentionMapper))]
    public async Task Map_FromUnknown_ReturnsNone()
    {
        //Given
        var logLevel = (LogLevel)999;

        //When
        var attention = logLevel.Map();

        //Then
        attention.Should().Be(LogAttentionLevel.None);
        await Task.CompletedTask;
    }
}
