using Microsoft.AspNetCore.Http;
using NedMonitor.Core.Helpers;

namespace NedMonitor.Core.Tests.Helpers.Fixtures;

public sealed class HttpContextQueryCounterTestsFixture
{
    public DefaultHttpContext Context { get; } = new();
    public IHttpContextAccessor Accessor => new TestHttpContextAccessor { HttpContext = Context };
    internal HttpContextQueryCounter CreateCounter() => new(Accessor);
    internal HttpContextQueryCounter CreateCounterWithNullContext()
        => new(new TestHttpContextAccessor { HttpContext = null });

    private sealed class TestHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; }
    }
}
