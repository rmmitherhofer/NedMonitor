namespace NedMonitor.Enums;

/// <summary>
/// Defines the execution mode for NedMonitor log processing,
/// determining which types of events will be captured and sent.
/// </summary>
public enum ExecutionMode
{
    /// <summary>
    /// Disables NedMonitor logging. No logs, notifications, or exceptions will be processed.
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// Captures only exceptions during execution.
    /// </summary>
    ExceptionsOnly = 1,

    /// <summary>
    /// Captures both domain/application notifications and exceptions.
    /// </summary>
    NotificationsAndExceptions = 2,

    /// <summary>
    /// Captures logs, notifications, and exceptions (full monitoring).
    /// </summary>
    Full = 3
}