namespace NedMonitor.Core.Settings;

/// <summary>
/// HTTP service configuration used by NedMonitor.
/// </summary>
public class RemoteServiceSettings
{
    /// <summary>
    /// Base address of the HTTP service.
    /// </summary>
    public string BaseAddress { get; set; }

    /// <summary>
    /// Endpoint configurations available on the service.
    /// </summary>
    public NedMonitorEndpointsSettings EndPoints { get; set; }
}


/// <summary>
/// Endpoint configurations for the NedMonitor HTTP service.
/// </summary>
public class NedMonitorEndpointsSettings
{
    /// <summary>
    /// Endpoint used for sending notifications.
    /// </summary>
    public string NotifyLogContext { get; set; }
}
