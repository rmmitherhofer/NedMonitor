using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Settings;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Settings;

public class CoreSettingsDefaultsTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given execution mode defaults, " +
        "When created, " +
        "Then EnableNedMonitor is true and others are false")]
    [Trait("Settings", nameof(ExecutionModeSettings))]
    public async Task ExecutionModeSettings_Defaults_AreExpected()
    {
        //Given
        var settings = new ExecutionModeSettings();

        //When
        var enabled = settings.EnableNedMonitor;

        //Then
        enabled.Should().BeTrue();
        settings.EnableMonitorExceptions.Should().BeFalse();
        settings.EnableMonitorNotifications.Should().BeFalse();
        settings.EnableMonitorLogs.Should().BeFalse();
        settings.EnableMonitorHttpRequests.Should().BeFalse();
        settings.EnableMonitorDbQueries.Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given http logging settings defaults, " +
        "When created, " +
        "Then MaxResponseBodySizeInMb is 1")]
    [Trait("Settings", nameof(HttpLoggingSettings))]
    public async Task HttpLoggingSettings_Defaults_AreExpected()
    {
        //Given
        var settings = new HttpLoggingSettings();

        //When
        var maxSize = settings.MaxResponseBodySizeInMb;

        //Then
        maxSize.Should().Be(1);
        settings.WritePayloadToConsole.Should().BeFalse();
        settings.CaptureResponseBody.Should().BeFalse();
        settings.CaptureCookies.Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given sensitive data masker defaults, " +
        "When created, " +
        "Then it uses enabled with default mask value")]
    [Trait("Settings", nameof(SensitiveDataMaskerSettings))]
    public async Task SensitiveDataMaskerSettings_Defaults_AreExpected()
    {
        //Given
        var settings = new SensitiveDataMaskerSettings();

        //When
        var enabled = settings.Enabled;

        //Then
        enabled.Should().BeTrue();
        settings.MaskValue.Should().Be("***REDACTED***");
        settings.SensitiveKeys.Should().BeNull();
        settings.SensitivePatterns.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given data interceptors defaults, " +
        "When created, " +
        "Then nested settings are initialized")]
    [Trait("Settings", nameof(DataInterceptorsSettings))]
    public async Task DataInterceptorsSettings_Defaults_AreExpected()
    {
        //Given
        var settings = new DataInterceptorsSettings();

        //When
        var ef = settings.EF;

        //Then
        ef.Should().NotBeNull();
        settings.Dapper.Should().NotBeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given exceptions settings defaults, " +
        "When created, " +
        "Then expected list is empty")]
    [Trait("Settings", nameof(ExceptionsSettings))]
    public async Task ExceptionsSettings_Defaults_AreExpected()
    {
        //Given
        var settings = new ExceptionsSettings();

        //When
        var expected = settings.Expected;

        //Then
        expected.Should().NotBeNull();
        expected.Should().BeEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given ORM settings defaults, " +
        "When created, " +
        "Then capture options is empty and enabled is null")]
    [Trait("Settings", nameof(ORMSettings))]
    public async Task OrmSettings_Defaults_AreExpected()
    {
        //Given
        var dapper = new DapperInterceptorSettings();
        var ef = new EfInterceptorSettings();

        //When
        var dapperOptions = dapper.CaptureOptions;
        var efOptions = ef.CaptureOptions;

        //Then
        dapper.Enabled.Should().BeNull();
        ef.Enabled.Should().BeNull();
        dapperOptions.Should().BeEmpty();
        efOptions.Should().BeEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given formatter options defaults, " +
        "When created, " +
        "Then formatter is null")]
    [Trait("Settings", nameof(FormatterOptions))]
    public async Task FormatterOptions_Defaults_AreExpected()
    {
        //Given
        var options = new FormatterOptions();

        //When
        var formatter = options.Formatter;

        //Then
        formatter.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given handler options configuration, " +
        "When accessed, " +
        "Then options instance is not null")]
    [Trait("Settings", nameof(HandlerOptionsConfiguration))]
    public async Task HandlerOptionsConfiguration_Defaults_AreExpected()
    {
        //Given
        var options = HandlerOptionsConfiguration.Options;

        //When
        var handlers = options;

        //Then
        handlers.Should().NotBeNull();
        await Task.CompletedTask;
    }
}
