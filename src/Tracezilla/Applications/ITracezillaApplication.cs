using Tracezilla.Models;

namespace Tracezilla.Applications;

/// <summary>
/// Defines a contract for notifying the Tracezilla system with request context information.
/// </summary>
public interface ITracezillaApplication
{
    Task Notify(Snapshot snapshot);
}
