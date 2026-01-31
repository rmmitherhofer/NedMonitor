using System.ComponentModel;

namespace NedMonitor.HttpResponses;

/// <summary>
/// Defines the types of issues that can be returned by the API in a structured response.
/// </summary>
public enum IssuerResponseType
{
    /// <summary>
    /// Indicates that the requested resource was not found.
    /// </summary>
    [Description("NotFound")]
    NotFound,

    /// <summary>
    /// Indicates that one or more validation rules were violated.
    /// </summary>
    [Description("Validation")]
    Validation,

    /// <summary>
    /// Indicates a generic or unexpected error occurred during the request.
    /// </summary>
    [Description("Error")]
    Error
}
