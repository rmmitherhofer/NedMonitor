using Microsoft.Extensions.Options;
using NedMonitor.Builders;
using NedMonitor.Core.Models;
using NedMonitor.Core.Settings;
using NedMonitor.HttpRequests;
using NedMonitor.HttpServices;

namespace NedMonitor.Applications;

/// <summary>
/// Implementation of <see cref="INedMonitorApplication"/> responsible for building and sending log context data
/// to the NedMonitor API based on runtime snapshot information.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NedMonitorApplication"/> class.
/// </remarks>
/// <param name="httpService">The HTTP service to send log data to NedMonitor.</param>
/// <param name="options">Configuration options for NedMonitor.</param>
/// <param name="builder">Builder used to construct the log context.</param>
public class NedMonitorApplication(
    INedMonitorHttpService httpService,
    IOptions<NedMonitorSettings> options,
    ILogContextBuilder builder) : INedMonitorApplication
{
    private readonly ILogContextBuilder _builder = builder;
    private readonly INedMonitorHttpService _httpService = httpService;
    private readonly NedMonitorSettings _settings = options.Value;

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

        if (_settings.ExecutionMode.EnableMonitorDbQueries) strategies.Add(b => b.WithDbQueryLogs());

        foreach (var strategy in strategies)
            strategy(builder);

        LogContextHttpRequest log = builder.Build();

        await _httpService.Flush(log);
    }
}
