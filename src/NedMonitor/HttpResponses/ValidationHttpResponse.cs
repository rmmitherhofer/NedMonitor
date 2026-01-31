using System.Text.Json.Serialization;

namespace NedMonitor.HttpResponses;

/// <summary>
/// Represents a response that contains validation error notifications.
/// </summary>
public class ValidationHttpResponse
{
    /// <summary>
    /// Gets the list of validation notifications returned by the API.
    /// </summary>
    [JsonPropertyName("validations")]
    public List<NotificationHttpResponse> Validations { get; set; }
}
