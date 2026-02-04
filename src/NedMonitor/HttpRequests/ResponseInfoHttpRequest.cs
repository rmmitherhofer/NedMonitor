using System.Net;
using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;

/// <summary>
/// Represents detailed information about an HTTP response.
/// </summary>
internal class ResponseInfoHttpRequest
{
    /// <summary>
    /// The HTTP status code returned by the response.
    /// </summary>
    [JsonPropertyName("statusCode")]
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// The reason phrase associated with the status code.
    /// </summary>
    [JsonPropertyName("reasonPhrase")]
    public string ReasonPhrase { get; set; }

    /// <summary>
    /// The HTTP headers included in the response.
    /// </summary>
    [JsonPropertyName("headers")]
    public IDictionary<string, List<string>> Headers { get; set; }

    /// <summary>
    /// The body content of the response.
    /// </summary>
    [JsonPropertyName("body")]
    public object? Body { get; set; }

    /// <summary>
    /// The size of the body content in bytes.
    /// </summary>
    [JsonPropertyName("bodySize")]
    public long BodySize { get; set; }
}
