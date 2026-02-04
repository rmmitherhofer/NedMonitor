using NedMonitor.Core.Models;
using System.Threading.Channels;

namespace NedMonitor.Queues;

/// <summary>
/// Represents a background queue for storing <see cref="Snapshot"/> instances to be processed asynchronously.
/// </summary>
internal interface INedMonitorQueue
{
    /// <summary>
    /// Enqueues a <see cref="Snapshot"/> to be processed by the NedMonitor background service.
    /// </summary>
    /// <param name="snapshot">The snapshot containing request, response, and context data.</param>
    void Enqueue(Snapshot snapshot);

    /// <summary>
    /// Gets the channel reader that provides access to the enqueued <see cref="Snapshot"/> instances.
    /// </summary>
    ChannelReader<Snapshot> Reader { get; }
}
