using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;

/// <summary>
/// Configuration settings for database query interceptors used by NedMonitor.
/// Contains settings for both Entity Framework Core and Dapper interceptors.
/// </summary>
public class DataInterceptorsSettingsHttpRequest
{
    /// <summary>
    /// Settings for the Entity Framework Core interceptor.
    /// </summary>
    [JsonPropertyName("ef")]
    public EfInterceptorSettingsHttpRequest? EF { get; set; }

    /// <summary>
    /// Settings for the Dapper interceptor.
    /// </summary>
    [JsonPropertyName("dapper")]
    public DapperInterceptorSettingsHttpRequest? Dapper { get; set; }
}