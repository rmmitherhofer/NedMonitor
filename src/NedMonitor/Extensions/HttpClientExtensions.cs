using System.Text.Json;

namespace NedMonitor.Extensions;

/// <summary>
/// Extension methods for HttpClient to simplify header management and logging,
/// leveraging the current HTTP context via IHttpContextAccessor.
/// </summary>
internal static class HttpClientExtensions
{
    /// <summary>
    /// Adds a header to the HttpClient default request headers if it does not already exist.
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    /// <param name="key">Header name.</param>
    /// <param name="value">Header value.</param>
    public static void AddHeader(this HttpClient client, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpClient));

        if (!client.DefaultRequestHeaders.Contains(key))
            client.DefaultRequestHeaders.Add(key, value);
    }

    /// <summary>
    /// Returns the HttpClient default request headers serialized as a JSON string.
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    /// <returns>JSON string representing the headers.</returns>
    public static string GetHeadersJsonFormat(this HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(HttpClient));

        return JsonSerializer.Serialize(client.DefaultRequestHeaders.ToDictionary(h => h.Key, h => h.Value.ToArray()));
    }
}
