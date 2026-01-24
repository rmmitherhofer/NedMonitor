using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Formatters;
using NedMonitor.Core.Settings;
using NedMonitor.Core.Tests.Formatters.Fixtures;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Formatters;

public class ExceptionFormatterTests(ITestOutputHelper output, ExceptionFormatterTestsFixture fixture)
    : Test(output), IClassFixture<ExceptionFormatterTestsFixture>
{
    private readonly ExceptionFormatterTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given nested exceptions, " +
        "When formatting, " +
        "Then it includes base exception header and message")]
    [Trait("Formatters", nameof(ExceptionFormatter))]
    public async Task Format_NestedException_IncludesBaseException()
    {
        //Given
        var exception = _fixture.CreateNestedException();
        var formatter = new ExceptionFormatter();

        //When
        var output = formatter.Format(exception);

        //Then
        output.Should().Contain("Base Exception:");
        output.Should().Contain("base");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given append handler configured, " +
        "When formatting exception, " +
        "Then it includes appended details")]
    [Trait("Formatters", nameof(ExceptionFormatter))]
    public async Task Format_WithAppendHandler_IncludesAppendDetails()
    {
        //Given
        var exception = new Exception("boom");
        var formatter = new ExceptionFormatter();
        HandlerOptionsConfiguration.Options.AppendExceptionDetails(_ => "extra details");

        try
        {
            //When
            var output = formatter.Format(exception);

            //Then
            output.Should().Contain("extra details");
        }
        finally
        {
            HandlerOptionsConfiguration.Options.AppendExceptionDetails(_ => null);
        }

        await Task.CompletedTask;
    }
}
