namespace NedMonitor.Dapper.Cache;

internal static class OrmVersionCache
{
    private static readonly string DapperVersion;

    static OrmVersionCache() => DapperVersion = GetDapperVersion();

    private static string GetDapperVersion()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            if (assembly.GetName().Name == "Dapper")
                return assembly.GetName().Version?.ToString() ?? "unknown";
        }
        return "not loaded";
    }

    public static string DapperVersionCached => DapperVersion;
}
