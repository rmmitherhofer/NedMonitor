using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NedMonitor.Applications;
using NedMonitor.Queues;

namespace NedMonitor.BackgroundServices;

/// <summary>
/// Background service responsible for processing log snapshots from the NedMonitor queue
/// and sending them asynchronously to the NedMonitor API.
/// </summary>
internal class NedMonitorBackgroundService(IServiceScopeFactory scopeFactory, ILogger<NedMonitorBackgroundService> logger) : BackgroundService
{
    private readonly ILogger<NedMonitorBackgroundService> _logger = logger;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    /// <summary>
    /// Continuously reads log snapshots from the queue and forwards them to the NedMonitor application
    /// for processing and delivery. Handles exceptions to ensure the background service remains alive.
    /// </summary>
    /// <param name="stoppingToken">Token used to signal service shutdown.</param>

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var queue = scope.ServiceProvider.GetRequiredService<INedMonitorQueue>();
                var application = scope.ServiceProvider.GetRequiredService<INedMonitorApplication>();

                await foreach (var snapshot in queue.Reader.ReadAllAsync(stoppingToken))
                {
                    try
                    {
                        await application.Notify(snapshot);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"[{nameof(NedMonitorBackgroundService)}]:{ex.Message} - {ex.StackTrace}");
                    }
                }
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
