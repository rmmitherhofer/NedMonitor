using System.Text.Json.Serialization;
using Tracezilla.Enums;

namespace Tracezilla.Models;


/// <summary>
/// Contains basic information about a project.
/// </summary>
public class ProjectInfo
{
    /// <summary>
    /// Unique identifier of the project.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the project.
    /// </summary>
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    /// <summary>
    /// Type of the project.
    /// </summary>
    [JsonPropertyName("type")]
    public ProjectType Type { get; set; }

    /// <summary>
    /// Defines the current logging behavior of Tracezilla.
    /// </summary>
    [JsonPropertyName("executionMode")]
    public ExecutionMode ExecutionMode { get; set; }

    /// <summary>
    /// Maximum size of the response body to capture, in megabytes.
    /// </summary>
    [JsonPropertyName("maxResponseBodySizeInMb")]
    public int MaxResponseBodySizeInMb { get; set; }

    /// <summary>
    /// Indicates whether to capture the response body for logging.
    /// </summary>
    [JsonPropertyName("captureResponseBody")]
    public bool CaptureResponseBody { get; set; }

    /// <summary>
    /// Indicates whether the request and response payload should be written to the console output.
    /// Useful for debugging during development.
    /// </summary>
    [JsonPropertyName("writePayloadToConsole")]
    public bool WritePayloadToConsole { get; set; }
}


