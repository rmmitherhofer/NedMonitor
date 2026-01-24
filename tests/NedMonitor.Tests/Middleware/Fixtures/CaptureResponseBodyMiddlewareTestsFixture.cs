using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NedMonitor.Core.Settings;

namespace NedMonitor.Tests.Middleware.Fixtures;

public sealed class CaptureResponseBodyMiddlewareTestsFixture
{
    public DefaultHttpContext CreateContext()
        => new()
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

    public IOptions<NedMonitorSettings> CreateOptions(bool capture, int maxResponseBodySizeInMb)
        => Options.Create(new NedMonitorSettings
        {
            HttpLogging = new HttpLoggingSettings
            {
                CaptureResponseBody = capture,
                MaxResponseBodySizeInMb = maxResponseBodySizeInMb
            }
        });
}
