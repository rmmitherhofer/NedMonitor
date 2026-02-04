using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Enums;
using NedMonitor.Core.Settings;
using NedMonitor.Core.Tests.Settings.Fixtures;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Settings;

public class NedMonitorSettingsValidationTests(ITestOutputHelper output, NedMonitorSettingsValidationTestsFixture fixture) : Test(output), IClassFixture<NedMonitorSettingsValidationTestsFixture>
{
    private readonly NedMonitorSettingsValidationTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given null settings, " +
        "When validating, " +
        "Then it fails with missing section message")]
    [Trait("Settings", nameof(NedMonitorSettingsValidation))]
    public async Task Validate_NullSettings_Fails()
    {
        //Given
        NedMonitorSettings? settings = null;

        //When
        var result = _fixture.Validate(settings);

        //Then
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("settings section is missing");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given empty ProjectId, " +
        "When validating, " +
        "Then it fails with ProjectId message")]
    [Trait("Settings", nameof(NedMonitorSettingsValidation))]
    public async Task Validate_EmptyProjectId_Fails()
    {
        //Given
        var settings = _fixture.CreateValidSettings();
        settings.ProjectId = Guid.Empty;

        //When
        var result = _fixture.Validate(settings);

        //Then
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("ProjectId");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given undefined ProjectType, " +
        "When validating, " +
        "Then it fails with ProjectType message")]
    [Trait("Settings", nameof(NedMonitorSettingsValidation))]
    public async Task Validate_UndefinedProjectType_Fails()
    {
        //Given
        var settings = _fixture.CreateValidSettings();
        settings.ProjectType = ProjectType.Undefined;

        //When
        var result = _fixture.Validate(settings);

        //Then
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("ProjectType");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given missing RemoteService, " +
        "When validating, " +
        "Then it fails with RemoteService message")]
    [Trait("Settings", nameof(NedMonitorSettingsValidation))]
    public async Task Validate_MissingRemoteService_Fails()
    {
        //Given
        var settings = _fixture.CreateValidSettings();
        settings.RemoteService = null!;

        //When
        var result = _fixture.Validate(settings);

        //Then
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("RemoteService");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given empty BaseAddress, " +
        "When validating, " +
        "Then it fails with BaseAddress message")]
    [Trait("Settings", nameof(NedMonitorSettingsValidation))]
    public async Task Validate_EmptyBaseAddress_Fails()
    {
        //Given
        var settings = _fixture.CreateValidSettings();
        settings.RemoteService.BaseAddress = "";

        //When
        var result = _fixture.Validate(settings);

        //Then
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("BaseAddress");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given missing Endpoint, " +
        "When validating, " +
        "Then it fails with Endpoint message")]
    [Trait("Settings", nameof(NedMonitorSettingsValidation))]
    public async Task Validate_MissingEndpoint_Fails()
    {
        //Given
        var settings = _fixture.CreateValidSettings();
        settings.RemoteService.Endpoint = null!;

        //When
        var result = _fixture.Validate(settings);

        //Then
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Endpoint");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given valid settings, " +
        "When validating, " +
        "Then it succeeds")]
    [Trait("Settings", nameof(NedMonitorSettingsValidation))]
    public async Task Validate_ValidSettings_Succeeds()
    {
        //Given
        var settings = _fixture.CreateValidSettings();

        //When
        var result = _fixture.Validate(settings);

        //Then
        result.Succeeded.Should().BeTrue();
        await Task.CompletedTask;
    }
}
