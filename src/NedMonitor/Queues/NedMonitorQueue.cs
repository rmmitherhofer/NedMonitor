using NedMonitor.Core.Models;
using System.Threading.Channels;

namespace NedMonitor.Queues;

/// <summary>
/// Implementation of <see cref="INedMonitorQueue"/> that manages an unbounded channel
/// to queue <see cref="Snapshot"/> instances for asynchronous processing.
/// </summary>
public class NedMonitorQueue : INedMonitorQueue
{
    private readonly Channel<Snapshot> _queue = Channel.CreateUnbounded<Snapshot>();

    /// <summary>
    /// Enqueues a <see cref="Snapshot"/> to the internal channel for processing.
    /// </summary>
    /// <param name="snapshot">The snapshot to enqueue.</param>
    public void Enqueue(Snapshot snapshot)
    {
        _queue.Writer.TryWrite(snapshot);
    }

    /// <summary>
    /// Gets the <see cref="ChannelReader{Snapshot}"/> to read snapshots from the queue.
    /// </summary>
    public ChannelReader<Snapshot> Reader => _queue.Reader;
}
