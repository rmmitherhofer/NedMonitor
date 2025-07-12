namespace NedMonitor.Core.Models;

/// <summary>
/// Represents a single query execution log entry.
/// </summary>
public class DbQueryEntry
{
    /// <summary>
    /// The raw SQL command text executed.
    /// </summary>
    public string? Sql { get; set; }

    /// <summary>
    /// The parameters used in the query, serialized as string.
    /// </summary>
    public string? Parameters { get; set; }

    /// <summary>
    /// The timestamp when the query was executed (UTC).
    /// </summary>
    public DateTime ExecutedAtUtc { get; set; }

    /// <summary>
    /// The duration of the query execution in milliseconds (optional).
    /// </summary>
    public double? DurationMs { get; set; }

    /// <summary>
    /// Indicates whether the query execution was successful.
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// The exception message if the query failed.
    /// </summary>
    public string? ExceptionMessage { get; set; }
    /// <summary>
    /// Additional context information from the database context, as key-value pairs.
    /// </summary>
    public IDictionary<string, string>? DbContext { get; set; }
    /// <summary>
    /// The ORM framework used for this query (e.g., EF Core, Dapper).
    /// </summary>
    public string ORM { get; set; }
}
