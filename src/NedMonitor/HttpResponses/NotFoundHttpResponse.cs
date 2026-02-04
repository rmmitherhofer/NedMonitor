using System.Net;
using System.Text.Json.Serialization;

namespace NedMonitor.HttpResponses;

/// <summary>
/// Represents a standard 404 Not Found API response with optional detail information.
/// </summary>
internal class NotFoundHttpResponse
{
    /// <summary>
    /// Gets the HTTP status code representing the response.
    /// </summary>
    [JsonPropertyName("status")]
    public HttpStatusCode Status { get; set; }

    /// <summary>
    /// Gets the title that summarizes the type of response.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// Gets the detailed message associated with the response.
    /// </summary>
    [JsonPropertyName("detail")]
    public string Detail { get; set; }
}
