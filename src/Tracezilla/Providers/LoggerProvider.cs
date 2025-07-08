using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Tracezilla.Adapters;
using Tracezilla.Configurations.Settings;

namespace Tracezilla.Providers;

/// <summary>
/// Logger provider for Tracezilla logger.
/// </summary>
[ProviderAlias("Tracezilla")]
public class LoggerProvider : ILoggerProvider
{
    private readonly FormatterOptions _options;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerProvider"/> class.
    /// </summary>
    /// <param name="options">The logger options.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public LoggerProvider(FormatterOptions options, IHttpContextAccessor httpContextAccessor)
    {
        _options = options;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Creates a new Tracezilla logger.
    /// </summary>
    /// <param name="categoryName">The logger category name.</param>
    /// <returns>The created logger.</returns>
    public ILogger CreateLogger(string categoryName) => new TracezillaLoggerAdapter(_options, categoryName, _httpContextAccessor);

    /// <summary>
    /// Disposes the provider. No-op.
    /// </summary>
    public void Dispose() { }
}
