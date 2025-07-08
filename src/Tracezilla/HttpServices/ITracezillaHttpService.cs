using Tracezilla.Models;

namespace Tracezilla.HttpServices;

/// <summary>
/// Interface for sending logs to the Tracezilla service.
/// </summary>
public interface ITracezillaHttpService
{
    /// <summary>
    /// Sends a complete log context to the Tracezilla API.
    /// </summary>
    /// <param name="log">The structured log context containing request, response, and diagnostics data.</param>
    Task Flush(LogContextRequest log);
}
