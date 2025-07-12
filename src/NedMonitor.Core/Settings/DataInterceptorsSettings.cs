namespace NedMonitor.Core.Settings;

/// <summary>
/// Configuration settings for data access interceptors used by NedMonitor.
/// Includes settings for both Entity Framework Core and Dapper interceptors.
/// </summary>
public class DataInterceptorsSettings
{
    /// <summary>
    /// Settings for the Entity Framework Core interceptor.
    /// </summary>
    public EfInterceptorSettings EF { get; set; }

    /// <summary>
    /// Settings for the Dapper interceptor.
    /// </summary>
    public DapperInterceptorSettings Dapper { get; set; }
}