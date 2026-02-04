using System.Net;
using System.Text.Json.Serialization;

namespace NedMonitor.HttpResponses;

/// <summary>
/// Represents a standardized structure for returning error details from the API.
/// </summary>
internal class ErrorHttpResponse
{
    /// <summary>
    /// Gets the HTTP status code associated with the error.
    /// </summary>
    [JsonPropertyName("statusCode")]
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// Gets the message that describes the error.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }
}
