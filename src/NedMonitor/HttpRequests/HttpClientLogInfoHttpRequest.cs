using System.Net;
using System.Text.Json.Serialization;

namespace NedMonitor.HttpRequests;

/// <summary>
/// Represents detailed information about an HTTP request and response made through an HTTP client,
/// including headers, bodies, status code, timing, and exceptions (if any).
/// </summary>
public class HttpClientLogInfoHttpRequest
{
    /// <summary>
    /// The UTC timestamp when the HTTP request started.
    /// </summary>
    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// The UTC timestamp when the HTTP response was received or the request ended.
    /// </summary>
    [JsonPropertyName("endTime")]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// The HTTP method used for the request (e.g., GET, POST).
    /// </summary>
    [JsonPropertyName("method")]
    public string Method { get; set; }

    /// <summary>
    /// The full URL used in the HTTP request.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// The original URL template used to build the request, if available.
    /// </summary>
    [JsonPropertyName("templateUrl")]
    public string? TemplateUrl { get; set; }

    /// <summary>
    /// The HTTP status code returned in the response.
    /// </summary>
    [JsonPropertyName("statusCode")]
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// The request body sent with the HTTP request.
    /// </summary>
    [JsonPropertyName("requestBody")]
    public object? RequestBody { get; set; }

    /// <summary>
    /// The response body received from the HTTP call.
    /// </summary>
    [JsonPropertyName("ResponseBody")]
    public object? ResponseBody { get; set; }

    /// <summary>
    /// The headers included in the HTTP request.
    /// </summary>
    [JsonPropertyName("requestHeaders")]
    public Dictionary<string, List<string>>? RequestHeaders { get; set; }

    /// <summary>
    /// The headers returned in the HTTP response.
    /// </summary>
    [JsonPropertyName("responseHeaders")]
    public Dictionary<string, List<string>>? ResponseHeaders { get; set; }

    /// <summary>
    /// The type name of the exception thrown during the request, if any.
    /// </summary>
    [JsonPropertyName("exceptionType")]
    public string? ExceptionType { get; set; }

    /// <summary>
    /// The message associated with the exception, if any.
    /// </summary>
    [JsonPropertyName("exceptionMessage")]
    public string? ExceptionMessage { get; set; }

    /// <summary>
    /// The stack trace of the exception, if available.
    /// </summary>
    [JsonPropertyName("stackTrace")]
    public string? StackTrace { get; set; }

    /// <summary>
    /// The message from the inner exception, if it exists.
    /// </summary>
    [JsonPropertyName("innerException")]
    public string? InnerException { get; set; }
}
