using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NedMonitor.Builders;
using NedMonitor.Common.Tests;
using NedMonitor.Core.Enums;
using NedMonitor.Core.Extensions;
using NedMonitor.Core.Models;
using NedMonitor.Core.Settings;
using NedMonitor.HttpResponses;
using System.Reflection;
using Xunit.Abstractions;

namespace NedMonitor.Tests.Builders;

public class LogContextBuilderTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given exception in snapshot, " +
        "When building log context, " +
        "Then it sets critical attention and error category")]
    [Trait("Builders", nameof(LogContextBuilder))]
    public async Task Build_WithException_SetsCriticalAndErrorCategory()
    {
        //Given
        var settings = CreateSettings(captureCookies: false, maskEnabled: false);
        var builder = CreateBuilder(settings);
        var snapshot = CreateSnapshot();
        snapshot.Exception = new ExceptionInfo
        {
            Type = "System.InvalidOperationException",
            Message = "boom",
            Timestamp = DateTime.UtcNow
        };

        //When
        var log = builder.WithSnapshot(snapshot).WithException().Build();

        //Then
        log.LogAttentionLevel.Should().Be(LogAttentionLevel.Critical);
        log.ErrorCategory.Should().Be("Error");
        log.Exceptions.Should().HaveCount(1);
        log.Exceptions!.First().Type.Should().Be("System.InvalidOperationException");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given notifications in snapshot, " +
        "When building log context, " +
        "Then it sets notification attention and category")]
    [Trait("Builders", nameof(LogContextBuilder))]
    public async Task Build_WithNotifications_SetsNotificationCategory()
    {
        //Given
        var settings = CreateSettings(captureCookies: false, maskEnabled: false);
        var builder = CreateBuilder(settings);
        var snapshot = CreateSnapshot();
        snapshot.Notifications =
        [
            CreateNotification(LogLevel.Warning)
        ];

        //When
        var log = builder.WithSnapshot(snapshot).WithNotifications().Build();

        //Then
        log.LogAttentionLevel.Should().Be(LogAttentionLevel.Medium);
        log.ErrorCategory.Should().Be("Notification");
        log.Notifications.Should().HaveCount(1);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given cookies capture enabled, " +
        "When building log context, " +
        "Then it masks cookies and maps uri")]
    [Trait("Builders", nameof(LogContextBuilder))]
    public async Task Build_CaptureCookies_MapsUriAndMasksCookies()
    {
        //Given
        var settings = CreateSettings(captureCookies: true, maskEnabled: true, maskValue: "***");
        var builder = CreateBuilder(settings);
        var snapshot = CreateSnapshot();
        snapshot.RequestCookies = new Dictionary<string, string>
        {
            ["token"] = "secret"
        };

        //When
        var log = builder.WithSnapshot(snapshot).Build();

        //Then
        log.Uri.Should().Be("https://example.com/api/test");
        log.UriTemplate.Should().Be("https://example.com/api/test");
        log.Request.Cookies.Should().ContainKey("token");
        log.Request.Cookies!["token"].Should().Be("***");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given cookies capture disabled, " +
        "When building log context, " +
        "Then cookies are null")]
    [Trait("Builders", nameof(LogContextBuilder))]
    public async Task Build_CookiesDisabled_ReturnsNull()
    {
        //Given
        var settings = CreateSettings(captureCookies: false, maskEnabled: true);
        var builder = CreateBuilder(settings);
        var snapshot = CreateSnapshot();
        snapshot.RequestCookies = new Dictionary<string, string>
        {
            ["token"] = "secret"
        };

        //When
        var log = builder.WithSnapshot(snapshot).Build();

        //Then
        log.Request.Cookies.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given task request body, " +
        "When building log context, " +
        "Then it resolves task result")]
    [Trait("Builders", nameof(LogContextBuilder))]
    public async Task Build_RequestBodyTask_ResolvesResult()
    {
        //Given
        var settings = CreateSettings(captureCookies: false, maskEnabled: false);
        var builder = CreateBuilder(settings);
        var snapshot = CreateSnapshot();
        snapshot.RequestBody = Task.FromResult<object>("payload");

        //When
        var log = builder.WithSnapshot(snapshot).Build();

        //Then
        log.Request.Body.Should().Be("payload");
        await Task.CompletedTask;
    }

    private static LogContextBuilder CreateBuilder(NedMonitorSettings settings)
    {
        var masker = new SensitiveDataMasker(settings.SensitiveDataMasking);
        return new LogContextBuilder(Options.Create(settings), masker);
    }

    private static NedMonitorSettings CreateSettings(bool captureCookies, bool maskEnabled, string? maskValue = null)
    {
        return new NedMonitorSettings
        {
            HttpLogging = new HttpLoggingSettings
            {
                CaptureCookies = captureCookies
            },
            SensitiveDataMasking = new SensitiveDataMaskerSettings
            {
                Enabled = maskEnabled,
                MaskValue = maskValue ?? "***",
                SensitiveKeys = ["token"]
            }
        };
    }

    private static Snapshot CreateSnapshot()
    {
        return new Snapshot
        {
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow,
            CorrelationId = "corr",
            Scheme = "https",
            Host = "example.com",
            Path = "/api/test",
            PathTemplate = "api/test",
            TotalMilliseconds = 10,
            TraceId = "trace",
            RemotePort = 1,
            LocalPort = 2,
            LocalIpAddress = "127.0.0.1",
            RequestId = "req",
            Method = "GET",
            PathBase = string.Empty,
            FullPath = "https://example.com/api/test",
            QueryString = string.Empty,
            RouteValues = new Dictionary<string, string>(),
            UserAgent = "agent",
            ClientId = "client",
            RequestHeaders = new Dictionary<string, List<string>>(),
            RequestContentType = "application/json",
            RequestContentLength = 0,
            IsAjaxRequest = false,
            IpAddress = "127.0.0.1",
            StatusCode = 200,
            ResponseHeaders = new Dictionary<string, List<string>>(),
            ResponseBody = null,
            ResponseBodySize = 0,
            ThreadId = 1,
            Roles = new List<string>(),
            Claims = new Dictionary<string, string>()
        };
    }

    private static Notification CreateNotification(LogLevel level)
    {
        var type = typeof(Notification);
        var ctor = type.GetConstructors()
            .FirstOrDefault(c => c.GetParameters().Any(p => p.ParameterType == typeof(LogLevel)));

        if (ctor is not null)
        {
            var args = ctor.GetParameters().Select(param =>
            {
                if (param.ParameterType == typeof(LogLevel)) return level;
                if (param.ParameterType == typeof(string)) return "value";
                return param.HasDefaultValue ? param.DefaultValue : null;
            }).ToArray();

            return (Notification)ctor.Invoke(args);
        }

        var notification = new Notification()
        {
            Key = "key",
            Value = "value",
        };
        var field = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .FirstOrDefault(f => f.FieldType == typeof(LogLevel));

        field?.SetValue(notification, level);
        return notification;
    }
}
