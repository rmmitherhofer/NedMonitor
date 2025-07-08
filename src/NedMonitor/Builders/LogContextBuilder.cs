using Common.Extensions;
using Common.Json;
using Common.Notifications.Messages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NedMonitor.Configurations.Settings;
using NedMonitor.Extensions;
using NedMonitor.Models;
using System.Net;
using System.Reflection;

namespace NedMonitor.Builders;

/// <summary>
/// Builder responsible for constructing a <see cref="LogContextRequest"/> by aggregating contextual
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
    private IEnumerable<Notification> _notifications;
    private IEnumerable<LogEntry> _logEntries;
    private Exception? _exception;

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
    /// Builds and returns a fully constructed <see cref="LogContextRequest"/> object with all aggregated data.
    /// </summary>
    /// <returns>A populated instance of <see cref="LogContextRequest"/>.</returns>
    public LogContextRequest Build()
    {
        return new LogContextRequest
        {
            CorrelationId = _snapshot.CorrelationId,
            Path = _snapshot.Path,
            ElapsedMilliseconds = _snapshot.ElapsedMilliseconds,
            TraceIdentifier = _snapshot.TraceId,
            Project = AddProject(),
            Environment = AddEnvironment(),
            User = AddUserContext(),
            Request = AddRequestInfo(),
            Response = AddResponseInfo(),
            Diagnostic = AddDiagnostics(),
            LogEntries = AddLogEntries(),            
            Notifications = AddNotifications(),
            Exceptions = AddExceptions(),
            LogAttentionLevel = _logLevel.Map(),
            ErrorCategory = _errorCategory
        };
    }

    /// <summary>
    /// Gathers metadata about the current NedMonitor project such as ID, name, and type.
    /// </summary>
    /// <returns>A <see cref="ProjectInfo"/> object containing project details.</returns>
    private ProjectInfo AddProject() => new()
    {
        Id = _settings.ProjectId,
        Name = _settings.Name,
        Type = _settings.ProjectType,
        ExecutionMode = _settings.ExecutionMode,
        MaxResponseBodySizeInMb = _settings.MaxResponseBodySizeInMb,
        CaptureResponseBody = _settings.CaptureResponseBody,
        WritePayloadToConsole = _settings.WritePayloadToConsole
    };

    /// <summary>
    /// Collects environment-related details like machine name, environment name, thread ID, and application version.
    /// </summary>
    /// <returns>A <see cref="EnvironmentInfo"/> object describing the runtime environment.</returns>

    private EnvironmentInfo AddEnvironment() => new()
    {
        MachineName = Environment.MachineName,
        Name = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
        ApplicationVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "Unknown",
        ThreadId = _snapshot.ThreadId,
    };

    /// <summary>
    /// Extracts user identity and authentication-related data from the snapshot.
    /// </summary>
    /// <returns>A <see cref="UserContextRequest"/> object describing the current user context.</returns>

    private UserContextRequest AddUserContext()
    {
        return new UserContextRequest
        {
            Id = _snapshot.UserId,
            Name = _snapshot.UserName,
            Document = _snapshot.UserDocument,
            Account = _snapshot.UserAccount,
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
    /// <returns>A <see cref="RequestInfoRequest"/> containing request information.</returns>

    private RequestInfoRequest AddRequestInfo()
    {
        var requestBodyObj = GetRequestBody(_snapshot.RequestBody);

        return new RequestInfoRequest
        {
            Id = _snapshot.RequestId,
            HttpMethod = _snapshot.Method,
            Url = _snapshot.Url,
            Scheme = _snapshot.Scheme,
            Protocol = _snapshot.Protocol,
            IsHttps = _snapshot.IsHttps,
            QueryString = _snapshot.QueryString,
            RouteValues = _snapshot.RouteValues,
            UserAgent = _snapshot.UserAgent,
            ClientId = _snapshot.ClientId,
            Headers = _snapshot.RequestHeaders,
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
    /// <returns>A <see cref="ResponseInfoRequest"/> containing response information.</returns>

    private ResponseInfoRequest AddResponseInfo()
    {
        var responseBodyObj = GetResponseBody(_snapshot.ResponseBody);

        return new ResponseInfoRequest
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
    /// <returns>A <see cref="DiagnosticRequest"/> object with diagnostic information.</returns>

    private DiagnosticRequest AddDiagnostics() => new()
    {
        MemoryUsageMb = _snapshot.MemoryUsageMb,
        DbQueryCount = 0,
        CacheHit = false,
        Dependencies = []
    };

    /// <summary>
    /// Maps captured log entries to their serializable format, if any exist.
    /// </summary>
    /// <returns>An enumerable of <see cref="LogEntryRequest"/>.</returns>

    private IEnumerable<LogEntryRequest> AddLogEntries()
    {
        if(_logEntries?.Any() != true) return [];

        return _logEntries.Select(e => new LogEntryRequest
        {
            LogCategory = e.Category,
            LogSeverity = e.LogLevel,
            LogMessage = e.Message,
            MemberType = e.MemberType,
            MemberName = e.MemberName,
            SourceLineNumber = e.LineNumber,
            Timestamp = e.DateTime
        });
    }

    /// <summary>
    /// Maps domain notifications to their serializable format and updates the log severity and error category accordingly.
    /// </summary>
    /// <returns>An enumerable of <see cref="NotificationInfoRequest"/>.</returns>

    private IEnumerable<NotificationInfoRequest> AddNotifications()
    {
        if (_notifications?.Any() != true) return [];

        _logLevel = _notifications?.Any() == true
            ? _notifications.Max(n => n.LogLevel)
            : LogLevel.Information;

        if (_logLevel > LogLevel.Information)
            _errorCategory = "Notification";

        return _notifications.Select(n => new NotificationInfoRequest
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
    /// <returns>An enumerable with a single <see cref="ExceptionInfoRequest"/>, or empty if no exception exists.</returns>

    private IEnumerable<ExceptionInfoRequest> AddExceptions()
    {
        if (_exception == null) return [];

        _logLevel = LogLevel.Critical;
        _errorCategory = "Error";

        return
        [
            new ExceptionInfoRequest
            {
                Type = _exception.GetType().FullName,
                Message = _exception.Message,
                Tracer = _exception.StackTrace,
                InnerException = _exception?.InnerException?.Message
            }
        ];
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
