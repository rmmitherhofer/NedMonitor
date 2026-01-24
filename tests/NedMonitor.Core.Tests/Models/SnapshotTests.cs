using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using NedMonitor.Common.Tests;
using NedMonitor.Common.Tests.FakerFactory;
using NedMonitor.Core.Models;
using System.Text;
using Xunit.Abstractions;
using Zypher.Notifications.Messages;
using Microsoft.Extensions.Primitives;

namespace NedMonitor.Core.Tests.Models;

public class SnapshotTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given original status header, " +
        "When capturing snapshot, " +
        "Then it uses the original status code")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_UsesOriginalStatusCodeHeader()
    {
        //Given
        var context = CreateContext();
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.Headers["X-Original-Status-Code"] = "418";

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 10,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-10),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.StatusCode.Should().Be(418);
        snapshot.ResponseBody.Should().Be("response");
        snapshot.ResponseBodySize.Should().Be(123);
        snapshot.RequestHeaders.Should().ContainKey("X-Client-ID");
    }

    [Fact(DisplayName =
        "Given redirect to error page, " +
        "When capturing snapshot, " +
        "Then it extracts status from location")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_ExtractsStatusCodeFromLocation()
    {
        //Given
        var context = CreateContext();
        context.Response.StatusCode = StatusCodes.Status302Found;
        context.Response.Headers["Location"] = "/error/404?x=1";

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 10,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-10),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.StatusCode.Should().Be(404);
    }

    [Fact(DisplayName =
        "Given non-error redirect, " +
        "When capturing snapshot, " +
        "Then it keeps response status code")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_NonErrorRedirect_KeepsStatusCode()
    {
        //Given
        var context = CreateContext();
        context.Response.StatusCode = StatusCodes.Status302Found;
        context.Response.Headers["Location"] = "/home";

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.StatusCode.Should().Be(StatusCodes.Status302Found);
    }

    [Fact(DisplayName =
        "Given cache hit and query data, " +
        "When capturing snapshot, " +
        "Then it maps diagnostics fields")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_MapsDiagnosticsFields()
    {
        //Given
        var context = CreateContext();
        context.Items[NedMonitorConstants.CONTEXT_CACHEHIT_KEY] = true;
        context.Items[NedMonitorConstants.CONTEXT_QUERY_COUNT_KEY] = 3;
        var queryEntry = new DbQueryEntry { Provider = "X", ORM = "Y" };
        context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY] = new List<DbQueryEntry> { queryEntry };

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.CacheHit.Should().BeTrue();
        snapshot.DbQueryCount.Should().Be(3);
        snapshot.DbQueryEntries.Should().NotBeNull();
        snapshot.DbQueryEntries.Should().ContainSingle().Which.Should().BeSameAs(queryEntry);
    }

    [Fact(DisplayName =
        "Given http client logs, " +
        "When capturing snapshot, " +
        "Then it maps dependencies")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_MapsDependenciesFromHttpLogs()
    {
        //Given
        var context = CreateContext();
        var httpLog = new HttpRequestLogContext
        {
            StartTime = DateTime.UtcNow.AddMilliseconds(-10),
            EndTime = DateTime.UtcNow,
            FullUrl = "https://example.local/api",
            UrlTemplate = "/api",
            StatusCode = 200
        };
        context.Items[NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY] = new List<HttpRequestLogContext> { httpLog };

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 10,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-10),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.Dependencies.Should().NotBeNull();
        snapshot.Dependencies.Should().ContainSingle();
        snapshot.Dependencies!.First().Type.Should().Be("HTTP");
        snapshot.Dependencies!.First().Target.Should().Be("/api");
        snapshot.Dependencies!.First().Success.Should().BeTrue();
    }

    [Fact(DisplayName =
        "Given invalid original status code header, " +
        "When capturing snapshot, " +
        "Then it keeps response status code")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_InvalidOriginalStatusCode_KeepsStatusCode()
    {
        //Given
        var context = CreateContext();
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.Headers["X-Original-Status-Code"] = "abc";

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact(DisplayName =
        "Given error redirect with invalid status segment, " +
        "When capturing snapshot, " +
        "Then it keeps response status code")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_ErrorRedirectWithInvalidStatus_KeepsStatusCode()
    {
        //Given
        var context = CreateContext();
        context.Response.StatusCode = StatusCodes.Status302Found;
        context.Response.Headers["Location"] = "/error/abc";

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.StatusCode.Should().Be(StatusCodes.Status302Found);
    }

    [Fact(DisplayName =
        "Given empty request body, " +
        "When capturing snapshot, " +
        "Then request body is null")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_EmptyBody_ReturnsNull()
    {
        //Given
        var context = CreateContext();
        SetRequestBody(context, string.Empty);

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        var bodyTask = snapshot.RequestBody.Should().BeOfType<Task<object?>>().Which;
        var body = await bodyTask;
        body.Should().BeNull();
    }

    [Fact(DisplayName =
        "Given invalid json body, " +
        "When capturing snapshot, " +
        "Then request body is raw string")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_InvalidJsonBody_ReturnsRawString()
    {
        //Given
        var context = CreateContext();
        SetRequestBody(context, "not-json");

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        var bodyTask = snapshot.RequestBody.Should().BeOfType<Task<object?>>().Which;
        var body = await bodyTask;
        body.Should().Be("not-json");
    }

    [Fact(DisplayName =
        "Given form request with file, " +
        "When capturing snapshot, " +
        "Then it maps form fields and marks form content type")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_FormRequest_MapsFormFields()
    {
        //Given
        var context = CreateContext();
        context.Request.Method = "POST";
        context.Request.ContentType = "application/x-www-form-urlencoded";
        context.Request.Headers["Content-Type"] = context.Request.ContentType;
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("field=value"));
        context.Request.ContentLength = context.Request.Body.Length;
        var form = new FormCollection(new Dictionary<string, StringValues>
        {
            ["field"] = "value"
        });
        context.Features.Set<IFormFeature>(new TestFormFeature(form));

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        var bodyTask = snapshot.RequestBody.Should().BeOfType<Task<object?>>().Which;
        var body = await bodyTask;
        snapshot.HasFormContentType.Should().BeTrue();
        var bodyDict = body.Should().BeAssignableTo<IDictionary<string, object>>().Which;
        bodyDict.Should().ContainKey("field");
        bodyDict["field"].Should().Be("value");
    }

    [Fact(DisplayName =
        "Given multipart form content type, " +
        "When capturing snapshot, " +
        "Then it marks form content type")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_MultipartContentType_MarksFormContentType()
    {
        //Given
        var context = CreateContext();
        context.Request.Method = "POST";
        context.Request.ContentType = "multipart/form-data; boundary=----1234";
        context.Request.Headers["Content-Type"] = context.Request.ContentType;
        var form = new FormCollection(new Dictionary<string, StringValues>
        {
            ["field"] = "value"
        });
        context.Features.Set<IFormFeature>(new TestFormFeature(form));

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.HasFormContentType.Should().BeTrue();
    }

    [Fact(DisplayName =
        "Given urlencoded content type with charset, " +
        "When capturing snapshot, " +
        "Then it marks form content type")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_UrlEncodedWithCharset_MarksFormContentType()
    {
        //Given
        var context = CreateContext();
        context.Request.Method = "POST";
        context.Request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
        context.Request.Headers["Content-Type"] = context.Request.ContentType;
        var form = new FormCollection(new Dictionary<string, StringValues>
        {
            ["field"] = "value"
        });
        context.Features.Set<IFormFeature>(new TestFormFeature(form));

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.HasFormContentType.Should().BeTrue();
    }

    [Fact(DisplayName =
        "Given empty content type, " +
        "When capturing snapshot, " +
        "Then it does not mark form content type")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_EmptyContentType_DoesNotMarkFormContentType()
    {
        //Given
        var context = CreateContext();
        context.Request.Method = "POST";
        context.Request.ContentType = "";
        context.Request.Headers.Remove("Content-Type");

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.HasFormContentType.Should().BeFalse();
    }

    [Fact(DisplayName =
        "Given form request with multiple files, " +
        "When capturing snapshot, " +
        "Then it maps file metadata")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_FormRequestWithFiles_MapsFileMetadata()
    {
        //Given
        var context = CreateContext();
        context.Request.Method = "POST";
        context.Request.ContentType = "multipart/form-data; boundary=----1234";
        context.Request.Headers["Content-Type"] = context.Request.ContentType;

        var files = new FormFileCollection
        {
            CreateFormFile("fileA", "a.txt", "text/plain", "alpha"),
            CreateFormFile("fileB", "b.bin", "application/octet-stream", "beta")
        };

        var form = new FormCollection(new Dictionary<string, StringValues>
        {
            ["field"] = "value"
        }, files);
        context.Features.Set<IFormFeature>(new TestFormFeature(form));

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        var bodyTask = snapshot.RequestBody.Should().BeOfType<Task<object?>>().Which;
        var body = await bodyTask;
        var bodyDict = body.Should().BeAssignableTo<IDictionary<string, object>>().Which;
        bodyDict["Name_0"].Should().Be("fileA");
        bodyDict["ContentType_0"].Should().Be("text/plain");
        bodyDict["FileName_0"].Should().Be("a.txt");
        bodyDict["Name_1"].Should().Be("fileB");
        bodyDict["ContentType_1"].Should().Be("application/octet-stream");
        bodyDict["FileName_1"].Should().Be("b.bin");
    }

    [Fact(DisplayName =
        "Given multipart content type with casing differences, " +
        "When capturing snapshot, " +
        "Then it marks form content type")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_MultipartCasing_MarksFormContentType()
    {
        //Given
        var context = CreateContext();
        context.Request.Method = "POST";
        context.Request.ContentType = "Multipart/Form-Data; boundary=----1234";
        context.Request.Headers["Content-Type"] = context.Request.ContentType;
        var form = new FormCollection(new Dictionary<string, StringValues>
        {
            ["field"] = "value"
        });
        context.Features.Set<IFormFeature>(new TestFormFeature(form));

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.HasFormContentType.Should().BeTrue();
    }

    [Fact(DisplayName =
        "Given repeated form keys, " +
        "When capturing snapshot, " +
        "Then it concatenates values")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_RepeatedFormKeys_ConcatenatesValues()
    {
        //Given
        var context = CreateContext();
        context.Request.Method = "POST";
        context.Request.ContentType = "application/x-www-form-urlencoded";
        context.Request.Headers["Content-Type"] = context.Request.ContentType;
        var form = new FormCollection(new Dictionary<string, StringValues>
        {
            ["field"] = new StringValues(new[] { "1", "2" })
        });
        context.Features.Set<IFormFeature>(new TestFormFeature(form));

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        var bodyTask = snapshot.RequestBody.Should().BeOfType<Task<object?>>().Which;
        var body = await bodyTask;
        var bodyDict = body.Should().BeAssignableTo<IDictionary<string, object>>().Which;
        bodyDict["field"].Should().Be("1,2");
    }

    [Fact(DisplayName =
        "Given empty form values, " +
        "When capturing snapshot, " +
        "Then it keeps empty values")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_EmptyFormValues_KeepsEmpty()
    {
        //Given
        var context = CreateContext();
        context.Request.Method = "POST";
        context.Request.ContentType = "application/x-www-form-urlencoded";
        context.Request.Headers["Content-Type"] = context.Request.ContentType;
        var form = new FormCollection(new Dictionary<string, StringValues>
        {
            ["field"] = ""
        });
        context.Features.Set<IFormFeature>(new TestFormFeature(form));

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        var bodyTask = snapshot.RequestBody.Should().BeOfType<Task<object?>>().Which;
        var body = await bodyTask;
        var bodyDict = body.Should().BeAssignableTo<IDictionary<string, object>>().Which;
        bodyDict["field"].Should().Be(string.Empty);
    }

    [Fact(DisplayName =
        "Given form feature but non-form content type, " +
        "When capturing snapshot, " +
        "Then it does not read form")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_FormFeatureWithNonFormContentType_DoesNotReadForm()
    {
        //Given
        var context = CreateContext();
        context.Request.Method = "POST";
        context.Request.ContentType = "application/json";
        context.Request.Headers["Content-Type"] = context.Request.ContentType;
        SetRequestBody(context, "{\"key\":\"value\"}");
        var form = new FormCollection(new Dictionary<string, StringValues>
        {
            ["field"] = "value"
        });
        context.Features.Set<IFormFeature>(new TestFormFeature(form, hasFormContentType: false, throwOnRead: true));

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.HasFormContentType.Should().BeFalse();
        var bodyTask = snapshot.RequestBody.Should().BeOfType<Task<object?>>().Which;
        var body = await bodyTask;
        body.Should().NotBeNull();
    }

    [Fact(DisplayName =
        "Given file without content type, " +
        "When capturing snapshot, " +
        "Then it maps empty content type")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_FileWithoutContentType_MapsEmptyContentType()
    {
        //Given
        var context = CreateContext();
        context.Request.Method = "POST";
        context.Request.ContentType = "multipart/form-data; boundary=----1234";
        context.Request.Headers["Content-Type"] = context.Request.ContentType;

        var file = CreateFormFile("file", "file.txt", string.Empty, "content");
        var files = new FormFileCollection { file };
        var form = new FormCollection(new Dictionary<string, StringValues>
        {
            ["field"] = "value"
        }, files);
        context.Features.Set<IFormFeature>(new TestFormFeature(form));

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        var bodyTask = snapshot.RequestBody.Should().BeOfType<Task<object?>>().Which;
        var body = await bodyTask;
        var bodyDict = body.Should().BeAssignableTo<IDictionary<string, object>>().Which;
        bodyDict["ContentType_0"].Should().Be(string.Empty);
    }

    [Fact(DisplayName =
        "Given form reader throws, " +
        "When capturing snapshot, " +
        "Then the request body task throws")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_FormReadThrows_PropagatesException()
    {
        //Given
        var context = CreateContext();
        context.Request.Method = "POST";
        context.Request.ContentType = "application/x-www-form-urlencoded";
        context.Request.Headers["Content-Type"] = context.Request.ContentType;
        var form = new FormCollection(new Dictionary<string, StringValues>
        {
            ["field"] = "value"
        });
        context.Features.Set<IFormFeature>(new TestFormFeature(form, throwOnRead: true));

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        var bodyTask = snapshot.RequestBody.Should().BeOfType<Task<object?>>().Which;
        var act = async () => _ = await bodyTask;
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact(DisplayName =
        "Given notifications and exception in context, " +
        "When capturing snapshot, " +
        "Then it maps both values")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_MapsNotificationsAndException()
    {
        //Given
        var context = CreateContext();
        var notifications = Array.Empty<Notification>();
        var exceptionInfo = new ExceptionInfo { Type = "System.Exception", Message = "boom" };
        context.Items[NedMonitorConstants.CONTEXT_NOTIFICATIONS_KEY] = notifications;
        context.Items[NedMonitorConstants.CONTEXT_EXCEPTION_KEY] = exceptionInfo;

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.Notifications.Should().BeSameAs(notifications);
        snapshot.Exception.Should().BeSameAs(exceptionInfo);
    }

    [Fact(DisplayName =
        "Given failed http client log, " +
        "When capturing snapshot, " +
        "Then dependency is marked as failed")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_FailedHttpLog_MarksDependencyFailed()
    {
        //Given
        var context = CreateContext();
        var httpLog = new HttpRequestLogContext
        {
            StartTime = DateTime.UtcNow.AddMilliseconds(-10),
            EndTime = DateTime.UtcNow,
            FullUrl = "https://example.local/api",
            UrlTemplate = "/api",
            StatusCode = 500
        };
        context.Items[NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY] = new List<HttpRequestLogContext> { httpLog };

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 10,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-10),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.Dependencies.Should().ContainSingle();
        snapshot.Dependencies!.First().Success.Should().BeFalse();
    }

    [Fact(DisplayName =
        "Given route values and claims, " +
        "When capturing snapshot, " +
        "Then it maps route values and claims")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_MapsRouteValuesAndClaims()
    {
        //Given
        var context = CreateContext();
        var identity = new System.Security.Claims.ClaimsIdentity("test");
        identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Admin"));
        identity.AddClaim(new System.Security.Claims.Claim("custom", "value"));
        context.User = new System.Security.Claims.ClaimsPrincipal(identity);

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.RouteValues.Should().ContainKey("id");
        snapshot.Claims.Should().ContainKey("custom");
        snapshot.Roles.Should().Contain("Admin");
    }

    [Fact(DisplayName =
        "Given empty response headers, " +
        "When capturing snapshot, " +
        "Then it maps an empty collection")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_EmptyResponseHeaders_MapsEmpty()
    {
        //Given
        var context = CreateContext();
        context.Response.Headers.Clear();

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.ResponseHeaders.Should().NotBeNull();
        snapshot.ResponseHeaders.Should().BeEmpty();
    }

    [Fact(DisplayName =
        "Given remote ip address, " +
        "When capturing snapshot, " +
        "Then it uses the remote ip")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_RemoteIpAddress_MapsValue()
    {
        //Given
        var context = CreateContext();
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("10.0.0.1");
        context.Connection.LocalIpAddress = null;
        context.Request.Headers.Remove("X-Forwarded-For");

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.IpAddress.Should().Be("10.0.0.1");
    }

    [Fact(DisplayName =
        "Given http client log without template, " +
        "When capturing snapshot, " +
        "Then dependency target uses full url")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_HttpLogWithoutTemplate_UsesFullUrl()
    {
        //Given
        var context = CreateContext();
        var httpLog = new HttpRequestLogContext
        {
            StartTime = DateTime.UtcNow.AddMilliseconds(-10),
            EndTime = DateTime.UtcNow,
            FullUrl = "https://example.local/api",
            UrlTemplate = null,
            StatusCode = 200
        };
        context.Items[NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY] = new List<HttpRequestLogContext> { httpLog };

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 10,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-10),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.Dependencies.Should().ContainSingle();
        snapshot.Dependencies!.First().Target.Should().Be("https://example.local/api");
    }

    [Fact(DisplayName =
        "Given query logs without query count, " +
        "When capturing snapshot, " +
        "Then query count is zero")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_QueryLogsWithoutCount_UsesZero()
    {
        //Given
        var context = CreateContext();
        var queryEntry = new DbQueryEntry { Provider = "X", ORM = "Y" };
        context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY] = new List<DbQueryEntry> { queryEntry };

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.DbQueryCount.Should().Be(0);
        snapshot.DbQueryEntries.Should().ContainSingle();
    }

    [Fact(DisplayName =
        "Given notifications in context, " +
        "When capturing snapshot, " +
        "Then it maps notifications list")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_MapsNotificationsList()
    {
        //Given
        var context = CreateContext();
        var notifications = new List<Notification>
        {
            new Notification("key", "value")
        };
        context.Items[NedMonitorConstants.CONTEXT_NOTIFICATIONS_KEY] = notifications;

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.Notifications.Should().BeSameAs(notifications);
        snapshot.Notifications.Should().HaveCount(1);
    }

    [Fact(DisplayName =
        "Given forwarded header and remote ip, " +
        "When capturing snapshot, " +
        "Then it uses forwarded ip")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_UsesForwardedIp()
    {
        //Given
        var context = CreateContext();
        context.Request.Headers["X-Forwarded-For"] = "9.9.9.9";
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("10.0.0.1");

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.IpAddress.Should().Be("9.9.9.9");
    }

    [Fact(DisplayName =
        "Given response body size missing, " +
        "When capturing snapshot, " +
        "Then it defaults response body size to zero")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_MissingResponseBodySize_DefaultsToZero()
    {
        //Given
        var context = CreateContext();
        context.Items.Remove(NedMonitorConstants.CONTEXT_REPONSE_BODY_SIZE_KEY);

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.ResponseBodySize.Should().Be(0);
    }

    [Fact(DisplayName =
        "Given request cookies, " +
        "When capturing snapshot, " +
        "Then it maps cookie values")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_MapsRequestCookies()
    {
        //Given
        var context = CreateContext();
        context.Request.Headers["Cookie"] = "a=1; b=2";

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 5,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-5),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.RequestCookies.Should().ContainKey("a");
        snapshot.RequestCookies.Should().ContainKey("b");
        snapshot.RequestCookies!["a"].Should().Be("1");
        snapshot.RequestCookies!["b"].Should().Be("2");
    }

    [Fact(DisplayName =
        "Given http client log with exception, " +
        "When capturing snapshot, " +
        "Then dependency is marked as failed")]
    [Trait("Models", nameof(Snapshot))]
    public async Task CaptureAsync_HttpLogWithException_MarksDependencyFailed()
    {
        //Given
        var context = CreateContext();
        var httpLog = new HttpRequestLogContext
        {
            StartTime = DateTime.UtcNow.AddMilliseconds(-10),
            EndTime = DateTime.UtcNow,
            FullUrl = "https://example.local/api",
            UrlTemplate = "/api",
            StatusCode = 200,
            ExceptionType = "System.Exception"
        };
        context.Items[NedMonitorConstants.CONTEXT_HTTP_CLIENT_LOGS_KEY] = new List<HttpRequestLogContext> { httpLog };

        //When
        var snapshot = await new Snapshot().CaptureAsync(
            context,
            elapsedMs: 10,
            startTimeAt: DateTime.UtcNow.AddMilliseconds(-10),
            endTimeAt: DateTime.UtcNow);

        //Then
        snapshot.Dependencies.Should().ContainSingle();
        snapshot.Dependencies!.First().Success.Should().BeFalse();
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = HttpContextFaker.Create(
            userAgent: "ua",
            referer: "https://ref.local",
            forwardedFor: "1.1.1.1",
            correlationId: "corr",
            clientId: "client");

        context.TraceIdentifier = "trace";

        var request = context.Request;
        request.Method = "GET";
        request.Scheme = "https";
        request.Protocol = "HTTP/1.1";
        request.IsHttps = true;
        request.Path = "/test";
        request.PathBase = "";
        request.Host = new HostString("example.local");
        request.QueryString = new QueryString("?a=1");
        request.RouteValues = new RouteValueDictionary { ["id"] = "123" };
        request.ContentType = "application/json";

        SetRequestBody(context, "{\"key\":\"value\"}");

        context.Items[NedMonitorConstants.CONTEXT_REPONSE_BODY_KEY] = "response";
        context.Items[NedMonitorConstants.CONTEXT_REPONSE_BODY_SIZE_KEY] = 123L;

        return context;
    }

    private static void SetRequestBody(DefaultHttpContext context, string content)
    {
        var body = Encoding.UTF8.GetBytes(content);
        context.Request.Body = new MemoryStream(body);
        context.Request.ContentLength = body.Length;
    }

    private static IFormFile CreateFormFile(string name, string fileName, string contentType, string content)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var file = new FormFile(stream, 0, stream.Length, name, fileName)
        {
            Headers = new HeaderDictionary { ["Content-Type"] = contentType }
        };

        return file;
    }

    private sealed class TestFormFeature : IFormFeature
    {
        private IFormCollection _form;
        private readonly bool _throwOnRead;
        private readonly bool _hasFormContentType;

        public TestFormFeature(
            IFormCollection form,
            bool hasFormContentType = true,
            bool throwOnRead = false)
        {
            _form = form;
            _hasFormContentType = hasFormContentType;
            _throwOnRead = throwOnRead;
        }

        public IFormCollection Form
        {
            get => _form;
            set => _form = value;
        }

        public bool HasFormContentType => _hasFormContentType;

        public IFormCollection ReadForm()
        {
            if (_throwOnRead)
                throw new InvalidOperationException("Form read failed.");

            return Form;
        }

        public Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default)
        {
            if (_throwOnRead)
                throw new InvalidOperationException("Form read failed.");

            return Task.FromResult(Form);
        }
    }
}
