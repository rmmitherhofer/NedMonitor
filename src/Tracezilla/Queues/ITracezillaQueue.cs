using System.Threading.Channels;
using Tracezilla.Models;

namespace Tracezilla.Queues;

/// <summary>
/// Represents a background queue for storing <see cref="Snapshot"/> instances to be processed asynchronously.
/// </summary>
public interface ITracezillaQueue
{
    /// <summary>
    /// Enqueues a <see cref="Snapshot"/> to be processed by the Tracezilla background service.
    /// </summary>
    /// <param name="snapshot">The snapshot containing request, response, and context data.</param>
    void Enqueue(Snapshot snapshot);

    /// <summary>
    /// Gets the channel reader that provides access to the enqueued <see cref="Snapshot"/> instances.
    /// </summary>
    ChannelReader<Snapshot> Reader { get; }
}
