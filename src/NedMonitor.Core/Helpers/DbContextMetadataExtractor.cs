using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace NedMonitor.Core.Helpers;

public static partial class DbContextMetadataExtractor
{
    public static Dictionary<string, string?> Extract(IDbConnection connection)
    {
        var info = new Dictionary<string, string?>
        {
            ["Database"] = connection.Database,
            ["DataSource"] = TryGetProperty(connection, "DataSource"),
            ["UserId"] = GetUserIdFromConnectionString(connection.ConnectionString)
        };

        if (IsOracleConnection(connection))
        {
            info["ServiceName"] = GetOracleServiceName(connection.ConnectionString);
        }

        return info;
    }

    private static string? GetUserIdFromConnectionString(string connectionString)
    {
        var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

        if (builder.TryGetValue("User ID", out var userId) || builder.TryGetValue("User", out userId))
            return userId?.ToString();

        return null;
    }

    private static string? GetOracleServiceName(string connectionString)
    {
        var match = ServiceNameRegex().Match(connectionString);
        return match.Success ? match.Groups["service"].Value : null;
    }

    private static bool IsOracleConnection(IDbConnection connection)
    {
        var name = connection.GetType().Name;
        return name.Contains("Oracle", StringComparison.OrdinalIgnoreCase);
    }

    private static string? TryGetProperty(IDbConnection connection, string propertyName)
    {
        var prop = connection.GetType().GetProperty(propertyName);
        return prop?.GetValue(connection)?.ToString();
    }

    [GeneratedRegex(@"SERVICE_NAME\s*=\s*(?<service>[^\s\)]+)", RegexOptions.IgnoreCase, "pt-BR")]
    private static partial Regex ServiceNameRegex();
}
