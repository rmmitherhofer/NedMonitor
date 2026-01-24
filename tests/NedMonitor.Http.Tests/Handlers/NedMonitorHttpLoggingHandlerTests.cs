using FluentAssertions;
using NedMonitor.Http.Tests.Handlers.Fixtures;
using NedMonitor.Core;
using NedMonitor.Core.Models;
using NedMonitor.Http.Handlers;
using System.Net;
using System.Net.Http.Headers;
using Xunit.Abstractions;
using Zypher.Http.Extensions;
using NedMonitor.Common.Tests;

namespace NedMonitor.Http.Tests.Handlers;

public class NedMonitorHttpLoggingHandlerTests(
    ITestOutputHelper output,
    NedMonitorHttpLoggingHandlerTestsFixture fixture)
    : Test(output), IClassFixture<NedMonitorHttpLoggingHandlerTestsFixture>
{
    private readonly NedMonitorHttpLoggingHandlerTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given monitoring disabled, " +
        "When sending request, " +
        "Then it does not store logs")]
    [Trait("Handlers", nameof(NedMonitorHttpLoggingHandler))]
    public async Task SendAsync_Disabled_DoesNotStoreLogs()
    {
        //Given
        _fixture.Context.Items.Clear();
        var settings = _fixture.CreateSettings(enabled: false);
        var inner = _fixture.CreateInnerHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var handler = _fixture.CreateHandler(settings, inner);
        var client = new HttpClient(handler);

        //When
        await client.GetAsync("https://example.local/api");

        //Then
        _fixture.Context.Items.Should().NotContainKey(NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY);
    }

    [Fact(DisplayName =
        "Given monitoring enabled, " +
        "When sending request, " +
        "Then it stores log entry with request and response data")]
    [Trait("Handlers", nameof(NedMonitorHttpLoggingHandler))]
    public async Task SendAsync_Enabled_StoresLogEntry()
    {
        //Given
        _fixture.Context.Items.Clear();
        var settings = _fixture.CreateSettings(enabled: true);
        var inner = _fixture.CreateInnerHandler(_ =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                Content = new StringContent("ok")
            };
            response.Headers.Add("X-Resp", "1");
            return response;
        });
        var handler = _fixture.CreateHandler(settings, inner);
        var client = new HttpClient(handler);
        var request = new HttpRequestMessage(HttpMethod.Post, "https://example.local/api");
        request.Headers.Add(HttpRequestMessageExtensions.X_REQUEST_TEMPLATE, "/template");
        request.Headers.Add("X-Test", "1");
        request.Content = new StringContent("payload");
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

        //When
        await client.SendAsync(request);

        //Then
        inner.SawTemplateHeader.Should().BeFalse();
        var list = _fixture.Context.Items[NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY]
            .Should().BeOfType<List<HttpRequestLogContext>>().Which;
        list.Should().HaveCount(1);
        list[0].Method.Should().Be("POST");
        list[0].FullUrl.Should().Be("https://example.local/api");
        list[0].UrlTemplate.Should().Be("/template");
        list[0].StatusCode.Should().Be((int)HttpStatusCode.Accepted);
        list[0].RequestHeaders.Should().ContainKey("X-Test");
        list[0].ResponseHeaders.Should().ContainKey("X-Resp");
        list[0].RequestBody.Should().Be("payload");
        list[0].ResponseBody.Should().Be("ok");
    }

    [Fact(DisplayName =
        "Given response body exceeds limit, " +
        "When sending request, " +
        "Then it stores the truncation message")]
    [Trait("Handlers", nameof(NedMonitorHttpLoggingHandler))]
    public async Task SendAsync_ResponseBodyTooLarge_StoresMessage()
    {
        //Given
        _fixture.Context.Items.Clear();
        var settings = _fixture.CreateSettings(enabled: true, maxResponseBodySizeInMb: 0);
        var inner = _fixture.CreateInnerHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("abc")
            });
        var handler = _fixture.CreateHandler(settings, inner);
        var client = new HttpClient(handler);

        //When
        await client.GetAsync("https://example.local/api");

        //Then
        var list = _fixture.Context.Items[NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY]
            .Should().BeOfType<List<HttpRequestLogContext>>().Which;
        list.Should().HaveCount(1);
        list[0].ResponseBody.Should().Contain("exceeds limit of 0MB");
    }

    [Fact(DisplayName =
        "Given inner handler throws, " +
        "When sending request, " +
        "Then it rethrows and stores exception info")]
    [Trait("Handlers", nameof(NedMonitorHttpLoggingHandler))]
    public async Task SendAsync_Throws_StoresExceptionInfo()
    {
        //Given
        _fixture.Context.Items.Clear();
        var settings = _fixture.CreateSettings(enabled: true);
        var exception = new InvalidOperationException("boom");
        var inner = _fixture.CreateThrowingInnerHandler(exception);
        var handler = _fixture.CreateHandler(settings, inner);
        var client = new HttpClient(handler);

        //When
        var act = () => client.GetAsync("https://example.local/api");

        //Then
        await act.Should().ThrowAsync<InvalidOperationException>();
        var list = _fixture.Context.Items[NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY]
            .Should().BeOfType<List<HttpRequestLogContext>>().Which;
        list.Should().HaveCount(1);
        list[0].ExceptionType.Should().Be(typeof(InvalidOperationException).FullName);
        list[0].ExceptionMessage.Should().Be("boom");
    }

    [Fact(DisplayName =
        "Given null HttpContext accessor, " +
        "When sending request, " +
        "Then it does not store logs")]
    [Trait("Handlers", nameof(NedMonitorHttpLoggingHandler))]
    public async Task SendAsync_NullHttpContext_DoesNotStoreLogs()
    {
        //Given
        var settings = _fixture.CreateSettings(enabled: true);
        var inner = _fixture.CreateInnerHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var handler = _fixture.CreateHandlerWithNullContext(settings, inner);
        var client = new HttpClient(handler);

        //When
        await client.GetAsync("https://example.local/api");

        //Then
        _fixture.Context.Items.Should().NotContainKey(NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY);
    }

    [Fact(DisplayName =
        "Given request without content, " +
        "When sending request, " +
        "Then it stores null request body")]
    [Trait("Handlers", nameof(NedMonitorHttpLoggingHandler))]
    public async Task SendAsync_RequestWithoutContent_StoresNullBody()
    {
        //Given
        _fixture.Context.Items.Clear();
        var settings = _fixture.CreateSettings(enabled: true);
        var inner = _fixture.CreateInnerHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var handler = _fixture.CreateHandler(settings, inner);
        var client = new HttpClient(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.local/api");

        //When
        await client.SendAsync(request);

        //Then
        var list = _fixture.Context.Items[NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY]
            .Should().BeOfType<List<HttpRequestLogContext>>().Which;
        list.Should().HaveCount(1);
        list[0].RequestBody.Should().BeNull();
    }

    [Fact(DisplayName =
        "Given response without content, " +
        "When sending request, " +
        "Then it stores null response body")]
    [Trait("Handlers", nameof(NedMonitorHttpLoggingHandler))]
    public async Task SendAsync_ResponseWithoutContent_StoresNullBody()
    {
        //Given
        _fixture.Context.Items.Clear();
        var settings = _fixture.CreateSettings(enabled: true);
        var inner = _fixture.CreateInnerHandler(_ => new HttpResponseMessage(HttpStatusCode.NoContent));
        var handler = _fixture.CreateHandler(settings, inner);
        var client = new HttpClient(handler);

        //When
        await client.GetAsync("https://example.local/api");

        //Then
        var list = _fixture.Context.Items[NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY]
            .Should().BeOfType<List<HttpRequestLogContext>>().Which;
        list.Should().HaveCount(1);
        list[0].ResponseBody.Should().BeNullOrEmpty();
    }

    [Fact(DisplayName =
        "Given content headers on request, " +
        "When sending request, " +
        "Then it merges request headers")]
    [Trait("Handlers", nameof(NedMonitorHttpLoggingHandler))]
    public async Task SendAsync_RequestHeaders_MergesContentHeaders()
    {
        //Given
        _fixture.Context.Items.Clear();
        var settings = _fixture.CreateSettings(enabled: true);
        var inner = _fixture.CreateInnerHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var handler = _fixture.CreateHandler(settings, inner);
        var client = new HttpClient(handler);
        var request = new HttpRequestMessage(HttpMethod.Post, "https://example.local/api")
        {
            Content = new StringContent("payload")
        };
        request.Headers.Add("X-Test", "1");
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

        //When
        await client.SendAsync(request);

        //Then
        var list = _fixture.Context.Items[NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY]
            .Should().BeOfType<List<HttpRequestLogContext>>().Which;
        list.Should().HaveCount(1);
        list[0].RequestHeaders.Should().ContainKey("X-Test");
        list[0].RequestHeaders.Should().ContainKey("Content-Type");
    }

    [Fact(DisplayName =
        "Given response content headers, " +
        "When sending request, " +
        "Then it merges response headers")]
    [Trait("Handlers", nameof(NedMonitorHttpLoggingHandler))]
    public async Task SendAsync_ResponseHeaders_MergesContentHeaders()
    {
        //Given
        _fixture.Context.Items.Clear();
        var settings = _fixture.CreateSettings(enabled: true);
        var inner = _fixture.CreateInnerHandler(_ =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("ok")
            };
            response.Headers.Add("X-Resp", "1");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return response;
        });
        var handler = _fixture.CreateHandler(settings, inner);
        var client = new HttpClient(handler);

        //When
        await client.GetAsync("https://example.local/api");

        //Then
        var list = _fixture.Context.Items[NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY]
            .Should().BeOfType<List<HttpRequestLogContext>>().Which;
        list.Should().HaveCount(1);
        list[0].ResponseHeaders.Should().ContainKey("X-Resp");
        list[0].ResponseHeaders.Should().ContainKey("Content-Type");
    }

    [Fact(DisplayName =
        "Given monitoring disabled for http requests, " +
        "When sending request, " +
        "Then it does not store logs")]
    [Trait("Handlers", nameof(NedMonitorHttpLoggingHandler))]
    public async Task SendAsync_MonitorHttpDisabled_DoesNotStoreLogs()
    {
        //Given
        _fixture.Context.Items.Clear();
        var settings = _fixture.CreateSettings(enabled: true);
        settings.ExecutionMode.EnableMonitorHttpRequests = false;
        var inner = _fixture.CreateInnerHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var handler = _fixture.CreateHandler(settings, inner);
        var client = new HttpClient(handler);

        //When
        await client.GetAsync("https://example.local/api");

        //Then
        _fixture.Context.Items.Should().NotContainKey(NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY);
    }

    [Fact(DisplayName =
        "Given no template header, " +
        "When sending request, " +
        "Then it stores null template")]
    [Trait("Handlers", nameof(NedMonitorHttpLoggingHandler))]
    public async Task SendAsync_NoTemplateHeader_StoresNullTemplate()
    {
        //Given
        _fixture.Context.Items.Clear();
        var settings = _fixture.CreateSettings(enabled: true);
        var inner = _fixture.CreateInnerHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var handler = _fixture.CreateHandler(settings, inner);
        var client = new HttpClient(handler);

        //When
        await client.GetAsync("https://example.local/api");

        //Then
        var list = _fixture.Context.Items[NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY]
            .Should().BeOfType<List<HttpRequestLogContext>>().Which;
        list.Should().HaveCount(1);
        list[0].UrlTemplate.Should().BeNull();
    }

    [Fact(DisplayName =
        "Given template header, " +
        "When sending request, " +
        "Then it does not include it in request headers")]
    [Trait("Handlers", nameof(NedMonitorHttpLoggingHandler))]
    public async Task SendAsync_TemplateHeader_RemovedFromRequestHeaders()
    {
        //Given
        _fixture.Context.Items.Clear();
        var settings = _fixture.CreateSettings(enabled: true);
        var inner = _fixture.CreateInnerHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var handler = _fixture.CreateHandler(settings, inner);
        var client = new HttpClient(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.local/api");
        request.Headers.Add(HttpRequestMessageExtensions.X_REQUEST_TEMPLATE, "/template");

        //When
        await client.SendAsync(request);

        //Then
        var list = _fixture.Context.Items[NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY]
            .Should().BeOfType<List<HttpRequestLogContext>>().Which;
        list.Should().HaveCount(1);
        list[0].RequestHeaders.Should().NotContainKey(HttpRequestMessageExtensions.X_REQUEST_TEMPLATE);
    }
}
