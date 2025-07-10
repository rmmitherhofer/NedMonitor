namespace NedMonitor.Core.Settings;

/// <summary>
/// Global static configuration for NedMonitor options.
/// Provides centralized access to the <see cref="HandlerOptions"/> object.
/// </summary>
public static class HandlerOptionsConfiguration
{
    /// <summary>
    /// Static instance of the NedMonitor handler options.
    /// </summary>
    public static HandlerOptions Options { get; private set; } = new();
}
