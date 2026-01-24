using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpServices;
using NedMonitor.Tests.HttpServices.Fixtures;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpServices;

public class NedMonitorHttpServiceTests(
    ITestOutputHelper output,
    NedMonitorHttpServiceTestsFixture fixture)
    : Test(output), IClassFixture<NedMonitorHttpServiceTestsFixture>
{
    private readonly NedMonitorHttpServiceTestsFixture _fixture = fixture;

    [Fact(DisplayName =
        "Given a log context, " +
        "When flushing, " +
        "Then it sends request with default headers")]
    [Trait("HttpServices", nameof(NedMonitorHttpService))]
    public async Task Flush_SendsRequest_WithDefaultHeaders()
    {
        //Given
        var settings = _fixture.CreateSettings();
        var log = _fixture.CreateLog(
            correlationId: "corr-1",
            clientId: "client-1",
            ipAddress: "10.0.0.1",
            userId: "user-1",
            userAgent: "agent-1",
            account: "acc-1",
            accountCode: "acc-code-1");

        var (service, handler) = _fixture.CreateService(settings);
        var expectedClientId = $"client-1;{Assembly.GetEntryAssembly()!.GetName().Name}";

        //When
        await service.Flush(log);

        //Then
        handler.LastRequest.Should().NotBeNull();
        handler.LastRequest!.Method.Should().Be(HttpMethod.Post);
        handler.LastRequest!.RequestUri!.ToString().Should().Be("https://example.local/logs");

        var headerValues = handler.LastRequest.Headers.SelectMany(h => h.Value).ToList();
        headerValues.Should().Contain("10.0.0.1");
        headerValues.Should().Contain("user-1");
        headerValues.Should().Contain("corr-1");
        headerValues.Should().Contain(expectedClientId);
        headerValues.Should().Contain("agent-1");
        headerValues.Should().Contain("acc-1");
        headerValues.Should().Contain("acc-code-1");
    }

    [Fact(DisplayName =
        "Given an error response, " +
        "When flushing, " +
        "Then it does not throw")]
    [Trait("HttpServices", nameof(NedMonitorHttpService))]
    public async Task Flush_ResponseHasErrors_DoesNotThrow()
    {
        //Given
        var settings = _fixture.CreateSettings();
        var log = _fixture.CreateLog();
        var (service, _) = _fixture.CreateService(settings, _ => new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("not-json")
        });

        //When
        var act = () => service.Flush(log);

        //Then
        await act.Should().NotThrowAsync();
    }

    [Fact(DisplayName =
        "Given null client id, " +
        "When flushing, " +
        "Then it uses assembly name as client id")]
    [Trait("HttpServices", nameof(NedMonitorHttpService))]
    public async Task Flush_NullClientId_UsesAssemblyName()
    {
        //Given
        var settings = _fixture.CreateSettings();
        var log = _fixture.CreateLog(clientId: null);
        var (service, handler) = _fixture.CreateService(settings);
        var assemblyName = Assembly.GetEntryAssembly()!.GetName().Name!;

        //When
        await service.Flush(log);

        //Then
        var headerValues = handler.LastRequest!.Headers.SelectMany(h => h.Value).ToList();
        headerValues.Should().Contain(assemblyName);
        headerValues.Should().NotContain($"{assemblyName};");
    }

    [Fact(DisplayName =
        "Given empty client id, " +
        "When flushing, " +
        "Then it uses assembly name as client id")]
    [Trait("HttpServices", nameof(NedMonitorHttpService))]
    public async Task Flush_EmptyClientId_UsesAssemblyName()
    {
        //Given
        var settings = _fixture.CreateSettings();
        var log = _fixture.CreateLog(clientId: string.Empty);
        var (service, handler) = _fixture.CreateService(settings);
        var assemblyName = Assembly.GetEntryAssembly()!.GetName().Name!;

        //When
        await service.Flush(log);

        //Then
        var headerValues = handler.LastRequest!.Headers.SelectMany(h => h.Value).ToList();
        headerValues.Should().Contain(assemblyName);
        headerValues.Should().NotContain("client-id");
    }

    [Fact(DisplayName =
        "Given null user id, " +
        "When flushing, " +
        "Then it does not include user id header")]
    [Trait("HttpServices", nameof(NedMonitorHttpService))]
    public async Task Flush_NullUserId_DoesNotIncludeHeader()
    {
        //Given
        var settings = _fixture.CreateSettings();
        var log = _fixture.CreateLog(userId: null);
        var (service, handler) = _fixture.CreateService(settings);

        //When
        await service.Flush(log);

        //Then
        var headerValues = handler.LastRequest!.Headers.SelectMany(h => h.Value).ToList();
        headerValues.Should().NotContain("user-id");
    }

    [Fact(DisplayName =
        "Given null ip address, " +
        "When flushing, " +
        "Then it does not include ip address header")]
    [Trait("HttpServices", nameof(NedMonitorHttpService))]
    public async Task Flush_NullIpAddress_DoesNotIncludeHeader()
    {
        //Given
        var settings = _fixture.CreateSettings();
        var log = _fixture.CreateLog(ipAddress: null);
        var (service, handler) = _fixture.CreateService(settings);

        //When
        await service.Flush(log);

        //Then
        var headerValues = handler.LastRequest!.Headers.SelectMany(h => h.Value).ToList();
        headerValues.Should().NotContain("10.0.0.1");
    }

    [Fact(DisplayName =
        "Given empty correlation id, " +
        "When flushing, " +
        "Then it does not include correlation id header")]
    [Trait("HttpServices", nameof(NedMonitorHttpService))]
    public async Task Flush_EmptyCorrelationId_DoesNotIncludeHeader()
    {
        //Given
        var settings = _fixture.CreateSettings();
        var log = _fixture.CreateLog(correlationId: string.Empty);
        var (service, handler) = _fixture.CreateService(settings);

        //When
        await service.Flush(log);

        //Then
        var headerValues = handler.LastRequest!.Headers.SelectMany(h => h.Value).ToList();
        headerValues.Should().NotContain("corr-id");
    }

    [Fact(DisplayName =
        "Given null user agent, " +
        "When flushing, " +
        "Then it does not include user agent header")]
    [Trait("HttpServices", nameof(NedMonitorHttpService))]
    public async Task Flush_NullUserAgent_DoesNotIncludeHeader()
    {
        //Given
        var settings = _fixture.CreateSettings();
        var log = _fixture.CreateLog(userAgent: null);
        var (service, handler) = _fixture.CreateService(settings);

        //When
        await service.Flush(log);

        //Then
        var headerValues = handler.LastRequest!.Headers.SelectMany(h => h.Value).ToList();
        headerValues.Should().NotContain("agent-1");
    }

    [Fact(DisplayName =
        "Given null account and account code, " +
        "When flushing, " +
        "Then it does not include account headers")]
    [Trait("HttpServices", nameof(NedMonitorHttpService))]
    public async Task Flush_NullAccountFields_DoesNotIncludeHeader()
    {
        //Given
        var settings = _fixture.CreateSettings();
        var log = _fixture.CreateLog(account: null, accountCode: null);
        var (service, handler) = _fixture.CreateService(settings);

        //When
        await service.Flush(log);

        //Then
        var headerValues = handler.LastRequest!.Headers.SelectMany(h => h.Value).ToList();
        headerValues.Should().NotContain("acc-1");
        headerValues.Should().NotContain("acc-code-1");
    }
}
