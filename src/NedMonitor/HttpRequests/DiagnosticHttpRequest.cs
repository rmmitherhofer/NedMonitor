using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;

/// <summary>
/// Represents diagnostic information about the application environment and operations.
/// </summary>
public class DiagnosticHttpRequest
{
    /// <summary>
    /// The memory usage of the application in megabytes.
    /// </summary>
    [JsonPropertyName("memoryUsageMb")]
    public double MemoryUsageMb { get; set; }

    /// <summary>
    /// The number of database queries executed.
    /// </summary>
    [JsonPropertyName("dbQueryCount")]
    public int DbQueryCount { get; set; }

    /// <summary>
    /// Indicates whether the cache was hit during the operation.
    /// </summary>
    [JsonPropertyName("cacheHit")]
    public bool CacheHit { get; set; }

    /// <summary>
    /// List of dependencies involved in the operation.
    /// </summary>
    [JsonPropertyName("dependencies")]
    public IEnumerable<DependencyInfoHttpRequest> Dependencies { get; set; }
}
