namespace Tracezilla.Configurations.Settings;

/// <summary>
/// Global static configuration for Tracezilla options.
/// Provides centralized access to the <see cref="HandlerOptions"/> object.
/// </summary>
public static class HandlerOptionsConfiguration
{
    /// <summary>
    /// Static instance of the Tracezilla handler options.
    /// </summary>
    public static HandlerOptions Options { get; private set; } = new();
}
