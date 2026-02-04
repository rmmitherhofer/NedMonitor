using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NedMonitor.Core.Adapters;
using NedMonitor.Core.Settings;

namespace NedMonitor.Core.Tests.Adapters.Fixtures;

public sealed class LoggerAdapterTestsFixture
{
    public DefaultHttpContext Context { get; } = new();
    public HttpContextAccessor Accessor { get; } = new();

    public LoggerAdapterTestsFixture()
    {
        Accessor.HttpContext = Context;
    }

    internal LoggerAdapter CreateLogger(LogLevel minimumLevel, FormatterOptions options)
    {
        Context.Items.Clear();
        Accessor.HttpContext = Context;
        var settings = Options.Create(new NedMonitorSettings { MinimumLogLevel = minimumLevel });
        return new LoggerAdapter(options, "TestCategory", Accessor, settings);
    }

    internal LoggerAdapter CreateLoggerWithNullContext(LogLevel minimumLevel, FormatterOptions options)
    {
        Accessor.HttpContext = null;
        var settings = Options.Create(new NedMonitorSettings { MinimumLogLevel = minimumLevel });
        return new LoggerAdapter(options, "TestCategory", Accessor, settings);
    }
}
