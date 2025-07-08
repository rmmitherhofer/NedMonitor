using Api.Responses;
using Common.Exceptions;
using Common.Extensions;
using Common.Http;
using Common.Http.Extensions;
using Common.Json;
using Common.Logs.Extensions;
using Common.Notifications.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using Tracezilla.Configurations.Settings;
using Tracezilla.Models;

namespace Tracezilla.HttpServices;

/// <summary>
/// HTTP service responsible for communicating with the Tracezilla logging endpoint.
/// </summary>
public class TracezillaHttpService : HttpService, ITracezillaHttpService
{
    private readonly TracezillaSettings _settings;

    /// <summary>
    /// Initializes a new instance of <see cref="TracezillaHttpService"/>.
    /// </summary>
    /// <param name="httpClient">The configured HTTP client for sending requests.</param>
    /// <param name="notification">Notification handler for capturing domain notifications.</param>
    /// <param name="logger">Logger instance for internal logging.</param>
    /// <param name="options">Tracezilla configuration settings.</param>
    public TracezillaHttpService(
        HttpClient httpClient,
        INotificationHandler notification,
        ILogger<TracezillaHttpService> logger,
        IOptions<TracezillaSettings> options
    ) : base(httpClient, notification, logger) => _settings = options.Value;

    /// <summary>
    /// Sends the given <see cref="LogContextRequest"/> to the Tracezilla API.
    /// </summary>
    /// <param name="log">The log context to be transmitted.</param>
    public async Task Flush(LogContextRequest log)
    {
        _httpClient.AddHeader(HttpRequestExtensions.CORRELATION_ID, log.CorrelationId);
        _httpClient.AddHeader(HttpRequestExtensions.FORWARDED, log.Request.IpAddress);
        _httpClient.AddHeader(HttpRequestExtensions.USER_AGENT, log.Request.UserAgent);

        var uri = _settings.Service.EndPoints.Notify;

        var content = JsonExtensions.SerializeContent(log);

        if (_settings.WritePayloadToConsole)
            EnableLogHeadersAndBody();  

        var response = await PostAsync(uri, content);

        try
        {
            if (response.HasErrors())
                await Print(response);
        }
        catch (CustomHttpRequestException ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    /// <summary>
    /// Reads and prints the details from an HTTP error response returned by the Tracezilla API.
    /// </summary>
    /// <param name="response">The HTTP response containing error details.</param>
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

        sb.AppendLine($"[Tracezilla]|{apiResponse.CorrelationId}|StatusCode:{(int)response.StatusCode} - {response.StatusCode}");

        foreach (var issue in apiResponse.Issues)
        {
            sb.AppendLine($"[Tracezilla]|{apiResponse.CorrelationId}|Type:{issue.DescriptionType}{(string.IsNullOrEmpty(issue.Title)? string.Empty : $" - Title:{issue.Title}")}");

            if(issue.Details?.Any() is not true)
            {
                sb.AppendLine($"[Tracezilla]|{apiResponse.CorrelationId}|No details available.");
                continue;
            }

            foreach (var detail in issue.Details)
                sb.AppendLine($"[Tracezilla]|{apiResponse.CorrelationId}|Level:{detail.LogLevel} - Key:{detail.Key} - Value:{detail.Value}");

            switch (apiResponse.Issues.FirstOrDefault().Type)
            {
                case IssuerResponseType.NotFound:
                case IssuerResponseType.Validation:
                    _logger.LogWarn(sb.ToString());
                    break;
                case IssuerResponseType.Error:
                    _logger.LogFail(sb.ToString());
                    break;
            }
        }
    }
}
