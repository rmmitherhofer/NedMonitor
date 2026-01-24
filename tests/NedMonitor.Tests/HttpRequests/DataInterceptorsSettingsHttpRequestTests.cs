using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpRequests;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class DataInterceptorsSettingsHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given data interceptors settings, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(DataInterceptorsSettingsHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var settings = new DataInterceptorsSettingsHttpRequest
        {
            EF = new EfInterceptorSettingsHttpRequest(),
            Dapper = new DapperInterceptorSettingsHttpRequest()
        };

        //When
        var json = JsonSerializer.Serialize(settings);

        //Then
        json.Should().Contain("\"ef\"");
        json.Should().Contain("\"dapper\"");
        await Task.CompletedTask;
    }
}
