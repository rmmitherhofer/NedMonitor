using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Formatters;
using System.Reflection;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Formatters;

public class FormatterArgsTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given state, exception and default value, " +
        "When creating formatter args, " +
        "Then it exposes the same values")]
    [Trait("Formatters", nameof(FormatterArgs))]
    public async Task CreateOptions_AssignsValues()
    {
        //Given
        var state = new object();
        var exception = new Exception("boom");
        var defaultValue = "default";
        var options = CreateOptions(state, exception, defaultValue);
        var ctor = GetConstructor();

        //When
        var args = (FormatterArgs)ctor.Invoke([options])!;

        //Then
        args.State.Should().BeSameAs(state);
        args.Exception.Should().BeSameAs(exception);
        args.DefaultValue.Should().Be(defaultValue);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given null options, " +
        "When creating formatter args, " +
        "Then it throws")]
    [Trait("Formatters", nameof(FormatterArgs))]
    public async Task CreateOptions_Null_Throws()
    {
        //Given
        var ctor = GetConstructor();

        //When
        var act = () => ctor.Invoke([null]);

        //Then
        act.Should().Throw<TargetInvocationException>()
            .WithInnerException<ArgumentNullException>();
        await Task.CompletedTask;
    }

    private static ConstructorInfo GetConstructor()
    {
        var nested = typeof(FormatterArgs).GetNestedType("CreateOptions",
            BindingFlags.Instance | BindingFlags.NonPublic);
        return typeof(FormatterArgs).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: [nested!],
            modifiers: null)!;
    }

    private static object CreateOptions(object state, Exception exception, string defaultValue)
    {
        var type = typeof(FormatterArgs).GetNestedType("CreateOptions",
            BindingFlags.Instance | BindingFlags.NonPublic);
        var instance = Activator.CreateInstance(type!);
        type!.GetProperty("State")!.SetValue(instance, state);
        type!.GetProperty("Exception")!.SetValue(instance, exception);
        type!.GetProperty("DefaultValue")!.SetValue(instance, defaultValue);
        return instance!;
    }
}
