using System.Net;
using System.Text.Json.Serialization;

namespace NedMonitor.HttpResponses;

/// <summary>
/// Represents a standard API response structure used for error, validation, and not found results.
/// </summary>
public class ApiHttpResponse
{
    /// <summary>
    /// Gets the HTTP status code associated with the response.
    /// </summary>
    [JsonPropertyName("statusCode")]
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the correlation identifier used for request tracking.
    /// </summary>
    [JsonPropertyName("correlationId")]
    public string CorrelationId { get; set; }

    /// <summary>
    /// Gets the list of issues returned in the response.
    /// </summary>
    [JsonPropertyName("issues")]
    public IEnumerable<IssuerHttpResponse> Issues { get; set; }
}
