using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NedMonitor.Core.Extensions;
using NedMonitor.Core.Models;
using NedMonitor.Core.Settings;
using NedMonitor.HttpRequests;
using System.Net;
using System.Reflection;
using Zypher.Json;
using Zypher.Notifications.Messages;

namespace NedMonitor.Builders;

/// <summary>
/// Builder responsible for constructing a <see cref="LogContextHttpRequest"/> by aggregating contextual
/// information such as HTTP request and response details, environment data, user identity, notifications,
/// log entries, exceptions, and diagnostics to be sent to NedMonitor.
/// </summary>
public class LogContextBuilder : ILogContextBuilder
{
    private readonly NedMonitorSettings _settings;
    private readonly SensitiveDataMasker _sensitiveDataMasker;

    private Snapshot _snapshot;
    private LogLevel _logLevel;
    private string? _errorCategory;
    private IEnumerable<Notification>? _notifications;
    private IEnumerable<LogEntry>? _logEntries;
    private IEnumerable<HttpRequestLogContext>? _httpClientLogs;
    private IEnumerable<DbQueryEntry>? _dbQueryEntries;
    private ExceptionInfo? _exception;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogContextBuilder"/> class.
    /// </summary>
    /// <param name="options">NedMonitor configuration settings.</param>
    /// <param name="sensitiveDataMasker">Service for masking sensitive data.</param>
    public LogContextBuilder(IOptions<NedMonitorSettings> options, SensitiveDataMasker sensitiveDataMasker)
    {
        _settings = options.Value;
        _sensitiveDataMasker = sensitiveDataMasker;
    }
    /// <summary>
    /// Initializes the builder with a pre-captured <see cref="Snapshot"/> instance containing contextual data.
    /// </summary>
    /// <param name="snapshot">The captured snapshot of the request/response context.</param>
    /// <returns>The current builder instance.</returns>
    public ILogContextBuilder WithSnapshot(Snapshot snapshot)
    {
        _snapshot = snapshot;
        return this;
    }
    /// <summary>
    /// Includes domain notifications from the snapshot into the log context.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ILogContextBuilder WithNotifications()
    {
        _notifications = _snapshot.Notifications ?? [];
        return this;
    }
    /// <summary>
    /// Includes collected log entries from the snapshot into the log context.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ILogContextBuilder WithLogEntries()
    {
        _logEntries = _snapshot.LogEntries ?? [];
        return this;
    }

    /// <summary>
    /// Includes any exception from the snapshot into the log context.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ILogContextBuilder WithException()
    {
        _exception = _snapshot.Exception;
        return this;
    }
    /// <summary>
    /// Includes any collected HTTP client logs from the snapshot into the log context.
    /// </summary>
    /// <returns>The current builder instance.</returns>
    public ILogContextBuilder WithHttpClientLogs()
    {
        _httpClientLogs = _snapshot.HttpClientLogs;
        return this;
    }
    /// <summary>
    /// Includes database query logs (e.g., from EF or Dapper) in the current log context,
    /// enabling detailed tracking of executed SQL statements during the request lifecycle.
    /// </summary>
    public ILogContextBuilder WithDbQueryLogs()
    {
        _dbQueryEntries = _snapshot.DbQueryEntries;
        return this;
    }

    /// <summary>
    /// Finalizes the builder configuration and returns a fully populated <see cref="LogContextHttpRequest"/> object.
    /// This method should be called after all <c>WithX</c> methods to include desired context.
    /// </summary>
    /// <returns>A constructed <see cref="LogContextHttpRequest"/> with aggregated data.</returns>
    public LogContextHttpRequest Build()
    {
        _logLevel = LogLevel.None;
        _errorCategory = null;
        return new LogContextHttpRequest
        {
            StartTimeUtc = _snapshot.StartTimeUtc,
            EndTimeUtc = _snapshot.EndTimeUtc,
            CorrelationId = _snapshot.CorrelationId,
            Uri = $"{_snapshot.Scheme}://{_snapshot.Host}{_snapshot.Path}",
            UriTemplate = $"{_snapshot.Scheme}://{_snapshot.Host}{_snapshot.PathTemplate}",
            TotalMilliseconds = _snapshot.TotalMilliseconds,
            TraceIdentifier = _snapshot.TraceId,
            RemotePort = _snapshot.RemotePort,
            LocalPort = _snapshot.LocalPort,
            LocalIpAddress = _snapshot.LocalIpAddress,
            Project = AddProject(),
            Environment = AddEnvironment(),
            User = AddUserContext(),
            Request = AddRequestInfo(),
            Response = AddResponseInfo(),
            Diagnostic = AddDiagnostics(),
            LogEntries = AddLogEntries(),
            Notifications = AddNotifications(),
            Exceptions = AddExceptions(),
            HttpClientLogs = AddHttpClientLogs(),
            DbQueryEntries = AddDbQueryEntries(),
            LogAttentionLevel = _logLevel.Map(),
            ErrorCategory = _errorCategory
        };
    }

    /// <summary>
    /// Gathers metadata about the current NedMonitor project such as ID, name, and type.
    /// </summary>
    /// <returns>A <see cref="ProjectInfoHttp"/> object containing project details.</returns>
    private ProjectInfoHttp AddProject() => new()
    {
        Id = _settings.ProjectId,
        Name = _settings.Name,
        Type = _settings.ProjectType,
        ExecutionMode = new()
        {
            EnableNedMonitor = _settings.ExecutionMode.EnableNedMonitor,
            EnableMonitorExceptions = _settings.ExecutionMode.EnableMonitorExceptions,
            EnableMonitorHttpRequests = _settings.ExecutionMode.EnableMonitorHttpRequests,
            EnableMonitorLogs = _settings.ExecutionMode.EnableMonitorLogs,
            EnableMonitorNotifications = _settings.ExecutionMode.EnableMonitorNotifications,
            EnableMonitorDbQueries = _settings.ExecutionMode.EnableMonitorDbQueries,
        },
        HttpLogging = new()
        {
            MaxResponseBodySizeInMb = _settings.HttpLogging.MaxResponseBodySizeInMb,
            CaptureResponseBody = _settings.HttpLogging.CaptureResponseBody,
            WritePayloadToConsole = _settings.HttpLogging.WritePayloadToConsole,
            CaptureCookies = _settings.HttpLogging.CaptureCookies,
        },
        SensitiveDataMasking = new()
        {
            Enabled = _settings.SensitiveDataMasking?.Enabled ?? false,
            SensitiveKeys = _settings.SensitiveDataMasking?.SensitiveKeys,
            MaskValue = _settings.SensitiveDataMasking?.MaskValue,
            SensitivePatterns = _settings.SensitiveDataMasking?.SensitivePatterns
        },
        Exceptions = new()
        {
            Expected = _settings.Exceptions.Expected
        },
        DataInterceptors = new()
        {
            Dapper = new()
            {
                Enabled = _settings.DataInterceptors?.Dapper.Enabled ?? false,
                CaptureOptions = _settings.DataInterceptors?.Dapper.CaptureOptions,
            },
            EF = new()
            {
                Enabled = _settings.DataInterceptors?.EF.Enabled ?? false,
                CaptureOptions = _settings.DataInterceptors?.EF.CaptureOptions,
            }
        },
        MinimumLogLevel = _settings.MinimumLogLevel
    };

    /// <summary>
    /// Collects environment-related details like machine name, environment name, thread ID, and application version.
    /// </summary>
    /// <returns>A <see cref="EnvironmentInfoHttpRequest"/> object describing the runtime environment.</returns>

    private EnvironmentInfoHttpRequest AddEnvironment() => new()
    {
        MachineName = Environment.MachineName,
        Name = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
        ApplicationVersion = Environment.GetEnvironmentVariable("BuildID") ?? Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "Unknown",
        ThreadId = _snapshot.ThreadId,
    };

    /// <summary>
    /// Extracts user identity and authentication-related data from the snapshot.
    /// </summary>
    /// <returns>A <see cref="UserInfoHttpRequest"/> object describing the current user context.</returns>

    private UserInfoHttpRequest AddUserContext()
    {
        return new UserInfoHttpRequest
        {
            Id = _snapshot.UserId,
            Name = _snapshot.UserName,
            Document = _snapshot.UserDocument,
            Account = _snapshot.UserAccount,
            AccountCode = _snapshot.UserAccountCode,
            Email = _snapshot.UserEmail,
            TenantId = _snapshot.TenantId,
            IsAuthenticated = _snapshot.IsAuthenticated,
            AuthenticationType = _snapshot.AuthenticationType,
            Roles = _snapshot.Roles,
            Claims = _snapshot.Claims
        };
    }

    /// <summary>
    /// Builds detailed HTTP request information including headers, metadata, and masked body content.
    /// </summary>
    /// <returns>A <see cref="RequestInfoHttpRequest"/> containing request information.</returns>

    private RequestInfoHttpRequest AddRequestInfo()
    {
        var requestBodyObj = GetRequestBody(_snapshot.RequestBody);

        return new RequestInfoHttpRequest
        {
            Id = _snapshot.RequestId,
            HttpMethod = _snapshot.Method,
            Host = _snapshot.Host,
            PathBase = _snapshot.PathBase,
            Path = _snapshot.Path,
            PathTemplate = _snapshot.PathTemplate,
            FullPath = _snapshot.FullPath,
            Referer = _snapshot.Referer,
            Scheme = _snapshot.Scheme,
            Protocol = _snapshot.Protocol,
            IsHttps = _snapshot.IsHttps,
            HasFormContentType = _snapshot.HasFormContentType,
            QueryString = _snapshot.QueryString,
            RouteValues = _snapshot.RouteValues,
            UserAgent = _snapshot.UserAgent,
            ClientId = _snapshot.ClientId,
            Headers = _sensitiveDataMasker.Mask(_snapshot.RequestHeaders),
            Cookies = _settings.HttpLogging.CaptureCookies ? _sensitiveDataMasker.Mask(_snapshot.RequestCookies) : null,
            ContentType = _snapshot.RequestContentType,
            ContentLength = _snapshot.RequestContentLength,
            Body = requestBodyObj,
            BodySize = _snapshot.RequestContentLength ?? 0,
            IsAjaxRequest = _snapshot.IsAjaxRequest,
            IpAddress = _snapshot.IpAddress,
        };
    }

    /// <summary>
    /// Builds detailed HTTP response information including status, headers, and masked body content.
    /// </summary>
    /// <returns>A <see cref="ResponseInfoHttpRequest"/> containing response information.</returns>

    private ResponseInfoHttpRequest AddResponseInfo()
    {
        var responseBodyObj = GetResponseBody(_snapshot.ResponseBody);

        return new ResponseInfoHttpRequest
        {
            StatusCode = (HttpStatusCode)_snapshot.StatusCode,
            ReasonPhrase = ReasonPhrases.GetReasonPhrase(_snapshot.StatusCode),
            Headers = _snapshot.ResponseHeaders,
            Body = responseBodyObj,
            BodySize = _snapshot.ResponseBodySize,
        };
    }

    /// <summary>
    /// Gathers diagnostic information such as memory usage and placeholder data for DB/cache dependencies.
    /// </summary>
    /// <returns>A <see cref="DiagnosticHttpRequest"/> object with diagnostic information.</returns>

    private DiagnosticHttpRequest AddDiagnostics() => new()
    {
        MemoryUsageMb = _snapshot.MemoryUsageMb,
        DbQueryCount = _snapshot.DbQueryCount,
        CacheHit = _snapshot.CacheHit,
        Dependencies = AddDependencies()
    };

    /// <summary>
    /// Maps captured log entries to their serializable format, if any exist.
    /// </summary>
    /// <returns>An enumerable of <see cref="LogEntryHttpRequest"/>.</returns>

    private IEnumerable<LogEntryHttpRequest> AddLogEntries()
    {
        if (_logEntries?.Any() != true) return [];

        return _logEntries.Select(e => new LogEntryHttpRequest
        {
            LogCategory = e.Category,
            LogSeverity = e.LogLevel,
            LogMessage = e.Message,
            MemberType = e.MemberType,
            MemberName = e.MemberName,
            SourceLineNumber = e.LineNumber,
            TimestampUtc = e.DateTime
        });
    }

    private IEnumerable<DbQueryEntryHttpRequest> AddDbQueryEntries()
    {
        if (_dbQueryEntries?.Any() != true) return [];

        return _dbQueryEntries.Select(e => new DbQueryEntryHttpRequest
        {
            Provider = e.Provider,
            Sql = e.Sql,
            Parameters = _sensitiveDataMasker.MaskString(e.Parameters),
            Success = e.Success,
            DbContext = e.DbContext,
            ExecutedAtUtc = e.ExecutedAtUtc,
            DurationMs = e.DurationMs,
            ExceptionMessage = e.ExceptionMessage,
            ORM = e.ORM,

        });
    }

    /// <summary>
    /// Maps domain notifications to their serializable format and updates the log severity and error category accordingly.
    /// </summary>
    /// <returns>An enumerable of <see cref="NotificationInfoHttpRequest"/>.</returns>

    private IEnumerable<NotificationInfoHttpRequest> AddNotifications()
    {
        if (_notifications?.Any() != true) return [];

        _logLevel = _notifications?.Any() == true
            ? _notifications.Max(n => n.LogLevel)
            : LogLevel.Information;

        if (_logLevel > LogLevel.Information)
            _errorCategory = "Notification";

        return _notifications.Select(n => new NotificationInfoHttpRequest
        {
            Id = n.Id,
            Key = n.Key,
            Value = n.Value,
            LogLevel = n.LogLevel,
            Timestamp = n.Timestamp
        });
    }

    /// <summary>
    /// Maps captured exception into a serializable format and sets log level to critical.
    /// </summary>
    /// <returns>An enumerable with a single <see cref="ExceptionInfoHttpRequest"/>, or empty if no exception exists.</returns>

    private IEnumerable<ExceptionInfoHttpRequest> AddExceptions()
    {
        if (_exception == null) return [];

        _logLevel = LogLevel.Critical;
        _errorCategory = "Error";

        return
        [
            new ExceptionInfoHttpRequest
            {
                Type = _exception.Type,
                Message = _exception.Message,
                Tracer = _exception.StackTrace,
                InnerException = _exception.InnerException,
                Source = _exception.Source,
                TimestampUtc = _exception.TimestampUtc
            }
        ];
    }
    /// <summary>
    /// Maps HTTP client log entries from the snapshot to a serializable format.
    /// </summary>
    /// <returns>An enumerable of <see cref="HttpClientLogInfoHttpRequest"/>.</returns>
    private IEnumerable<HttpClientLogInfoHttpRequest> AddHttpClientLogs()
    {
        if (_httpClientLogs?.Any() != true) return [];

        return _httpClientLogs.Select(n => new HttpClientLogInfoHttpRequest
        {
            StartTimeUtc = n.StartTime,
            EndTimeUtc = n.EndTime,
            Method = n.Method,
            Url = n.FullUrl,
            TemplateUrl = n.UrlTemplate,
            StatusCode = (HttpStatusCode)n.StatusCode,
            RequestBody = GetObjectBody(n.RequestBody),
            ResponseBody = GetObjectBody(n.ResponseBody),
            RequestHeaders = _sensitiveDataMasker.Mask(n.RequestHeaders),
            ResponseHeaders = _sensitiveDataMasker.Mask(n.ResponseHeaders),
            ExceptionType = n.ExceptionType,
            ExceptionMessage = n.ExceptionMessage,
            StackTrace = n.StackTrace

        });
    }

    private IEnumerable<DependencyInfoHttpRequest> AddDependencies()
    {
        if (_snapshot.Dependencies?.Any() != true) return [];

        return _snapshot.Dependencies.Select(n => new DependencyInfoHttpRequest
        {
            Type = n.Type,
            Target = n.Target,
            Success = n.Success,
            DurationMs = n.DurationMs
        });
    }
    /// <summary>
    /// Attempts to deserialize and mask sensitive data from a JSON string.
    /// </summary>
    /// <param name="body">The raw JSON body string.</param>
    /// <returns>A masked object if successful; otherwise, null.</returns>
    private object? GetObjectBody(string body)
    {
        if (string.IsNullOrEmpty(body)) return null;

        try
        {
            var formattedJson = JsonExtensions.TryFormatJson(body);
            var obj = JsonExtensions.Deserialize<object>(formattedJson);
            return _sensitiveDataMasker.Mask(obj);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to deserialize and mask sensitive information from the response body.
    /// </summary>
    /// <param name="responseObj">The raw response body object.</param>
    /// <returns>The masked object or original string if deserialization fails.</returns>

    private object? GetResponseBody(object responseObj)
    {
        if (responseObj is string responseJson)
        {
            try
            {
                var formattedJson = JsonExtensions.TryFormatJson(responseJson);
                var obj = JsonExtensions.Deserialize<object>(formattedJson);
                return _sensitiveDataMasker.Mask(obj);
            }
            catch
            {
                return responseJson;
            }
        }
        return _sensitiveDataMasker.Mask(responseObj);
    }
    /// <summary>
    /// Extracts and masks sensitive information from the request body, including results from Task-based types.
    /// </summary>
    /// <param name="requestObj">The raw request body object.</param>
    /// <returns>The masked request object.</returns>

    private object? GetRequestBody(object requestObj)
    {
        if (requestObj is Task task)
        {
            var resultProperty = task.GetType().GetProperty("Result");
            var body = resultProperty?.GetValue(task);

            return _sensitiveDataMasker.Mask(body);
        }
        return requestObj;
    }
}
