using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NedMonitor.Core.Extensions;
using NedMonitor.Core.Settings;
using NedMonitor.Extensions;
using NedMonitor.HttpRequests;
using NedMonitor.HttpResponses;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;

namespace NedMonitor.HttpServices;

/// <summary>
/// HTTP service responsible for sending structured log data to the NedMonitor logging API endpoint.
/// Handles serialization, header injection, error parsing, and public logging.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NedMonitorHttpService"/> class.
/// </remarks>
/// <param name="httpClient">An instance of <see cref="HttpClient"/> configured for outbound requests.</param>
/// <param name="logger">Typed logger for capturing public service operations.</param>
/// <param name="options">Injected settings containing the NedMonitor configuration.</param>
internal class NedMonitorHttpService(HttpClient httpClient, ILogger<NedMonitorHttpService> logger, IOptions<NedMonitorSettings> options) : INedMonitorHttpService
{
    /// <summary>
    /// HTTP client instance used for sending requests.
    /// </summary>
    private readonly HttpClient _httpClient = httpClient;
    /// <summary>
    /// Logger for logging request and response inf
    /// </summary>
    private readonly ILogger<NedMonitorHttpService> _logger = logger;
    private readonly NedMonitorSettings _settings = options.Value;
    /// <summary>
    /// Indicates whether detailed request and response logging (headers and body) is enabled.
    /// </summary>
    private bool IsDetailedLoggingEnabled;

    /// <summary>
    /// Sends a <see cref="LogContextHttpRequest"/> payload to the configured NedMonitor API endpoint.
    /// If enabled, writes the outgoing payload to the console before transmission.
    /// </summary>
    /// <param name="log">The structured log payload to send to NedMonitor.</param>
    public async Task Flush(LogContextHttpRequest log)
    {
        try
        {
            var uri = _settings.RemoteService.Endpoint;

            var content = JsonExtensions.SerializeContent(log);

            using var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content
            };

            AddDefaultHeaders(request, log);

            if (_settings.HttpLogging.WritePayloadToConsole)
                EnableLogHeadersAndBody();

            LogRequest(request);
            var stopwatch = Stopwatch.StartNew();
            var response = await _httpClient.SendAsync(request);

            LogResponse(request, response, stopwatch);

            if (response.HasErrors())
                await Print(response);
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR"))}|CRIT|{log.CorrelationId}|[NedMonitor]|" + ex.Message);
        }
    }
    /// <summary>
    /// Reads and logs detailed error information returned from the NedMonitor API,
    /// including correlation ID, status, issue types, and diagnostic details.
    /// </summary>
    /// <param name="response">The HTTP response object containing an error payload.</param>
    /// <exception cref="HttpRequestException">Thrown when response deserialization fails.</exception>
    private async Task Print(HttpResponseMessage response)
    {
        StringBuilder sb = new();
        ApiHttpResponse? apiResponse;

        try
        {
            apiResponse = await response.ReadAsAsync<ApiHttpResponse>();

            if (apiResponse is null)
                throw new HttpRequestException();
        }
        catch (Exception)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.BadGateway:
                    throw new HttpRequestException($"{response.RequestMessage?.Method} - {response.RequestMessage?.RequestUri} - {(int)response.StatusCode} - {response.StatusCode}",
                    null,
                    response.StatusCode);
                default:
                    return;
            }

        }

        sb.AppendLine($"{apiResponse.CorrelationId}|[NedMonitor]|StatusCode:{(int)response.StatusCode} - {response.StatusCode}");

        if (apiResponse?.Issues == null || !apiResponse.Issues.Any())
        {
            _logger.LogError(sb.ToString() + "|No structured issues payload.");
            return;
        }

        foreach (var issue in apiResponse.Issues)
        {
            sb.AppendLine($"{apiResponse.CorrelationId}|[NedMonitor]|Type:{issue.DescriptionType}{(string.IsNullOrEmpty(issue.Title) ? string.Empty : $" - Title:{issue.Title}")}");

            if (issue.Details?.Any() is not true)
            {
                sb.AppendLine($"{apiResponse.CorrelationId}|[NedMonitor]|No details available.");
                continue;
            }

            foreach (var detail in issue.Details)
                sb.AppendLine($"{apiResponse.CorrelationId}|[NedMonitor]|Level:{detail.LogLevel} - Key:{detail.Key} - Value:{detail.Value}");

            switch (apiResponse.Issues.FirstOrDefault()?.Type)
            {
                case IssuerResponseType.NotFound:
                case IssuerResponseType.Validation:
                    _logger.LogWarning(sb.ToString());
                    break;
                case IssuerResponseType.Error:
                    _logger.LogError(sb.ToString());
                    break;
            }
        }
    }
    #region Headers
    /// <summary>
    /// Injects common headers (such as user ID, IP, client ID, and correlation ID)
    /// into the outbound HTTP request before sending logs to NedMonitor.
    /// </summary>
    /// <param name="log">The current <see cref="LogContextHttpRequest"/> being sent.</param>
    private void AddDefaultHeaders(HttpRequestMessage request, LogContextHttpRequest log)
    {
        AddHeaderIpAddress(request, log);
        AddHeaderUserId(request, log);
        AddHeaderCorrelationId(request, log);
        AddHeaderClientId(request, log);
        AddHeaderUserAgent(request, log);
        AddHeaderServerHostName(request);
        AddHeaderUserAccount(request, log);
        AddHeaderUserAccountCode(request, log);
    }
    /// <summary>
    /// Adds the client IP address to the request headers.
    /// </summary>
    private void AddHeaderIpAddress(HttpRequestMessage request, LogContextHttpRequest log)
    {
        var ip = log.Request.IpAddress;

        if (!string.IsNullOrEmpty(ip))
            request.AddOrUpdateHeader(HttpRequestExtensions.FORWARDED, ip);
    }
    /// <summary>
    /// Adds the user ID to the request headers.
    /// </summary>
    private void AddHeaderUserId(HttpRequestMessage request, LogContextHttpRequest log)
    {
        var userId = log.User.Id;

        if (!string.IsNullOrEmpty(userId))
            request.AddOrUpdateHeader(HttpRequestExtensions.USER_ID, userId);
    }
    /// <summary>
    /// Adds the correlation ID to the request headers.
    /// </summary>
    private void AddHeaderCorrelationId(HttpRequestMessage request, LogContextHttpRequest log)
    {
        var correlationId = log.CorrelationId;

        if (!string.IsNullOrEmpty(correlationId))
            request.AddOrUpdateHeader(HttpRequestExtensions.CORRELATION_ID, correlationId);
    }
    /// <summary>
    /// Adds the client ID and application name to the request headers.
    /// </summary>
    private void AddHeaderClientId(HttpRequestMessage request, LogContextHttpRequest log)
    {
        var clientId = log.Request.ClientId;

        if (!string.IsNullOrEmpty(clientId))
            clientId = string.Join(';', clientId, Assembly.GetEntryAssembly().GetName().Name);
        else
            clientId = Assembly.GetEntryAssembly().GetName().Name;

        if (!string.IsNullOrEmpty(clientId))
            request.AddOrUpdateHeader(HttpRequestExtensions.CLIENT_ID, clientId);
    }
    /// <summary>
    /// Adds the user-agent string to the request headers.
    /// </summary>
    private void AddHeaderUserAgent(HttpRequestMessage request, LogContextHttpRequest log)
    {
        var userAgent = log.Request.UserAgent;

        if (!string.IsNullOrEmpty(userAgent))
            request.AddOrUpdateHeader(HttpRequestExtensions.USER_AGENT, userAgent);
    }
    /// <summary>
    /// Adds the current server or pod host name to the request headers.
    /// </summary>
    private void AddHeaderServerHostName(HttpRequestMessage request)
    {
        var podeName = Dns.GetHostName();

        if (!string.IsNullOrEmpty(podeName))
            request.AddOrUpdateHeader(HttpRequestExtensions.POD_NAME, podeName);
    }

    /// <summary>
    /// Adds the user account identifier to the request headers.
    /// </summary>
    private void AddHeaderUserAccount(HttpRequestMessage request, LogContextHttpRequest log)
    {
        var userAccount = log.User.Account;

        if (!string.IsNullOrEmpty(userAccount))
            request.AddOrUpdateHeader(HttpRequestExtensions.USER_ACCOUNT, userAccount);
    }
    /// <summary>
    /// Adds the user account code to the request headers.
    /// </summary>
    private void AddHeaderUserAccountCode(HttpRequestMessage request, LogContextHttpRequest log)
    {
        var userAccountCode = log.User.AccountCode;

        if (!string.IsNullOrEmpty(userAccountCode))
            request.AddOrUpdateHeader(HttpRequestExtensions.USER_ACCOUNT_CODE, userAccountCode);
    }
    #endregion

    #region Logs
    private void EnableLogHeadersAndBody() => IsDetailedLoggingEnabled = true;
    /// <summary>
    /// Logs the start of an HTTP request including method, URI, headers, and optional content.
    /// </summary>
    private void LogRequest(HttpRequestMessage request)
    {
        var headersJson = string.Empty;
        var contentJson = string.Empty;
        if (IsDetailedLoggingEnabled)
        {
            headersJson = $"|Headers:{request.GetHeadersJsonFormat()}";

            if (request?.Content is not null)
                contentJson = $"|Content:{request.Content?.ReadAsStringAsync().Result}";
        }
        _logger.LogInformation($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR"))}|INFO|{request.GetHeader(HttpRequestExtensions.CORRELATION_ID)}|[NedMonitor]|Start processing HTTP request {request?.Method.Method} {request?.RequestUri!}{headersJson}{contentJson}");
    }

    /// <summary>
    /// Logs the end of an HTTP request including method, URI, elapsed time and response status.
    /// </summary>
    private void LogResponse(HttpRequestMessage request, HttpResponseMessage response, Stopwatch stopwatch)
    {
        stopwatch.Stop();

        string message = $"{request.GetHeader(HttpRequestExtensions.CORRELATION_ID)}|[NedMonitor]|End processing HTTP request {response?.RequestMessage?.Method} {response?.RequestMessage?.RequestUri} after {stopwatch.GetFormattedTime()} - {(int)response.StatusCode} - {response.StatusCode}";

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
            case HttpStatusCode.Created:
            case HttpStatusCode.Accepted:
            case HttpStatusCode.NoContent:
            case HttpStatusCode.Continue:
            case HttpStatusCode.ResetContent:
            case HttpStatusCode.PartialContent:
            case HttpStatusCode.MultiStatus:
            case HttpStatusCode.AlreadyReported:
            case HttpStatusCode.IMUsed:
                _logger.LogInformation($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR"))}|INFO|{message}");
                break;
            case HttpStatusCode.BadRequest:
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.PaymentRequired:
            case HttpStatusCode.Forbidden:
            case HttpStatusCode.MethodNotAllowed:
            case HttpStatusCode.NotAcceptable:
            case HttpStatusCode.ProxyAuthenticationRequired:
            case HttpStatusCode.RequestTimeout:
            case HttpStatusCode.Conflict:
            case HttpStatusCode.Gone:
            case HttpStatusCode.LengthRequired:
            case HttpStatusCode.PreconditionFailed:
            case HttpStatusCode.RequestEntityTooLarge:
            case HttpStatusCode.RequestUriTooLong:
            case HttpStatusCode.UnsupportedMediaType:
            case HttpStatusCode.RequestedRangeNotSatisfiable:
            case HttpStatusCode.ExpectationFailed:
            case HttpStatusCode.MisdirectedRequest:
            case HttpStatusCode.UnprocessableEntity:
            case HttpStatusCode.Locked:
            case HttpStatusCode.FailedDependency:
            case HttpStatusCode.UpgradeRequired:
            case HttpStatusCode.PreconditionRequired:
            case HttpStatusCode.TooManyRequests:
            case HttpStatusCode.RequestHeaderFieldsTooLarge:
            case HttpStatusCode.UnavailableForLegalReasons:
                _logger.LogWarning($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR"))}|WARN|{message}");
                break;
            case HttpStatusCode.NotFound:
            case HttpStatusCode.InternalServerError:
            case HttpStatusCode.NotImplemented:
            case HttpStatusCode.BadGateway:
            case HttpStatusCode.ServiceUnavailable:
            case HttpStatusCode.GatewayTimeout:
            case HttpStatusCode.HttpVersionNotSupported:
            case HttpStatusCode.VariantAlsoNegotiates:
            case HttpStatusCode.InsufficientStorage:
            case HttpStatusCode.LoopDetected:
            case HttpStatusCode.NotExtended:
            case HttpStatusCode.NetworkAuthenticationRequired:
                _logger.LogError($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR"))}|FAIL|{message}");
                break;
            default:
                _logger.LogCritical($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR"))}|CRIT|{message}");
                break;
        }
    }
    #endregion
}
