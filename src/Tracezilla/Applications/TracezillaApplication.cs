using Microsoft.Extensions.Options;
using Tracezilla.Builders;
using Tracezilla.Configurations.Settings;
using Tracezilla.Enums;
using Tracezilla.HttpServices;
using Tracezilla.Models;

namespace Tracezilla.Applications;

/// <summary>
/// Implementation of <see cref="ITracezillaApplication"/> responsible for building and sending log context data
/// to the Tracezilla API based on runtime snapshot information.
/// </summary>
public class TracezillaApplication : ITracezillaApplication
{

    private readonly ILogContextBuilder _builder;
    private readonly ITracezillaHttpService _httpService;
    private readonly TracezillaSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="TracezillaApplication"/> class.
    /// </summary>
    /// <param name="httpService">The HTTP service to send log data to Tracezilla.</param>
    /// <param name="options">Configuration options for Tracezilla.</param>
    /// <param name="builder">Builder used to construct the log context.</param>
    public TracezillaApplication(
        ITracezillaHttpService httpService,
        IOptions<TracezillaSettings> options,
        ILogContextBuilder builder)
    {
        _httpService = httpService;
        _settings = options.Value;
        _builder = builder;
    }

    /// <summary>
    /// Builds a complete log context from the provided snapshot and sends it to the Tracezilla API,
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
