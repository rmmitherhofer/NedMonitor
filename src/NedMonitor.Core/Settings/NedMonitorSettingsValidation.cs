using Microsoft.Extensions.Options;
using NedMonitor.Core.Enums;

namespace NedMonitor.Core.Settings;

public class NedMonitorSettingsValidation : IValidateOptions<NedMonitorSettings>
{
    public ValidateOptionsResult Validate(string name, NedMonitorSettings settings)
    {
        if (settings is null)
            return ValidateOptionsResult.Fail("NedMonitor settings section is missing.");

        if (settings.ProjectId == Guid.Empty)
            return ValidateOptionsResult.Fail("NedMonitor: ProjectId is required and must be a valid GUID.");

        if (settings.ProjectType == ProjectType.Undefined)
            return ValidateOptionsResult.Fail("NedMonitor: ProjectType is required.");

        if (settings.RemoteService is null)
            return ValidateOptionsResult.Fail("NedMonitor: RemoteService section is required.");

        if (string.IsNullOrWhiteSpace(settings.RemoteService.BaseAddress))
            return ValidateOptionsResult.Fail("NedMonitor: RemoteService.BaseAddress is required.");

        if (settings.RemoteService.Endpoints is null || string.IsNullOrWhiteSpace(settings.RemoteService.Endpoints.NotifyLogContext))
            return ValidateOptionsResult.Fail("NedMonitor: RemoteService.Endpoints.NotifyLogContext is required.");

        return ValidateOptionsResult.Success;
    }
}