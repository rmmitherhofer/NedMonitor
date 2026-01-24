using Microsoft.AspNetCore.Http;
using NedMonitor.Core.Interfaces;

namespace NedMonitor.Core.Tests.Middlewares.Fixtures;

public sealed class QueryCounterResetMiddlewareTestsFixture
{
    public DefaultHttpContext Context { get; } = new();
    public List<string> Calls { get; } = [];

    public IQueryCounter CreateCounter() => new TestQueryCounter(Calls);

    public RequestDelegate CreateNext() => context =>
    {
        Calls.Add("next");
        return Task.CompletedTask;
    };

    private sealed class TestQueryCounter(List<string> calls) : IQueryCounter
    {
        private readonly List<string> _calls = calls;

        public void Increment() => _calls.Add("increment");

        public int GetCount(HttpContext context) => 0;

        public void Reset(HttpContext context) => _calls.Add("reset");
    }
}
