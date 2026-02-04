using NedMonitor.Core.Models;

namespace NedMonitor.Applications;

/// <summary>
/// Defines a contract for notifying the NedMonitor system with request context information.
/// </summary>
internal interface INedMonitorApplication
{
    Task Notify(Snapshot snapshot);
}
