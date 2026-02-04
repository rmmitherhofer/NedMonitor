namespace NedMonitor.Http.Extensions;

/// <summary>
/// Extension methods for <see cref="HttpRequestMessage"/> to simplify header manipulation,
/// </summary>
internal static class HttpRequestMessageExtensions
{
    /// <summary>
    /// The header key used to store the original request template.
    /// </summary>
    public static string X_REQUEST_TEMPLATE = "X-Request-Template";

    /// <summary>
    /// Retrieves the first value of a specific header from the HttpRequestMessage.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    /// <param name="key">Header name.</param>
    /// <returns>The header value if found; otherwise, null.</returns>
    public static string? GetHeader(this HttpRequestMessage request, string key)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        if (request.Headers.TryGetValues(key, out var values))
            return values.FirstOrDefault();

        return null;
    }

    /// <summary>
    /// Retrieves the X-Request-Template header value from the HttpRequestMessage.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    /// <returns>The route template if present; otherwise, null.</returns>
    public static string? GetHeaderRequestTemplate(this HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        return request.GetHeader(X_REQUEST_TEMPLATE);
    }
}
