namespace NedMonitor.Core.Models;

/// <summary>
/// Represents the logging context of an HTTP request and its response,
/// including timing, headers, bodies, status, and exception details if any.
/// </summary>
public class HttpRequestLogContext
{
    /// <summary>
    /// The timestamp when the HTTP request started.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// The timestamp when the HTTP request ended.
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// The HTTP method used in the request (GET, POST, PUT, DELETE, etc.).
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// The full URL requested, including scheme, host, path, and query string.
    /// </summary>
    public string FullUrl { get; set; }

    /// <summary>
    /// Optional URL template (route pattern) matched for the request, if available.
    /// </summary>
    public string? UrlTemplate { get; set; }

    /// <summary>
    /// The HTTP status code returned by the response.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// The raw body content of the HTTP request as a string, if captured.
    /// </summary>
    public string? RequestBody { get; set; }

    /// <summary>
    /// The raw body content of the HTTP response as a string, if captured.
    /// </summary>
    public string? ResponseBody { get; set; }

    /// <summary>
    /// Collection of request headers, with header names as keys and list of values.
    /// </summary>
    public Dictionary<string, List<string>>? RequestHeaders { get; set; }

    /// <summary>
    /// Collection of response headers, with header names as keys and list of values.
    /// </summary>
    public Dictionary<string, List<string>>? ResponseHeaders { get; set; }

    /// <summary>
    /// The type or class name of the exception, if an exception occurred.
    /// </summary>
    public string? ExceptionType { get; set; }

    /// <summary>
    /// The message of the exception, if any.
    /// </summary>
    public string? ExceptionMessage { get; set; }

    /// <summary>
    /// The full stack trace of the exception, if any.
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// The message or details of an inner exception, if any.
    /// </summary>
    public string? InnerException { get; set; }
}