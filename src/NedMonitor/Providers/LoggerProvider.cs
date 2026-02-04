using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NedMonitor.Core.Adapters;
using NedMonitor.Core.Settings;

namespace NedMonitor.Providers;

/// <summary>
/// Logger provider for NedMonitor logger.
/// </summary>
[ProviderAlias("NedMonitor")]
internal class LoggerProvider : ILoggerProvider
{
    private readonly FormatterOptions _options;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<NedMonitorSettings> _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerProvider"/> class.
    /// </summary>
    /// <param name="options">The logger options.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public LoggerProvider(FormatterOptions options, IHttpContextAccessor httpContextAccessor, IOptions<NedMonitorSettings> settings)
    {
        _options = options;
        _httpContextAccessor = httpContextAccessor;
        _settings = settings;
    }

    /// <summary>
    /// Creates a new NedMonitor logger.
    /// </summary>
    /// <param name="categoryName">The logger category name.</param>
    /// <returns>The created logger.</returns>
    public ILogger CreateLogger(string categoryName) => new LoggerAdapter(_options, categoryName, _httpContextAccessor, _settings);

    /// <summary>
    /// Disposes the provider. No-op.
    /// </summary>
    public void Dispose() { }
}
