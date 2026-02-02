using System.Text.Json;

namespace NedMonitor.Extensions;

internal static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Reads the HTTP response content as a JSON string and deserializes it into the specified type.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the JSON content into.</typeparam>
    /// <param name="response">The HttpResponseMessage instance.</param>
    /// <param name="options">Optional JsonSerializerOptions to use during deserialization.</param>
    /// <returns>The deserialized object, or default if content is empty.</returns>
    public static async Task<TResponse?> ReadAsAsync<TResponse>(this HttpResponseMessage response, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(HttpResponseMessage));

        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(content)) return default;

        return JsonSerializer.Deserialize<TResponse>(content, options);
    }

    /// <summary>
    /// Determines whether the HTTP response represents a non-successful status code.
    /// </summary>    
    public static bool HasErrors(this HttpResponseMessage response)
        => !response.IsSuccessStatusCode;
}
