using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NedMonitor.Core.Settings;

namespace NedMonitor.Tests.Providers.Fixtures;

public sealed class LoggerProviderTestsFixture
{
    public FormatterOptions CreateFormatterOptions()
        => new()
        {
            Formatter = args => args.DefaultValue
        };

    public IHttpContextAccessor CreateHttpContextAccessor()
        => new HttpContextAccessor();

    public IOptions<NedMonitorSettings> CreateOptions()
        => Options.Create(new NedMonitorSettings
        {
            RemoteService = new RemoteServiceSettings
            {
                BaseAddress = "https://example.local/",
                Endpoint = "/logs"
            }
        });
}
