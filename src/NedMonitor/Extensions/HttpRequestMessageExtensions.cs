using System.Text.Json;

namespace NedMonitor.Extensions;

/// <summary>
/// Extension methods for <see cref="HttpRequestMessage"/> to simplify header manipulation,
/// </summary>
internal static class HttpRequestMessageExtensions
{
    /// <summary>
    /// Adds or updates a header in the HttpRequestMessage.
    /// </summary>
    /// <param name="request">HttpRequestMessage instance.</param>
    /// <param name="key">Header name.</param>
    /// <param name="value">Header value.</param>
    public static void AddOrUpdateHeader(this HttpRequestMessage request, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(HttpRequestMessage));

        if (request.Headers.Contains(key))
            request.Headers.Remove(key);

        request.Headers.TryAddWithoutValidation(key, value);
    }
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
    /// Returns the HttpRequestMessage request headers serialized as a JSON string.
    /// </summary>
    /// <param name="client">HttpRequestMessage instance.</param>
    /// <returns>JSON string representing the headers.</returns>
    public static string GetHeadersJsonFormat(this HttpRequestMessage client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpRequestMessage));

        return JsonSerializer.Serialize(client.Headers.ToDictionary(h => h.Key, h => h.Value.ToArray()));
    }
}
