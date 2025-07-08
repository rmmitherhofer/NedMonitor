using NedMonitor.Models;

namespace NedMonitor.Builders;

/// <summary>
/// Represents a builder interface for constructing a <see cref="LogContextRequest"/> instance,
/// allowing fluent composition of various contextual elements such as log entries, notifications,
/// exceptions, and snapshot metadata collected during an HTTP request.
/// </summary>

public interface ILogContextBuilder
{
    /// <summary>
    /// Initializes the builder with a <see cref="Snapshot"/> containing core request context data.
    /// </summary>
    /// <param name="snapshot">The snapshot containing metadata for the request.</param>
    /// <returns>The current builder instance.</returns>
    ILogContextBuilder WithSnapshot(Snapshot snapshot);

    /// <summary>
    /// Adds any registered domain notifications (e.g., validation errors) to the log context.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    ILogContextBuilder WithNotifications();

    /// <summary>
    /// Adds log entries collected during the request lifecycle to the log context.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    ILogContextBuilder WithLogEntries();

    /// <summary>
    /// Attaches any exception captured during the request to the log context.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    ILogContextBuilder WithException();

    /// <summary>
    /// Builds the final <see cref="LogContextRequest"/> with all collected data.
    /// </summary>
    /// <returns>The constructed log context object.</returns>
    LogContextRequest Build();
}
