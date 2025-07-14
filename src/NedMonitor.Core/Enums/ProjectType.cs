namespace NedMonitor.Core.Enums;

/// <summary>
/// Defines the type of the project for NedMonitor categorization.
/// </summary>
public enum ProjectType
{
    Undefined = 0,
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

    /// <summary>
    /// Console application project type, typically used for CLI tools or utilities.
    /// </summary>
    Console = 5,

    /// <summary>
    /// Windows service or daemon process project type.
    /// </summary>
    Service = 6,

    /// <summary>
    /// Blazor WebAssembly or Server-Side application.
    /// </summary>
    Blazor = 7,

    /// <summary>
    /// Xamarin or MAUI mobile application.
    /// </summary>
    Mobile = 8,

    /// <summary>
    /// Desktop application using WPF or WinForms.
    /// </summary>
    Desktop = 9,

    /// <summary>
    /// Serverless function (e.g., Azure Functions, AWS Lambda).
    /// </summary>
    Function = 10,

    /// <summary>
    /// Library project, such as a class library or shared module.
    /// </summary>
    Library = 11,

    /// <summary>
    /// Test project for unit, integration, or end-to-end testing.
    /// </summary>
    Test = 12
}