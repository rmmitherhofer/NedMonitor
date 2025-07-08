using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tracezilla.Applications;
using Tracezilla.Queues;

namespace Tracezilla.BackgroundServices;

/// <summary>
/// Background service responsible for processing log snapshots from the Tracezilla queue
/// and sending them asynchronously to the Tracezilla API.
/// </summary>
public class TracezillaBackgroundService(ITracezillaQueue queue, ITracezillaApplication tracezilla, ILogger<TracezillaBackgroundService> logger) : BackgroundService
{
    private readonly ILogger<TracezillaBackgroundService> _logger = logger;
    private readonly ITracezillaQueue _queue = queue;
    private readonly ITracezillaApplication _tracezilla = tracezilla;

    /// <summary>
    /// Continuously reads log snapshots from the queue and forwards them to the Tracezilla application
    /// for processing and delivery. Handles exceptions to ensure the background service remains alive.
    /// </summary>
    /// <param name="stoppingToken">Token used to signal service shutdown.</param>

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var snapshot in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await _tracezilla.Notify(snapshot);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(TracezillaBackgroundService)}]:{ex.Message} - {ex.StackTrace}");
            }
        }
    }
}
