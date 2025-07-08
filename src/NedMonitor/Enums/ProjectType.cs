namespace NedMonitor.Enums;

/// <summary>
/// Defines the type of the project for NedMonitor categorization.
/// </summary>
public enum ProjectType
{
    /// <summary>
    /// Web application project type.
    /// </summary>
    Webapp = 1,

    /// <summary>
    /// API project type.
    /// </summary>
    Api = 2,

    /// <summary>
    /// Background job or scheduled task project type.
    /// </summary>
    Job = 3,

    /// <summary>
    /// gRPC service project type.
    /// </summary>
    gRCP = 4,
}
