using System.Text.Json.Serialization;

namespace NedMonitor.HttpResponses;

/// <summary>
/// Represents a detailed response segment issued by the API for a specific category of result, such as errors or validation issues.
/// </summary>
internal class IssuerHttpResponse
{
    /// <summary>
    /// Gets the type of issue returned by the API (e.g., validation, error, not found).
    /// </summary>

    [JsonPropertyName("type")]
    public IssuerResponseType Type { get; set; }

    /// <summary>
    /// Gets the description of the issue type, usually derived from an attribute or enum name.
    /// </summary>
    [JsonPropertyName("descriptionType")]
    public string DescriptionType { get; set; }

    /// <summary>
    /// Gets or sets the title that summarizes the issue.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the detailed list of notifications associated with the issue.
    /// </summary>
    [JsonPropertyName("details")]
    public List<NotificationHttpResponse>? Details { get; set; }
}
