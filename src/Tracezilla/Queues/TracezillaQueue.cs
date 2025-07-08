using System.Threading.Channels;
using Tracezilla.Models;

namespace Tracezilla.Queues;

/// <summary>
/// Implementation of <see cref="ITracezillaQueue"/> that manages an unbounded channel
/// to queue <see cref="Snapshot"/> instances for asynchronous processing.
/// </summary>
public class TracezillaQueue : ITracezillaQueue
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
