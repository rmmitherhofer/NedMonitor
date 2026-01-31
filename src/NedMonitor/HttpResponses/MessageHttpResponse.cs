using System.Text.Json.Serialization;

namespace NedMonitor.HttpResponses;

/// <summary>
/// Represents the base class for structured API messages that belong to a specific aggregate context.
/// </summary>
public abstract class MessageHttpResponse
{
    /// <summary>
    /// Gets or sets the type of the message, typically used to classify the event or action.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the aggregate root identifier related to the message context.
    /// </summary>
    [JsonPropertyName("aggregateId")]
    public Guid? AggregateId { get; set; }
}
