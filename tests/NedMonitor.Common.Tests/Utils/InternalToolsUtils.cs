using System.Text;

namespace NedMonitor.Common.Tests.Utils;

public static class publicToolsUtils
{
    public static string DecodeBase64String(string base64EncodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }
}
