namespace NedMonitor.Configurations.Settings;

/// <summary>
/// HTTP service configuration used by NedMonitor.
/// </summary>
public class HttpServiceSettings
{
    /// <summary>
    /// Base address of the HTTP service.
    /// </summary>
    public string BaseAddress { get; set; }

    /// <summary>
    /// Endpoint configurations available on the service.
    /// </summary>
    public HttpServiceEndPointsSettings EndPoints { get; set; }
}


/// <summary>
/// Endpoint configurations for the NedMonitor HTTP service.
/// </summary>
public class HttpServiceEndPointsSettings
{
    /// <summary>
    /// Endpoint used for sending notifications.
    /// </summary>
    public string Notify { get; set; }
}
