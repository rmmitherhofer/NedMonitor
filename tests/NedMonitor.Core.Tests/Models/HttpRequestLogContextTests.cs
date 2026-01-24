using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Common.Tests.FakerFactory.Models;
using NedMonitor.Core.Models;
using Xunit.Abstractions;

namespace NedMonitor.Core.Tests.Models;

public class HttpRequestLogContextTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given http log context, " +
        "When setting properties, " +
        "Then it stores values")]
    [Trait("Models", nameof(HttpRequestLogContext))]
    public async Task Properties_SetAndGet()
    {
        //Given
        var context = HttpRequestLogContextFaker.Create(
            method: "GET",
            fullUrl: "https://example.local/api",
            statusCode: 200,
            requestHeaders: new Dictionary<string, List<string>> { ["h"] = ["v"] },
            responseHeaders: new Dictionary<string, List<string>> { ["rh"] = ["rv"] },
            exceptionType: "System.Exception",
            exceptionMessage: "boom");

        //When
        var method = context.Method;

        //Then
        method.Should().Be("GET");
        context.FullUrl.Should().Be("https://example.local/api");
        context.StatusCode.Should().Be(200);
        context.RequestHeaders.Should().ContainKey("h");
        context.ResponseHeaders.Should().ContainKey("rh");
        context.ExceptionType.Should().Be("System.Exception");
        context.ExceptionMessage.Should().Be("boom");
        await Task.CompletedTask;
    }
}
