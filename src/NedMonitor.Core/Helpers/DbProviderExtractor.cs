using System.Data;
using System.Data.Common;

namespace NedMonitor.Core.Helpers;

public static class DbProviderExtractor
{
    /// <summary>
    /// Gets the provider name from a generic <see cref="IDbConnection"/> instance.
    /// </summary>
    public static string GetProviderName(IDbConnection connection)
    {
        if (connection is null) return "Unknown";

        return connection.GetType().Namespace
            ?? (connection as DbConnection)?.GetType().Namespace
            ?? "Unknown";
    }

    /// <summary>
    /// Gets a friendly label (like "SQL Server", "PostgreSQL") from the provider namespace.
    /// </summary>
    public static string GetFriendlyProviderName(IDbConnection connection)
    {
        var ns = GetProviderName(connection);

        return ns switch
        {
            var n when n.Contains("SqlClient") => "SQL Server",
            var n when n.Contains("Npgsql") => "PostgreSQL",
            var n when n.Contains("Oracle") => "Oracle",
            var n when n.Contains("MySql") => "MySQL",
            var n when n.Contains("Sqlite") => "SQLite",
            _ => ns
        };
    }

    /// <summary>
    /// Gets the provider name from a <see cref="DbConnection"/> instance.
    /// </summary>
    public static string GetProviderName(DbConnection connection)
    {
        if (connection is null) return "Unknown";
        return connection.GetType().Namespace ?? "Unknown";
    }

    /// <summary>
    /// Gets a friendly provider label from a <see cref="DbConnection"/> instance.
    /// </summary>
    public static string GetFriendlyProviderName(DbConnection connection)
    {
        var ns = GetProviderName(connection);

        return ns switch
        {
            var n when n.Contains("SqlClient") => "SQL Server",
            var n when n.Contains("Npgsql") => "PostgreSQL",
            var n when n.Contains("Oracle") => "Oracle",
            var n when n.Contains("MySql") => "MySQL",
            var n when n.Contains("Sqlite") => "SQLite",
            _ => ns
        };
    }
}