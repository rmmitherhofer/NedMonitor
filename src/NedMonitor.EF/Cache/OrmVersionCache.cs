namespace NedMonitor.EF.Cache;

/// <summary>
/// Provides a cached reference to the Entity Framework Core version being used at runtime.
/// This is useful for logging or diagnostic purposes without repeatedly accessing reflection.
/// </summary>
public static class OrmVersionCache
{
    private static readonly string EfCoreVersion;

    static OrmVersionCache() => EfCoreVersion = GetAssemblyVersion(typeof(Microsoft.EntityFrameworkCore.DbContext));

    private static string GetAssemblyVersion(Type type) => type.Assembly.GetName().Version?.ToString() ?? "unknown";

    /// <summary>
    /// Gets the version string of the currently loaded Entity Framework Core assembly.
    /// </summary>
    public static string EfVersion => EfCoreVersion;
}