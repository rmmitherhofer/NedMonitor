using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Enums;
using NedMonitor.HttpRequests;
using System.Collections.Generic;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class ProjectInfoHttpTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given project info, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(ProjectInfoHttp))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var info = new ProjectInfoHttp
        {
            Id = Guid.NewGuid(),
            Type = ProjectType.Api,
            Name = "NedMonitor",
            ExecutionMode = new ExecutionModeSettingsHttpRequest(),
            HttpLogging = new HttpLoggingSettingsHttpRequest(),
            SensitiveDataMasking = new SensitiveDataMaskerSettingsHttpRequest
            {
                SensitiveKeys = ["token"]
            },
            Exceptions = new ExceptionsSettingsHttpRequest
            {
                Expected = ["System.Exception"]
            },
            DataInterceptors = new DataInterceptorsSettingsHttpRequest
            {
                EF = new EfInterceptorSettingsHttpRequest(),
                Dapper = new DapperInterceptorSettingsHttpRequest()
            },
            MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information
        };

        //When
        var json = JsonSerializer.Serialize(info);

        //Then
        json.Should().Contain("\"executionMode\"");
        json.Should().Contain("\"httpLogging\"");
        json.Should().Contain("\"sensitiveDataMasking\"");
        json.Should().Contain("\"exceptions\"");
        json.Should().Contain("\"dataInterceptors\"");
        json.Should().Contain("\"minimumLogLevel\"");
        await Task.CompletedTask;
    }
}
