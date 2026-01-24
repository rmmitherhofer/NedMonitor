using FluentAssertions;
using Microsoft.Extensions.Logging;
using NedMonitor.Common.Tests;
using NedMonitor.Common.Tests.FakerFactory.Models;
using NedMonitor.Core.Models;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Models;

public class LogEntryTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given log entry data, " +
        "When creating log entry, " +
        "Then it stores values and sets timestamp")]
    [Trait("Models", nameof(LogEntry))]
    public async Task Constructor_SetsProperties()
    {
        //Given
        var start = DateTime.Now;
        var category = "Category";
        var level = LogLevel.Warning;
        var message = "message";

        //When
        var entry = LogEntryFaker.Create(category, level, message);
        var end = DateTime.Now;

        //Then
        entry.Category.Should().Be(category);
        entry.LogLevel.Should().Be(level);
        entry.Message.Should().Be(message);
        entry.DateTime.Should().BeOnOrAfter(start).And.BeOnOrBefore(end);
        await Task.CompletedTask;
    }
}
