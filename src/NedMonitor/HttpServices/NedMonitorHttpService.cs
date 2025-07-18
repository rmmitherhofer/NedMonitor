using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NedMonitor.Core.Settings;
using NedMonitor.HttpRequests;
using System.Net;
using System.Reflection;
using System.Text;
using Zypher.Extensions.Core;
using Zypher.Http;
using Zypher.Http.Exceptions;
using Zypher.Http.Extensions;
using Zypher.Json;
using Zypher.Logs.Extensions;
using Zypher.Notifications.Interfaces;
using Zypher.Responses;

namespace NedMonitor.HttpServices;

/// <summary>
/// HTTP service responsible for sending structured log data to the NedMonitor logging API endpoint.
/// Handles serialization, header injection, error parsing, and internal logging.
/// </summary>
public class NedMonitorHttpService : HttpService, INedMonitorHttpService
{
    private readonly NedMonitorSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="NedMonitorHttpService"/> class.
    /// </summary>
    /// <param name="httpClient">An instance of <see cref="HttpClient"/> configured for outbound requests.</param>
    /// <param name="notification">Domain notification handler for validation and processing errors.</param>
    /// <param name="logger">Typed logger for capturing internal service operations.</param>
    /// <param name="options">Injected settings containing the NedMonitor configuration.</param>

    public NedMonitorHttpService(
        HttpClient httpClient,
        INotificationHandler notification,
        ILogger<NedMonitorHttpService> logger,
        IOptions<NedMonitorSettings> options
    ) : base(httpClient, notification, logger) => _settings = options.Value;

    /// <summary>
    /// Sends a <see cref="LogContextHttpRequest"/> payload to the configured NedMonitor API endpoint.
    /// If enabled, writes the outgoing payload to the console before transmission.
    /// </summary>
    /// <param name="log">The structured log payload to send to NedMonitor.</param>
    public async Task Flush(LogContextHttpRequest log)
    {
        try
        {
            var uri = _settings.RemoteService.Endpoints.NotifyLogContext;

            var content = JsonExtensions.SerializeContent(log);

            AddDefaultHeaders(log);

            if (_settings.HttpLogging.WritePayloadToConsole)
                EnableLogHeadersAndBody();

            LogRequest(HttpMethod.Post.Method, new Uri(_httpClient.BaseAddress! + uri), content);

            var response = await _httpClient.PostAsync(uri, content);

            LogResponse(response);

            if (response.HasErrors())
                await Print(response);
        }
        catch (CustomHttpRequestException ex)
        {
            _logger.LogFail($"{log.CorrelationId}|" + ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogCrit($"{log.CorrelationId}|" + ex.Message);
        }
    }
    /// <summary>
    /// Reads and logs detailed error information returned from the NedMonitor API,
    /// including correlation ID, status, issue types, and diagnostic details.
    /// </summary>
    /// <param name="response">The HTTP response object containing an error payload.</param>
    /// <exception cref="CustomHttpRequestException">Thrown when response deserialization fails.</exception>
    private async Task Print(HttpResponseMessage response)
    {
        StringBuilder sb = new();
        ApiResponse apiResponse = null;

        try
        {
            apiResponse = await response.ReadAsAsync<ApiResponse>();

            if (apiResponse is null)
                throw new CustomHttpRequestException();
        }
        catch (Exception)
        {
            throw new CustomHttpRequestException(
                response.StatusCode,
                $"{response.RequestMessage.Method} - {response.RequestMessage.RequestUri} - {(int)response.StatusCode} - {response.StatusCode}"
            );
        }

        sb.AppendLine($"[NedMonitor]{apiResponse.CorrelationId}|StatusCode:{(int)response.StatusCode} - {response.StatusCode}");

        foreach (var issue in apiResponse.Issues)
        {
            sb.AppendLine($"[NedMonitor]{apiResponse.CorrelationId}|Type:{issue.DescriptionType}{(string.IsNullOrEmpty(issue.Title) ? string.Empty : $" - Title:{issue.Title}")}");

            if (issue.Details?.Any() is not true)
            {
                sb.AppendLine($"[NedMonitor]{apiResponse.CorrelationId}|No details available.");
                continue;
            }

            foreach (var detail in issue.Details)
                sb.AppendLine($"[NedMonitor]{apiResponse.CorrelationId}|Level:{detail.LogLevel} - Key:{detail.Key} - Value:{detail.Value}");

            switch (apiResponse.Issues.FirstOrDefault().Type)
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
    /// <summary>
    /// Injects common headers (such as user ID, IP, client ID, and correlation ID)
    /// into the outbound HTTP request before sending logs to NedMonitor.
    /// </summary>
    /// <param name="log">The current <see cref="LogContextHttpRequest"/> being sent.</param>
    private void AddDefaultHeaders(LogContextHttpRequest log)
    {
        AddHeaderIpAddress(log);
        AddHeaderUserId(log);
        AddHeaderCorrelationId(log);
        AddHeaderClientId(log);
        AddHeaderUserAgent(log);
        AddHeaderServerHostName();
        AddHeaderUserAccount(log);
        AddHeaderUserAccountCode(log);
    }
    /// <summary>
    /// Adds the client IP address to the request headers.
    /// </summary>
    private void AddHeaderIpAddress(LogContextHttpRequest log)
    {
        var ip = log.Request.IpAddress;

        if (!string.IsNullOrEmpty(ip))
            _httpClient.AddHeader(HttpRequestExtensions.FORWARDED, ip);
    }
    /// <summary>
    /// Adds the user ID to the request headers.
    /// </summary>
    private void AddHeaderUserId(LogContextHttpRequest log)
    {
        var userId = log.User.Id;

        if (!string.IsNullOrEmpty(userId))
            _httpClient.AddHeader(HttpRequestExtensions.USER_ID, userId);
    }
    /// <summary>
    /// Adds the correlation ID to the request headers.
    /// </summary>
    private void AddHeaderCorrelationId(LogContextHttpRequest log)
    {
        var correlationId = log.CorrelationId;

        if (!string.IsNullOrEmpty(correlationId))
            _httpClient.AddHeader(HttpRequestExtensions.CORRELATION_ID, correlationId);
    }
    /// <summary>
    /// Adds the client ID and application name to the request headers.
    /// </summary>
    private void AddHeaderClientId(LogContextHttpRequest log)
    {
        var clientId = log.Request.ClientId;

        if (!string.IsNullOrEmpty(clientId))
            clientId = string.Join(';', clientId, Assembly.GetEntryAssembly().GetName().Name);
        else
            clientId = Assembly.GetEntryAssembly().GetName().Name;

        if (!string.IsNullOrEmpty(clientId))
            _httpClient.AddHeader(HttpRequestExtensions.CLIENT_ID, clientId);
    }
    /// <summary>
    /// Adds the user-agent string to the request headers.
    /// </summary>
    private void AddHeaderUserAgent(LogContextHttpRequest log)
    {
        var userAgent = log.Request.UserAgent;

        if (!string.IsNullOrEmpty(userAgent))
            _httpClient.AddHeader(HttpRequestExtensions.USER_AGENT, userAgent);
    }
    /// <summary>
    /// Adds the current server or pod host name to the request headers.
    /// </summary>
    private void AddHeaderServerHostName()
    {
        var podeName = Dns.GetHostName();

        if (!string.IsNullOrEmpty(podeName))
            _httpClient.AddHeader(HttpRequestExtensions.POD_NAME, podeName);
    }

    /// <summary>
    /// Adds the user account identifier to the request headers.
    /// </summary>
    private void AddHeaderUserAccount(LogContextHttpRequest log)
    {
        var userAccount = log.User.Account;

        if (!string.IsNullOrEmpty(userAccount))
            _httpClient.AddHeader(HttpRequestExtensions.USER_ACCOUNT, userAccount);
    }
    /// <summary>
    /// Adds the user account code to the request headers.
    /// </summary>
    private void AddHeaderUserAccountCode(LogContextHttpRequest log)
    {
        var userAccountCode = log.User.AccountCode;

        if (!string.IsNullOrEmpty(userAccountCode))
            _httpClient.AddHeader(HttpRequestExtensions.USER_ACCOUNT_CODE, userAccountCode);
    }
}
