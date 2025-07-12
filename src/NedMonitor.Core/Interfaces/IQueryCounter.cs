using Microsoft.AspNetCore.Http;

namespace NedMonitor.Core.Interfaces;

public interface IQueryCounter
{
    void Increment();
    int GetCount(HttpContext context);
    void Reset(HttpContext context);
}
