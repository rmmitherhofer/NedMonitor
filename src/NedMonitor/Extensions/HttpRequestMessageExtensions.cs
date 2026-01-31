namespace NedMonitor.Extensions;

/// <summary>
/// Extension methods for <see cref="HttpRequestMessage"/> to simplify header manipulation,
/// </summary>
internal static class HttpRequestMessageExtensions
{
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
}
