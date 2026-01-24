using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Common.Tests.FakerFactory.Models;
using NedMonitor.Core.Models;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Models;

public class ExceptionInfoTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given exception info, " +
        "When setting properties, " +
        "Then it stores values")]
    [Trait("Models", nameof(ExceptionInfo))]
    public async Task Properties_SetAndGet()
    {
        //Given
        var info = ExceptionInfoFaker.Create(
            type: "System.Exception",
            message: "boom",
            stackTrace: "stack",
            innerException: "inner",
            source: "source");

        //When
        var type = info.Type;

        //Then
        type.Should().Be("System.Exception");
        info.Message.Should().Be("boom");
        info.StackTrace.Should().Be("stack");
        info.InnerException.Should().Be("inner");
        info.Source.Should().Be("source");
        await Task.CompletedTask;
    }
}
