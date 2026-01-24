using FluentAssertions;
using NedMonitor.Common.Tests;
using NedMonitor.HttpRequests;
using System.Collections.Generic;
using System.Text.Json;
using Xunit.Abstractions;

namespace NedMonitor.Tests.HttpRequests;

public class UserInfoHttpRequestTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given user info, " +
        "When serializing to json, " +
        "Then it uses the expected property names")]
    [Trait("HttpRequests", nameof(UserInfoHttpRequest))]
    public async Task Serialize_UsesExpectedPropertyNames()
    {
        //Given
        var user = new UserInfoHttpRequest
        {
            Id = "user",
            Name = "User",
            Document = "doc",
            AccountCode = "acc-code",
            Account = "acc",
            Email = "user@example.local",
            TenantId = "tenant",
            IsAuthenticated = true,
            AuthenticationType = "jwt",
            Roles = ["role"],
            Claims = new Dictionary<string, string>
            {
                ["claim"] = "value"
            }
        };

        //When
        var json = JsonSerializer.Serialize(user);

        //Then
        json.Should().Contain("\"accountCode\"");
        json.Should().Contain("\"isAuthenticated\"");
        json.Should().Contain("\"authenticationType\"");
        json.Should().Contain("\"claims\"");
        await Task.CompletedTask;
    }
}
