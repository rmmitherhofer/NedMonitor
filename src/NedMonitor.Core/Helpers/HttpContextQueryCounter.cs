using Microsoft.AspNetCore.Http;
using NedMonitor.Core.Interfaces;

namespace NedMonitor.Core.Helpers;

public class HttpContextQueryCounter : IQueryCounter
{
    private readonly IHttpContextAccessor _accessor;

    public HttpContextQueryCounter(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public void Increment()
    {
        var context = _accessor.HttpContext;
        if (context == null) return;

        if (!context.Items.TryGetValue(NedMonitorConstants.CONTEXT_QUERY_COUNT_KEY, out var count) || count is not int current)
        {
            current = 0;
        }

        context.Items[NedMonitorConstants.CONTEXT_QUERY_COUNT_KEY] = current + 1;
    }


    public int GetCount(HttpContext context)
    {
        if (context.Items.TryGetValue(NedMonitorConstants.CONTEXT_QUERY_COUNT_KEY, out var count) && count is int value)
            return value;

        return 0;
    }

    public void Reset(HttpContext context)
    {
        context.Items[NedMonitorConstants.CONTEXT_QUERY_COUNT_KEY] = 0;
    }
}
