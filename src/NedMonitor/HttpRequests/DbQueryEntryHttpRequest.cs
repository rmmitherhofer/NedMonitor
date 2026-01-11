using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;

/// <summary>
/// Represents a single query execution log entry.
/// </summary>
public class DbQueryEntryHttpRequest
{
    /// <summary>
    /// The raw SQL command text executed.
    /// </summary>
    [JsonPropertyName("provider")]
    public string Provider { get; set; }

    /// <summary>
    /// The raw SQL command text executed.
    /// </summary>
    [JsonPropertyName("sql")]
    public string? Sql { get; set; }

    /// <summary>
    /// The parameters used in the query, serialized as string.
    /// </summary>
    [JsonPropertyName("parameters")]
    public string? Parameters { get; set; }

    /// <summary>
    /// The timestamp when the query was executed.
    /// </summary>
    [JsonPropertyName("executedAt")]
    public DateTime ExecutedAt { get; set; }

    /// <summary>
    /// The duration of the query execution in milliseconds (optional).
    /// </summary>
    [JsonPropertyName("durationMs")]
    public double DurationMs { get; set; }

    /// <summary>
    /// Indicates whether the query execution was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; } = true;

    /// <summary>
    /// The exception message if the query failed.
    /// </summary>
    [JsonPropertyName("exceptionMessage")]
    public string? ExceptionMessage { get; set; }

    /// <summary>
    /// Additional context information from the database context, as key-value pairs.
    /// </summary>
    [JsonPropertyName("dbContext")]
    public IDictionary<string, string>? DbContext { get; set; }

    /// <summary>
    /// The ORM framework used for this query (e.g., EF Core, Dapper).
    /// </summary>
    [JsonPropertyName("orm")]
    public string ORM { get; set; }
}
