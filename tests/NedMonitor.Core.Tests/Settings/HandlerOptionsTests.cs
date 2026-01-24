using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Settings;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Settings;

public class HandlerOptionsTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given null handler, " +
        "When appending exception details, " +
        "Then it keeps default handler")]
    [Trait("Settings", nameof(HandlerOptions))]
    public async Task AppendExceptionDetails_Null_KeepsDefault()
    {
        //Given
        var options = new HandlerOptions();
        var defaultResult = ReadHandler(options).Invoke(new Exception("x"));

        //When
        options.AppendExceptionDetails(null!);
        var after = ReadHandler(options).Invoke(new Exception("x"));

        //Then
        defaultResult.Should().BeNull();
        after.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given custom handler, " +
        "When appending exception details, " +
        "Then it uses the new handler")]
    [Trait("Settings", nameof(HandlerOptions))]
    public async Task AppendExceptionDetails_CustomHandler_Overrides()
    {
        //Given
        var options = new HandlerOptions();

        //When
        options.AppendExceptionDetails(ex => "custom");

        //Then
        ReadHandler(options).Invoke(new Exception("x")).Should().Be("custom");
        await Task.CompletedTask;
    }

    private static Func<Exception, string> ReadHandler(HandlerOptions options)
    {
        var handlersProp = typeof(HandlerOptions).GetProperty("Handlers",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        var handlers = handlersProp!.GetValue(options);
        var appendProp = handlers!.GetType().GetProperty("AppendExceptionDetails");
        return (Func<Exception, string>)appendProp!.GetValue(handlers)!;
    }
}
