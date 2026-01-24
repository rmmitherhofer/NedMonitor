using Microsoft.AspNetCore.Http;
using NedMonitor.Common.Tests.FakerFactory;
using NedMonitor.Core.Models;
using NedMonitor.Queues;
using System.Net;
using System.Text;
using System.Threading.Channels;
using Moq;

namespace NedMonitor.Tests.Middleware.Fixtures;

public sealed class NedMonitorMiddlewareTestsFixture
{
    public DefaultHttpContext CreateContext(string body = "{}")
    {
        var context = HttpContextFaker.Create() ?? new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Scheme = "http";
        context.Request.Protocol = "HTTP/1.1";
        context.Request.Path = "/api/test";
        context.Request.Host = new HostString("example.local");
        context.Request.ContentType = "application/json";
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        context.Response.Body = new MemoryStream();
        context.Connection.LocalIpAddress = IPAddress.Loopback;
        context.Connection.LocalPort = 1234;
        context.Connection.RemotePort = 5678;

        return context;
    }

    public Mock<INedMonitorQueue> CreateQueueMock()
    {
        var channel = Channel.CreateUnbounded<Snapshot>();
        var mock = new Mock<INedMonitorQueue>();
        mock.SetupGet(q => q.Reader).Returns(channel.Reader);
        return mock;
    }
}
