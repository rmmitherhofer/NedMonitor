using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NedMonitor.Common.Tests;
using NedMonitor.Core;
using NedMonitor.Core.Enums;
using NedMonitor.Core.Helpers;
using NedMonitor.Core.Interfaces;
using NedMonitor.Core.Models;
using NedMonitor.Core.Settings;
using NedMonitor.Dapper.Interceptors;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit.Abstractions;

namespace NedMonitor.Dapper.Tests.Interceptors;

public class CountingDbConnectionTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given capture options none, " +
        "When executing with logging, " +
        "Then it increments but does not add query log")]
    [Trait("Interceptors", nameof(CountingDbConnection))]
    public async Task ExecuteWithLogging_CaptureNone_DoesNotLog()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.None]);
        var connection = CreateConnection(context, counter, settings);

        //When
        InvokeExecuteWithLogging(connection, () => 1, "select 1", new { id = 1 });

        //Then
        counter.Count.Should().Be(1);
        context.Items.Should().NotContainKey(NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given capture options empty, " +
        "When executing with logging, " +
        "Then it increments but does not add query log")]
    [Trait("Interceptors", nameof(CountingDbConnection))]
    public async Task ExecuteWithLogging_CaptureEmpty_DoesNotLog()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, []);
        var connection = CreateConnection(context, counter, settings);

        //When
        InvokeExecuteWithLogging(connection, () => 1, "select 1", new { id = 1 });

        //Then
        counter.Count.Should().Be(1);
        context.Items.Should().NotContainKey(NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given interceptor disabled, " +
        "When executing with logging, " +
        "Then it increments but does not add query log")]
    [Trait("Interceptors", nameof(CountingDbConnection))]
    public async Task ExecuteWithLogging_Disabled_DoesNotLog()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: false, [CaptureOptions.Query]);
        var connection = CreateConnection(context, counter, settings);

        //When
        InvokeExecuteWithLogging(connection, () => 1, "select 1", new { id = 1 });

        //Then
        counter.Count.Should().Be(1);
        context.Items.Should().NotContainKey(NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given capture options null, " +
        "When executing with logging, " +
        "Then it increments but does not add query log")]
    [Trait("Interceptors", nameof(CountingDbConnection))]
    public async Task ExecuteWithLogging_CaptureNull_DoesNotLog()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, null);
        var connection = CreateConnection(context, counter, settings);

        //When
        InvokeExecuteWithLogging(connection, () => 1, "select 1", new { id = 1 });

        //Then
        counter.Count.Should().Be(1);
        context.Items.Should().NotContainKey(NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given query and parameters capture, " +
        "When executing with logging, " +
        "Then it stores query entry")]
    [Trait("Interceptors", nameof(CountingDbConnection))]
    public async Task ExecuteWithLogging_CaptureQueryAndParameters_LogsEntry()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.Query, CaptureOptions.Parameters]);
        var connection = CreateConnection(context, counter, settings);
        var param = new ParametersHolder();

        //When
        InvokeExecuteWithLogging(connection, () => 1, "select 1", param);

        //Then
        counter.Count.Should().Be(1);
        var list = context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY]
            .Should().BeOfType<List<DbQueryEntry>>().Which;
        list.Should().HaveCount(1);
        list[0].Sql.Should().Be("select 1");
        list[0].Parameters.Should().Contain("id");
        list[0].Parameters.Should().Contain("123");
        list[0].Success.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given query capture only, " +
        "When executing with logging, " +
        "Then it stores sql without parameters")]
    [Trait("Interceptors", nameof(CountingDbConnection))]
    public async Task ExecuteWithLogging_CaptureQueryOnly_LogsSqlWithoutParameters()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.Query]);
        var connection = CreateConnection(context, counter, settings);

        //When
        InvokeExecuteWithLogging(connection, () => 1, "select 1", new { id = 1 });

        //Then
        var list = context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY]
            .Should().BeOfType<List<DbQueryEntry>>().Which;
        list.Should().HaveCount(1);
        list[0].Sql.Should().Be("select 1");
        list[0].Parameters.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given parameters capture and null params, " +
        "When executing with logging, " +
        "Then it stores empty parameters")]
    [Trait("Interceptors", nameof(CountingDbConnection))]
    public async Task ExecuteWithLogging_CaptureParameters_NullParams_StoresEmpty()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.Parameters]);
        var connection = CreateConnection(context, counter, settings);

        //When
        InvokeExecuteWithLogging(connection, () => 1, "select 1", null);

        //Then
        var list = context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY]
            .Should().BeOfType<List<DbQueryEntry>>().Which;
        list.Should().HaveCount(1);
        list[0].Parameters.Should().BeEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given context capture enabled, " +
        "When executing with logging, " +
        "Then it stores db context info")]
    [Trait("Interceptors", nameof(CountingDbConnection))]
    public async Task ExecuteWithLogging_CaptureContext_StoresDbContext()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.Context]);
        var connection = CreateConnection(context, counter, settings);

        //When
        InvokeExecuteWithLogging(connection, () => 1, "select 1", new { id = 1 });

        //Then
        var list = context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY]
            .Should().BeOfType<List<DbQueryEntry>>().Which;
        list.Should().HaveCount(1);
        list[0].DbContext.Should().NotBeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given exception capture enabled and no exception, " +
        "When executing with logging, " +
        "Then it stores empty exception message")]
    [Trait("Interceptors", nameof(CountingDbConnection))]
    public async Task ExecuteWithLogging_ExceptionCapture_NoException_StoresEmpty()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.ExceptionMessage]);
        var connection = CreateConnection(context, counter, settings);

        //When
        InvokeExecuteWithLogging(connection, () => 1, "select 1", new { });

        //Then
        var list = context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY]
            .Should().BeOfType<List<DbQueryEntry>>().Which;
        list.Should().HaveCount(1);
        list[0].ExceptionMessage.Should().BeNullOrEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given exception capture, " +
        "When executing with logging throws, " +
        "Then it stores exception message")]
    [Trait("Interceptors", nameof(CountingDbConnection))]
    public async Task ExecuteWithLogging_Exception_LogsMessage()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.ExceptionMessage]);
        var connection = CreateConnection(context, counter, settings);

        //When
        var act = () => InvokeExecuteWithLogging<int>(connection, () => throw new InvalidOperationException("boom"), "select 1", new { });

        //Then
        act.Should().Throw<InvalidOperationException>();
        counter.Count.Should().Be(1);
        var list = context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY]
            .Should().BeOfType<List<DbQueryEntry>>().Which;
        list.Should().HaveCount(1);
        list[0].Success.Should().BeFalse();
        list[0].ExceptionMessage.Should().Be("boom");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given no http context, " +
        "When executing with logging, " +
        "Then it increments without logging")]
    [Trait("Interceptors", nameof(CountingDbConnection))]
    public async Task ExecuteWithLogging_NoHttpContext_DoesNotLog()
    {
        //Given
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.Query]);
        var connection = CreateConnectionWithNullContext(counter, settings);

        //When
        InvokeExecuteWithLogging(connection, () => 1, "select 1", new { id = 1 });

        //Then
        counter.Count.Should().Be(1);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given async execution and query capture, " +
        "When executing with logging, " +
        "Then it stores query entry")]
    [Trait("Interceptors", nameof(CountingDbConnection))]
    public async Task ExecuteWithLoggingAsync_CaptureQuery_LogsEntry()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.Query]);
        var connection = CreateConnection(context, counter, settings);

        //When
        await InvokeExecuteWithLoggingAsync(connection, () => Task.FromResult(1), "select 1", new { id = 1 });

        //Then
        counter.Count.Should().Be(1);
        var list = context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY]
            .Should().BeOfType<List<DbQueryEntry>>().Which;
        list.Should().HaveCount(1);
        list[0].Sql.Should().Be("select 1");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given async execution throws and exception capture, " +
        "When executing with logging, " +
        "Then it stores exception message")]
    [Trait("Interceptors", nameof(CountingDbConnection))]
    public async Task ExecuteWithLoggingAsync_Exception_LogsMessage()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.ExceptionMessage]);
        var connection = CreateConnection(context, counter, settings);

        //When
        var act = () => InvokeExecuteWithLoggingAsync<int>(connection, () =>
            Task.FromException<int>(new InvalidOperationException("boom")), "select 1", new { });

        //Then
        await act.Should().ThrowAsync<InvalidOperationException>();
        counter.Count.Should().Be(1);
        var list = context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY]
            .Should().BeOfType<List<DbQueryEntry>>().Which;
        list.Should().HaveCount(1);
        list[0].Success.Should().BeFalse();
        list[0].ExceptionMessage.Should().Be("boom");
        await Task.CompletedTask;
    }

    private static NedMonitorSettings CreateSettings(bool enabled, List<CaptureOptions>? captureOptions)
    {
        return new NedMonitorSettings
        {
            DataInterceptors = new DataInterceptorsSettings
            {
                Dapper = new DapperInterceptorSettings
                {
                    Enabled = enabled,
                    CaptureOptions = captureOptions
                }
            }
        };
    }

    private static CountingDbConnection CreateConnection(
        DefaultHttpContext context,
        IQueryCounter counter,
        NedMonitorSettings settings)
    {
        var accessor = new HttpContextAccessor { HttpContext = context };
        var options = Options.Create(settings);
        return new CountingDbConnection(new FakeSqlConnection(), counter, accessor, options);
    }

    private static CountingDbConnection CreateConnectionWithNullContext(
        IQueryCounter counter,
        NedMonitorSettings settings)
    {
        var accessor = new HttpContextAccessor();
        var options = Options.Create(settings);
        return new CountingDbConnection(new FakeSqlConnection(), counter, accessor, options);
    }

    private static T InvokeExecuteWithLogging<T>(
        CountingDbConnection connection,
        Func<T> func,
        string sql,
        object? param)
    {
        var method = typeof(CountingDbConnection)
            .GetMethod("ExecuteWithLogging", BindingFlags.Instance | BindingFlags.NonPublic);
        var generic = method!.MakeGenericMethod(typeof(T));
        try
        {
            return (T)generic.Invoke(connection, [func, sql, param])!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            throw ex.InnerException;
        }
    }

    private static async Task<T> InvokeExecuteWithLoggingAsync<T>(
        CountingDbConnection connection,
        Func<Task<T>> func,
        string sql,
        object? param)
    {
        var method = typeof(CountingDbConnection)
            .GetMethod("ExecuteWithLoggingAsync", BindingFlags.Instance | BindingFlags.NonPublic);
        var generic = method!.MakeGenericMethod(typeof(T));
        try
        {
            var task = (Task<T>)generic.Invoke(connection, [func, sql, param])!;
            return await task;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            throw ex.InnerException;
        }
    }

    private sealed class FakeCounter : IQueryCounter
    {
        public int Count { get; private set; }
        public void Increment() => Count++;
        public int GetCount(HttpContext context) => Count;
        public void Reset(HttpContext context) => Count = 0;
    }

    private sealed class ParametersHolder
    {
        private readonly System.Collections.IDictionary parameters = new Dictionary<string, object?>
        {
            ["id"] = new ValueHolder(123)
        };
    }

    private sealed class ValueHolder
    {
        public ValueHolder(object value) => Value = value;
        public object Value { get; }
    }

    private sealed class FakeSqlConnection : DbConnection
    {
        [AllowNull]
        public override string ConnectionString { get; set; } = string.Empty;
        public override string Database => "Db";
        public override string DataSource => "Source";
        public override string ServerVersion => "1.0";
        public override ConnectionState State => ConnectionState.Closed;
        public override void ChangeDatabase(string databaseName) { }
        public override void Close() { }
        public override void Open() { }
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();
        protected override DbCommand CreateDbCommand() => throw new NotSupportedException();
    }
}
