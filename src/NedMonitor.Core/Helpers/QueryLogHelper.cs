using Microsoft.AspNetCore.Http;
using NedMonitor.Core.Models;

namespace NedMonitor.Core.Helpers;

public static class QueryLogHelper
{
    public static void AddQueryLog(HttpContext context, DbQueryEntry entry)
    {
        if (!context.Items.TryGetValue(NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY, out var obj) || obj is not List<DbQueryEntry> list)
        {
            list = [];
            context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY] = list;
        }

        list.Add(entry);
    }
}
