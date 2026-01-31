using System.Net;
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
    /// Determines if the HTTP response contains error status codes and optionally throws a custom exception.
    /// </summary>
    /// <param name="response">The HttpResponseMessage instance.</param>
    /// <param name="throwException">Indicates whether to throw an exception for certain status codes.</param>
    /// <returns>True if the response has errors; otherwise, false.</returns>
    /// <exception cref="HttpRequestException">Thrown when the status code is Unauthorized or publicServerError and throwException is true.</exception>
    public static bool HasErrors(this HttpResponseMessage response, bool throwException = true)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(HttpResponseMessage));

        var statusCode = response.StatusCode;

        if (statusCode is HttpStatusCode.BadRequest or HttpStatusCode.Forbidden or HttpStatusCode.BadGateway) return true;

        if (statusCode is HttpStatusCode.Unauthorized or HttpStatusCode.InternalServerError)
        {
            if (throwException)
            {
                var method = response.RequestMessage?.Method?.Method ?? "UNKNOWN";
                var uri = response.RequestMessage?.RequestUri?.ToString() ?? "UNKNOWN";

                throw new HttpRequestException($"{method} - {uri} - {(int)statusCode} - {statusCode}", null, statusCode);
            }
            return true;
        }

        try
        {
            response.EnsureSuccessStatusCode();
            return false;
        }
        catch
        {
            return true;
        }
    }
}
