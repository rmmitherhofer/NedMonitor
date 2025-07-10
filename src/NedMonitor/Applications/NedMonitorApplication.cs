using Microsoft.Extensions.Options;
using NedMonitor.Builders;
using NedMonitor.Core.Models;
using NedMonitor.Core.Settings;
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
        if (!_settings.ExecutionMode.EnableNedMonitor) return;

        ILogContextBuilder builder = _builder.WithSnapshot(snapshot);

        var strategies = new List<Action<ILogContextBuilder>>();

        if (_settings.ExecutionMode.EnableMonitorExceptions) strategies.Add(b => b.WithException());

        if (_settings.ExecutionMode.EnableMonitorHttpRequests) strategies.Add(b => b.WithHttpClientLogs());

        if (_settings.ExecutionMode.EnableMonitorNotifications) strategies.Add(b => b.WithNotifications());

        if (_settings.ExecutionMode.EnableMonitorLogs) strategies.Add(b => b.WithLogEntries());

        foreach (var strategy in strategies)
            strategy(builder);

        LogContextHttpRequest log = builder.Build();

        await _httpService.Flush(log);
    }
}
