using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;

/// <summary>
/// Represents detailed information about an HTTP request.
/// </summary>
public class RequestInfoHttpRequest
{
    /// <summary>
    /// The unique identifier for the request.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The HTTP method used in the request (GET, POST, etc.).
    /// </summary>
    [JsonPropertyName("httpMethod")]
    public string HttpMethod { get; set; }

    /// <summary>
    /// The full URL of the request.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// The URL scheme (http, https).
    /// </summary>
    [JsonPropertyName("scheme")]
    public string Scheme { get; set; }

    /// <summary>
    /// The HTTP protocol version.
    /// </summary>
    [JsonPropertyName("protocol")]
    public string Protocol { get; set; }

    /// <summary>
    /// Indicates if the request uses HTTPS.
    /// </summary>
    [JsonPropertyName("isHttps")]
    public bool IsHttps { get; set; }

    /// <summary>
    /// The query string parameters.
    /// </summary>
    [JsonPropertyName("queryString")]
    public string QueryString { get; set; }

    /// <summary>
    /// The route values extracted from the request URL.
    /// </summary>
    [JsonPropertyName("routeValues")]
    public IDictionary<string, string> RouteValues { get; set; }

    /// <summary>
    /// The User-Agent header from the request.
    /// </summary>
    [JsonPropertyName("userAgent")]
    public string UserAgent { get; set; }

    /// <summary>
    /// The client identifier making the request.
    /// </summary>
    [JsonPropertyName("clientId")]
    public string ClientId { get; set; }

    /// <summary>
    /// The HTTP headers included in the request.
    /// </summary>
    [JsonPropertyName("headers")]
    public IDictionary<string, List<string>> Headers { get; set; }

    /// <summary>
    /// The content type of the request body.
    /// </summary>
    [JsonPropertyName("contentType")]
    public string ContentType { get; set; }

    /// <summary>
    /// The length of the content body, if available.
    /// </summary>
    [JsonPropertyName("contentLength")]
    public long? ContentLength { get; set; }

    /// <summary>
    /// The request body content.
    /// </summary>
    [JsonPropertyName("body")]
    public object? Body { get; set; }

    /// <summary>
    /// The size of the body in bytes.
    /// </summary>
    [JsonPropertyName("bodySize")]
    public double BodySize { get; set; }

    /// <summary>
    /// Indicates if the request was made via AJAX.
    /// </summary>
    [JsonPropertyName("isAjaxRequest")]
    public bool IsAjaxRequest { get; set; }
    /// <summary>
    /// The IP address of the request origin.
    /// </summary>
    [JsonPropertyName("IpAddress")]
    public string? IpAddress { get; set; }
}
