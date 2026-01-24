using Microsoft.Extensions.Options;
using NedMonitor.Core.Enums;
using NedMonitor.Core.Settings;

namespace NedMonitor.Core.Tests.Settings.Fixtures;

public sealed class NedMonitorSettingsValidationTestsFixture
{
    public NedMonitorSettings CreateValidSettings() => new()
    {
        ProjectId = Guid.NewGuid(),
        ProjectType = ProjectType.Api,
        RemoteService = new RemoteServiceSettings
        {
            BaseAddress = "https://example.local",
            Endpoints = new NedMonitorEndpointsSettings
            {
                NotifyLogContext = "/notify"
            }
        }
    };

    public ValidateOptionsResult Validate(NedMonitorSettings? settings)
        => new NedMonitorSettingsValidation().Validate(NedMonitorSettings.NEDMONITOR_NODE, settings!);
}
