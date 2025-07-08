using Microsoft.Extensions.Options;
using NedMonitor.Builders;
using NedMonitor.Configurations.Settings;
using NedMonitor.Enums;
using NedMonitor.HttpServices;
using NedMonitor.Models;

namespace NedMonitor.Applications;

/// <summary>
/// Implementation of <see cref="INedMonitorApplication"/> responsible for building and sending log context data
/// to the NedMonitor API based on runtime snapshot information.
/// </summary>
public class NedMonitorApplication : INedMonitorApplication
{

    private readonly ILogContextBuilder _builder;
    private readonly INedMonitorHttpService _httpService;
    private readonly NedMonitorSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="NedMonitorApplication"/> class.
    /// </summary>
    /// <param name="httpService">The HTTP service to send log data to NedMonitor.</param>
    /// <param name="options">Configuration options for NedMonitor.</param>
    /// <param name="builder">Builder used to construct the log context.</param>
    public NedMonitorApplication(
        INedMonitorHttpService httpService,
        IOptions<NedMonitorSettings> options,
        ILogContextBuilder builder)
    {
        _httpService = httpService;
        _settings = options.Value;
        _builder = builder;
    }

    /// <summary>
    /// Builds a complete log context from the provided snapshot and sends it to the NedMonitor API,
    /// if logging is enabled via configuration.
    /// </summary>
    /// <param name="snapshot">The snapshot containing request, response, exception, and other contextual data.</param>

    public async Task Notify(Snapshot snapshot)
    {
        if (_settings.ExecutionMode == ExecutionMode.Disabled) return;

        LogContextRequest log = null;

        switch (_settings.ExecutionMode)
        {
            case ExecutionMode.ExceptionsOnly:
                log = _builder.WithSnapshot(snapshot)
                    .WithException()
                    .Build();
                break;

            case ExecutionMode.NotificationsAndExceptions:
                log = _builder.WithSnapshot(snapshot)
                    .WithNotifications()
                    .WithException()
                    .Build();
                break;

            case ExecutionMode.Full:
                log = _builder.WithSnapshot(snapshot)
                    .WithLogEntries()
                    .WithNotifications()
                    .WithException()
                    .Build();
                break;
        }
        await _httpService.Flush(log);
    }
}
