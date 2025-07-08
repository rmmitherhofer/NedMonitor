using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NedMonitor.Applications;
using NedMonitor.Queues;

namespace NedMonitor.BackgroundServices;

/// <summary>
/// Background service responsible for processing log snapshots from the NedMonitor queue
/// and sending them asynchronously to the NedMonitor API.
/// </summary>
public class NedMonitorBackgroundService(INedMonitorQueue queue, INedMonitorApplication application, ILogger<NedMonitorBackgroundService> logger) : BackgroundService
{
    private readonly ILogger<NedMonitorBackgroundService> _logger = logger;
    private readonly INedMonitorQueue _queue = queue;
    private readonly INedMonitorApplication _application = application;

    /// <summary>
    /// Continuously reads log snapshots from the queue and forwards them to the NedMonitor application
    /// for processing and delivery. Handles exceptions to ensure the background service remains alive.
    /// </summary>
    /// <param name="stoppingToken">Token used to signal service shutdown.</param>

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var snapshot in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await _application.Notify(snapshot);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(NedMonitorBackgroundService)}]:{ex.Message} - {ex.StackTrace}");
            }
        }
    }
}
