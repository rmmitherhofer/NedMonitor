using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NedMonitor.Core.Settings;
using System.Linq;

namespace NedMonitor.Tests.Middleware.Fixtures;

public sealed class NedMonitorExceptionCaptureMiddlewareTestsFixture
{
    public DefaultHttpContext CreateContext() => new();

    public IOptions<NedMonitorSettings> CreateOptions(params string[] expectedExceptions)
        => Options.Create(new NedMonitorSettings
        {
            Exceptions = new ExceptionsSettings
            {
                Expected = expectedExceptions.ToList()
            }
        });
}
