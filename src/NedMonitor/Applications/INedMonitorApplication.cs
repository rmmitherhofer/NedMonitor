using NedMonitor.Models;

namespace NedMonitor.Applications;

/// <summary>
/// Defines a contract for notifying the NedMonitor system with request context information.
/// </summary>
public interface INedMonitorApplication
{
    Task Notify(Snapshot snapshot);
}
