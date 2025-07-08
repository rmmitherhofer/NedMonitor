namespace Tracezilla.Configurations.Settings;

/// <summary>
/// HTTP service configuration used by Tracezilla.
/// </summary>
public class TracezillaHttpServiceSettings
{
    /// <summary>
    /// Base address of the HTTP service.
    /// </summary>
    public string BaseAddress { get; set; }

    /// <summary>
    /// Endpoint configurations available on the service.
    /// </summary>
    public TracezillaHttpServiceEndPointsSettings EndPoints { get; set; }
}


/// <summary>
/// Endpoint configurations for the Tracezilla HTTP service.
/// </summary>
public class TracezillaHttpServiceEndPointsSettings
{
    /// <summary>
    /// Endpoint used for sending notifications.
    /// </summary>
    public string Notify { get; set; }
}
