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
using NedMonitor.EF.Interceptors;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit.Abstractions;

namespace NedMonitor.EF.Tests.Interceptors;

public class EfQueryCounterTests(ITestOutputHelper output)
    : Test(output)
{
    [Fact(DisplayName =
        "Given capture options none, " +
        "When adding log, " +
        "Then it does not store entries")]
    [Trait("Interceptors", nameof(EfQueryCounter))]
    public async Task AddLog_CaptureNone_DoesNotLog()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.None]);
        var interceptor = CreateInterceptor(context, counter, settings);
        var command = new FakeDbCommand();

        //When
        InvokeAddLog(interceptor, command, true, null, 1);

        //Then
        context.Items.Should().NotContainKey(NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given interceptor disabled, " +
        "When adding log, " +
        "Then it does not store entries")]
    [Trait("Interceptors", nameof(EfQueryCounter))]
    public async Task AddLog_Disabled_DoesNotLog()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: false, [CaptureOptions.Query]);
        var interceptor = CreateInterceptor(context, counter, settings);
        var command = new FakeDbCommand();

        //When
        InvokeAddLog(interceptor, command, true, null, 1);

        //Then
        context.Items.Should().NotContainKey(NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given query and parameters capture, " +
        "When adding log, " +
        "Then it stores query entry")]
    [Trait("Interceptors", nameof(EfQueryCounter))]
    public async Task AddLog_CaptureQueryAndParameters_StoresEntry()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.Query, CaptureOptions.Parameters]);
        var interceptor = CreateInterceptor(context, counter, settings);
        var command = new FakeDbCommand
        {
            CommandText = "select [Id] from [Users]"
        };
        command.Parameters.Add(new FakeDbParameter("p1", "text"));
        command.Parameters.Add(new FakeDbParameter("p2", 123));

        //When
        InvokeAddLog(interceptor, command, true, null, 10);

        //Then
        var list = context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY]
            .Should().BeOfType<List<DbQueryEntry>>().Which;
        list.Should().HaveCount(1);
        list[0].Sql.Should().Be("select Id from Users");
        list[0].Parameters.Should().Contain("p1=\"text\"");
        list[0].Parameters.Should().Contain("p2=123");
        list[0].Success.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given query capture only, " +
        "When adding log, " +
        "Then it stores sql without parameters")]
    [Trait("Interceptors", nameof(EfQueryCounter))]
    public async Task AddLog_CaptureQueryOnly_StoresSqlWithoutParameters()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.Query]);
        var interceptor = CreateInterceptor(context, counter, settings);
        var command = new FakeDbCommand
        {
            CommandText = "select 1"
        };
        command.Parameters.Add(new FakeDbParameter("p1", "text"));

        //When
        InvokeAddLog(interceptor, command, true, null, 1);

        //Then
        var list = context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY]
            .Should().BeOfType<List<DbQueryEntry>>().Which;
        list.Should().HaveCount(1);
        list[0].Sql.Should().Be("select 1");
        list[0].Parameters.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given parameters capture and no parameters, " +
        "When adding log, " +
        "Then it stores empty parameters")]
    [Trait("Interceptors", nameof(EfQueryCounter))]
    public async Task AddLog_CaptureParameters_NoParameters_StoresEmpty()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.Parameters]);
        var interceptor = CreateInterceptor(context, counter, settings);
        var command = new FakeDbCommand();

        //When
        InvokeAddLog(interceptor, command, true, null, 1);

        //Then
        var list = context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY]
            .Should().BeOfType<List<DbQueryEntry>>().Which;
        list.Should().HaveCount(1);
        list[0].Parameters.Should().BeEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given no http context, " +
        "When adding log, " +
        "Then it does not store entries")]
    [Trait("Interceptors", nameof(EfQueryCounter))]
    public async Task AddLog_NoHttpContext_DoesNotLog()
    {
        //Given
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.Query]);
        var interceptor = CreateInterceptorWithNullContext(counter, settings);
        var command = new FakeDbCommand();

        //When
        var act = () => InvokeAddLog(interceptor, command, true, null, 1);

        //Then
        act.Should().NotThrow();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given null duration, " +
        "When adding log, " +
        "Then it stores zero duration")]
    [Trait("Interceptors", nameof(EfQueryCounter))]
    public async Task AddLog_NullDuration_StoresZero()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.Query]);
        var interceptor = CreateInterceptor(context, counter, settings);
        var command = new FakeDbCommand();

        //When
        InvokeAddLog(interceptor, command, true, null, null);

        //Then
        var list = context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY]
            .Should().BeOfType<List<DbQueryEntry>>().Which;
        list.Should().HaveCount(1);
        list[0].DurationMs.Should().Be(0);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given exception capture enabled, " +
        "When adding log with error, " +
        "Then it stores exception message")]
    [Trait("Interceptors", nameof(EfQueryCounter))]
    public async Task AddLog_ExceptionMessage_StoresValue()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.ExceptionMessage]);
        var interceptor = CreateInterceptor(context, counter, settings);
        var command = new FakeDbCommand();

        //When
        InvokeAddLog(interceptor, command, false, "boom", 1);

        //Then
        var list = context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY]
            .Should().BeOfType<List<DbQueryEntry>>().Which;
        list.Should().HaveCount(1);
        list[0].Success.Should().BeFalse();
        list[0].ExceptionMessage.Should().Be("boom");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given context capture enabled, " +
        "When adding log, " +
        "Then it stores db context info")]
    [Trait("Interceptors", nameof(EfQueryCounter))]
    public async Task AddLog_Context_StoresDbContext()
    {
        //Given
        var context = new DefaultHttpContext();
        var counter = new FakeCounter();
        var settings = CreateSettings(enabled: true, [CaptureOptions.Context]);
        var interceptor = CreateInterceptor(context, counter, settings);
        var command = new FakeDbCommand();

        //When
        InvokeAddLog(interceptor, command, true, null, 1);

        //Then
        var list = context.Items[NedMonitorConstants.CONTEXT_QUERY_LOGS_KEY]
            .Should().BeOfType<List<DbQueryEntry>>().Which;
        list.Should().HaveCount(1);
        list[0].DbContext.Should().NotBeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given parameter values, " +
        "When formatting, " +
        "Then it formats null, string, datetime and bytes")]
    [Trait("Interceptors", nameof(EfQueryCounter))]
    public async Task FormatValue_FormatsKnownTypes()
    {
        //Given
        var format = typeof(EfQueryCounter)
            .GetMethod("FormatValue", BindingFlags.Static | BindingFlags.NonPublic)!;

        //When
        var nullValue = format.Invoke(null, [null]);
        var textValue = format.Invoke(null, ["text"]);
        var dateValue = format.Invoke(null, [new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)]);
        var bytesValue = format.Invoke(null, [new byte[3]]);

        //Then
        nullValue.Should().Be("null");
        textValue.Should().Be("\"text\"");
        dateValue.Should().Be("2020-01-01T00:00:00.0000000Z");
        bytesValue.Should().Be("byte[3]");
        await Task.CompletedTask;
    }

    private static NedMonitorSettings CreateSettings(bool enabled, List<CaptureOptions> captureOptions)
    {
        return new NedMonitorSettings
        {
            DataInterceptors = new DataInterceptorsSettings
            {
                EF = new EfInterceptorSettings
                {
                    Enabled = enabled,
                    CaptureOptions = captureOptions
                }
            }
        };
    }

    private static EfQueryCounter CreateInterceptor(
        DefaultHttpContext context,
        IQueryCounter counter,
        NedMonitorSettings settings)
    {
        var accessor = new HttpContextAccessor { HttpContext = context };
        var options = Options.Create(settings);
        return new EfQueryCounter(counter, accessor, options);
    }

    private static EfQueryCounter CreateInterceptorWithNullContext(
        IQueryCounter counter,
        NedMonitorSettings settings)
    {
        var accessor = new HttpContextAccessor();
        var options = Options.Create(settings);
        return new EfQueryCounter(counter, accessor, options);
    }

    private static void InvokeAddLog(
        EfQueryCounter interceptor,
        DbCommand command,
        bool success,
        string? exceptionMessage,
        double? durationMs)
    {
        var method = typeof(EfQueryCounter)
            .GetMethod("AddLog", BindingFlags.Instance | BindingFlags.NonPublic)!;
        method.Invoke(interceptor, [command, success, exceptionMessage, durationMs]);
    }

    private sealed class FakeCounter : IQueryCounter
    {
        public int Count { get; private set; }
        public void Increment() => Count++;
        public int GetCount(HttpContext context) => Count;
        public void Reset(HttpContext context) => Count = 0;
    }

    private sealed class FakeDbConnection : DbConnection
    {
        [AllowNull]
        public override string ConnectionString { get; set; } = "Data Source=Source;User Id=User";
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

    private sealed class FakeDbCommand : DbCommand
    {
        private readonly DbParameterCollection _parameters = new FakeParameterCollection();

        [AllowNull]
        public override string CommandText { get; set; } = "select 1";
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        [AllowNull]
        protected override DbConnection DbConnection { get; set; } = new FakeDbConnection();
        protected override DbTransaction? DbTransaction { get; set; }
        protected override DbParameterCollection DbParameterCollection => _parameters;
        public override bool DesignTimeVisible { get; set; }
        public override void Cancel() { }
        public override int ExecuteNonQuery() => 1;
        public override object ExecuteScalar() => 1;
        public override void Prepare() { }
        protected override DbParameter CreateDbParameter() => new FakeDbParameter("", null);
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => throw new NotSupportedException();
    }

    private sealed class FakeDbParameter : DbParameter
    {
        public FakeDbParameter(string name, object? value)
        {
            ParameterName = name;
            Value = value;
        }

        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;
        public override bool IsNullable { get; set; }
        [AllowNull]
        public override string ParameterName { get; set; }
        [AllowNull]
        public override string SourceColumn { get; set; } = string.Empty;
        public override object? Value { get; set; }
        public override bool SourceColumnNullMapping { get; set; }
        public override int Size { get; set; }
        public override void ResetDbType() { }
    }

    private sealed class FakeParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _items = [];

        public override int Count => _items.Count;
        public override object SyncRoot { get; } = new();

        public override int Add(object value)
        {
            _items.Add((DbParameter)value);
            return _items.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var value in values)
            {
                Add(value!);
            }
        }

        public override void Clear() => _items.Clear();
        public override bool Contains(object value) => _items.Contains((DbParameter)value);
        public override bool Contains(string value) => _items.Any(p => p.ParameterName == value);
        public override void CopyTo(Array array, int index) => _items.ToArray().CopyTo(array, index);
        public override System.Collections.IEnumerator GetEnumerator() => _items.GetEnumerator();
        public override int IndexOf(object value) => _items.IndexOf((DbParameter)value);
        public override int IndexOf(string parameterName) => _items.FindIndex(p => p.ParameterName == parameterName);
        public override void Insert(int index, object value) => _items.Insert(index, (DbParameter)value);
        public override void Remove(object value) => _items.Remove((DbParameter)value);
        public override void RemoveAt(int index) => _items.RemoveAt(index);
        public override void RemoveAt(string parameterName)
        {
            var index = IndexOf(parameterName);
            if (index >= 0) RemoveAt(index);
        }

        protected override DbParameter GetParameter(int index) => _items[index];
        protected override DbParameter GetParameter(string parameterName) => _items[IndexOf(parameterName)];
        protected override void SetParameter(int index, DbParameter value) => _items[index] = value;
        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var index = IndexOf(parameterName);
            if (index >= 0) _items[index] = value;
        }
    }
}
