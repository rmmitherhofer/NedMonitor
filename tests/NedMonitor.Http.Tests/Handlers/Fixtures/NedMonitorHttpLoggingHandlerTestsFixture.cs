using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NedMonitor.Core.Settings;
using NedMonitor.Http.Handlers;
using Zypher.Http.Extensions;

namespace NedMonitor.Http.Tests.Handlers.Fixtures;

public sealed class NedMonitorHttpLoggingHandlerTestsFixture
{
    public DefaultHttpContext Context { get; } = new();
    public IHttpContextAccessor Accessor => new TestHttpContextAccessor { HttpContext = Context };
    public IHttpContextAccessor NullAccessor => new TestHttpContextAccessor { HttpContext = null };

    public NedMonitorSettings CreateSettings(bool enabled, int maxResponseBodySizeInMb = 1)
    {
        return new NedMonitorSettings
        {
            ExecutionMode = new ExecutionModeSettings
            {
                EnableNedMonitor = enabled,
                EnableMonitorHttpRequests = enabled
            },
            HttpLogging = new HttpLoggingSettings
            {
                MaxResponseBodySizeInMb = maxResponseBodySizeInMb
            }
        };
    }

    public NedMonitorHttpLoggingHandler CreateHandler(
        NedMonitorSettings settings,
        HttpMessageHandler innerHandler)
    {
        return new NedMonitorHttpLoggingHandler(Accessor, Options.Create(settings))
        {
            InnerHandler = innerHandler
        };
    }

    public TestHttpMessageHandler CreateInnerHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        => new(handler);

    public TestHttpMessageHandler CreateThrowingInnerHandler(Exception exception)
        => new(_ => throw exception);

    public NedMonitorHttpLoggingHandler CreateHandlerWithNullContext(
        NedMonitorSettings settings,
        HttpMessageHandler innerHandler)
    {
        return new NedMonitorHttpLoggingHandler(NullAccessor, Options.Create(settings))
        {
            InnerHandler = innerHandler
        };
    }

    private sealed class TestHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; }
    }

    public sealed class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public bool SawTemplateHeader { get; private set; }

        public TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            SawTemplateHeader = request.Headers.Contains(HttpRequestMessageExtensions.X_REQUEST_TEMPLATE);
            return Task.FromResult(_handler(request));
        }
    }
}
