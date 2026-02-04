using NedMonitor.HttpRequests;

namespace NedMonitor.HttpServices;

/// <summary>
/// Interface for sending logs to the NedMonitor service.
/// </summary>
internal interface INedMonitorHttpService
{
    /// <summary>
    /// Sends a complete log context to the NedMonitor API.
    /// </summary>
    /// <param name="log">The structured log context containing request, response, and diagnostics data.</param>
    Task Flush(LogContextHttpRequest log);
}
